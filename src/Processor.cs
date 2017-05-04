using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlickrNet;
using YetAnotherFlickrUploader.Helpers;
using YetAnotherFlickrUploader.Services;

using YetAnotherFlickrUploader.Extensions;

namespace YetAnotherFlickrUploader
{
  public interface IProcessor
  {
    void Start(Options options);
  }

  public class Processor : IProcessor
  {
    private readonly IFlickrService _flickrService = new FlickrService();

    public void Start(Options options)
    {
      if (_flickrService.FlickrAuthenticate())
      {
        return;
      }

      if (!Directory.Exists(options.Path))
      {
        Logger.Error("The specified path is invalid.");
        return;
      }

      var subDirectories = Directory.GetDirectories(options.Path);

      if (subDirectories.Length > 0)
      {
        foreach (var subDirectory in subDirectories)
        {
          Process(subDirectory, options);
        }
      }
      else
      {
        Process(options.Path, options);
      }
    }

    private void Process(string path, Options options)
    {
      var files = FileService.FindPictureFiles(path);

      if (!files.Any())
      {
        Logger.Warning("Could not locate any files to upload in the directory: {0}.", path);

        return;
      }


      Logger.Debug("Processing files in the directory: {0}.", path);

      var photosetName = Path.GetFileName(path.TrimEnd('\\'));

      var photoset = Uploader.FindPhotosetByName(photosetName);
      var photosetExists = photoset != null && photoset.Title == photosetName;
      var photosetChanged = false;
      string photosetId = null;

      var photosetPhotos = photosetExists
        ? Uploader.GetPhotosetPictures(photoset.PhotosetId)
        : new List<Photo>();

      if (photosetExists)
      {
        photosetId = photoset.PhotosetId;

        var totalFilesInDirectory = files.Count;
        files.RemoveAll(x => photosetPhotos.Any(p => p.Title == Path.GetFileNameWithoutExtension(x)));
        Logger.Info("{0} out of {1} files are already in the existing photoset.", totalFilesInDirectory - files.Count, totalFilesInDirectory);
      }

      // Check again as the collection might have been modified
      if (!files.Any())
      {
        Logger.Warning("All photos are already in the photoset. Nothing to upload.");
      }
      else
      {

          photosetChanged = true;

          #region Upload photos

          var photoIds = new List<string>();

          Console.WriteLine();
          Logger.Info("Uploading files...");
          //todo: move to FlickrService
          var failures = FlickrService.ParallelExecute(files, fileName =>
          {
            var title = Path.GetFileNameWithoutExtension(fileName);
            // Check if picture is not in the photoset (if it exists)
            var photo = photosetPhotos.FirstOrDefault(x => x.Title == title); //?? Uploader.FindPictureByName(title);
            if (photo == null || photo.Title != title)
            {
              // No such picture found - uploading
              var photoId = Uploader.UploadPicture(fileName, title, null, null);
              if (photoIds.Contains(photoId))
              {
                //uploaded twice?
                throw new Exception($"{title} is already in the list of uploaded files.");
              }

              photoIds.Add(photoId);
            }
          }, Settings.BatchSizeForParallelUpload);

          if (failures.Any())
          {
            Logger.Error("Uploaded with errors:");
            foreach (var failure in failures)
            {
              Logger.Error("{0,-20}: {1}", failure.Key, failure.Value);
            }
          }
          else
          {
            Logger.Info("All files were successfully uploaded.");
          }

          #endregion

          if (!photoIds.Any())
          {
            Logger.Warning("No files were uploaded to '{0}'.", photosetName);
          }
          else if (!photosetExists)
          {
            #region Create new photoset

            Console.WriteLine();
            Logger.Info("Creating photoset '{0}'...", photosetName);

            // Set the first photo in the set as its cover
            var coverPhotoId = photoIds.First();
            photoIds.Remove(coverPhotoId);

            // Create new photoset
            photoset = Uploader.CreatePhotoSet(photosetName, coverPhotoId);
            photosetId = photoset.PhotosetId;

            Logger.Info("Photoset created.");

            photosetExists = true;

            #endregion

            #region Move photos to the photoset

            Console.WriteLine();
            Logger.Info("Moving uploaded files to the photoset...");
            //todo: move to FlickrService
            var fails = FlickrService.ParallelExecute(photoIds, id => Uploader.AddPictureToPhotoSet(id, photosetId),
              Settings.BatchSizeForParallelProcessing);

            if (!fails.Any())
            {
              Logger.Info("Uploaded pictures were successfully moved to '{0}'.", photosetName);
            }
            else
            {
              Logger.Error("Moved with errors:");
              foreach (var fail in fails)
              {
                Logger.Error("{0,-20}: {1}", fail.Key, fail.Value);
              }
            }

            #endregion
          }
        }
      

      bool updatePermissions = options.ShareWithFamily || options.ShareWithFriends;

      if (photosetExists && (photosetChanged || updatePermissions))
      {
        // Get all photos in the photoset
        photosetPhotos = Uploader.GetPhotosetPictures(photosetId);

        FileService.ValidateDirectory(options.Path, photosetPhotos);

        if (photosetPhotos.Count > 1)
        {
          if (photosetChanged)
          {
            FlickrService.SortPhotosInSet(photosetPhotos);
          }

          if (updatePermissions)
          {
            FlickrService.SetPermissions(photosetPhotos, options.ShareWithFamily, options.ShareWithFriends);
          }
        }
      }
    }
  }
}
