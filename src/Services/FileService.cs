using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlickrNet;
using YetAnotherFlickrUploader.Helpers;

namespace YetAnotherFlickrUploader.Services
{
  public static class FileService
  {
    public static List<string> FindPictureFiles(string directory)
    {
      return Directory.EnumerateFiles(directory)
        .Where(file => file.ToLower().EndsWith("jpg") 
                    || file.ToLower().EndsWith("jpeg")
                    || file.ToLower().EndsWith("mov"))
        .ToList();
    }

    public static void ValidateDirectory(string directory, IEnumerable<Photo> photosetPhotos)
    {
      var files = FileService.FindPictureFiles(directory);

      var photosetPhotoTitles = photosetPhotos.Select(p => p.Title).ToList();

      // Find files which were not uploaded to the photoset
      var leftFiles = files.Select(Path.GetFileNameWithoutExtension).Where(x => !photosetPhotoTitles.Contains(x)).ToList();
      if (leftFiles.Any())
      {
        Console.WriteLine();
        Logger.Warning("Some files were not uploaded:");
        foreach (var leftFile in leftFiles)
        {
          ConsoleHelper.WriteWarningLine(leftFile);
        }
      }

      // Find duplicates in the photoset
      var duplicates = photosetPhotoTitles
        .GroupBy(x => x)
        .Select(g => new { Title = g.Key, Count = g.Count() })
        .Where(x => x.Count > 1)
        .ToList();

      if (duplicates.Any())
      {
        Console.WriteLine();
        Logger.Warning("Some files have duplicates:");
        foreach (var duplicate in duplicates)
        {
          Logger.Warning("{0,-20} x{1}", duplicate.Title, duplicate.Count);
        }
      }
    }

  }
}
