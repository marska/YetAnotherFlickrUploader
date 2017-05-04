using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FlickrNet;
using YetAnotherFlickrUploader.Extensions;
using YetAnotherFlickrUploader.Helpers;

namespace YetAnotherFlickrUploader.Services
{
  public interface IFlickrService
  {
    bool FlickrAuthenticate();
  }

  public class FlickrService : IFlickrService
  {
		public const string ApiKey = "7f148aeb1785cb0d9d0dac171e201bcc";
		public const string SharedSecret = "4c6d48ffe81b3ac6";

		public static Flickr GetInstance()
		{
			return new Flickr(ApiKey, SharedSecret);
		}

		public static Flickr GetAuthInstance()
		{
		  var f = new Flickr(ApiKey, SharedSecret)
		  {
		    OAuthAccessToken = OAuthToken.Token,
		    OAuthAccessTokenSecret = OAuthToken.TokenSecret
		  };

		  return f;
		}

		public static OAuthAccessToken OAuthToken
		{
			get
			{
				return Properties.Settings.Default.OAuthToken;
			}
			set
			{
				Properties.Settings.Default.OAuthToken = value;
				Properties.Settings.Default.Save();
			}
		}

    public bool FlickrAuthenticate()
    {
      Logger.Debug("Authenticating...");

      if (OAuthToken?.Token == null)
      {
        ConsoleHelper.WriteInfoLine("Requesting access token...");

        var flickr = GetInstance();
        OAuthRequestToken requestToken = flickr.OAuthGetRequestToken("oob");

        var url = flickr.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write);

        Process.Start(url);

        ConsoleHelper.WriteInfo("Verifier: ");
        var verifier = Console.ReadLine();

        OAuthToken = flickr.OAuthGetAccessToken(requestToken, verifier);
      }

      if (OAuthToken == null)
      {
        Logger.Error("Could not authenticate.");
        return true;
      }

      Logger.Info("Authenticated as " + OAuthToken.FullName + ".");

      Uploader.UserId = OAuthToken.UserId;
      Uploader.Flickr = GetAuthInstance();
      return false;
    }

    public static void SortPhotosInSet(List<Photo> photosetPhotos)
    {
      List<Photo> orderedList = photosetPhotos.OrderBy(x => x.DateTaken).ToList();
      DateTime maxDateUploaded = orderedList.Select(x => x.DateUploaded).Last();
      int number = orderedList.Count;

      Console.WriteLine();
      Logger.Info("Setting photo upload dates in the photoset...");

      var fails = ParallelExecute(orderedList, photo =>
      {
        DateTime dateUploaded = maxDateUploaded.AddSeconds(-1 * number--);
        Uploader.SetPhotoUploadDate(photo.PhotoId, dateUploaded);

        ConsoleHelper.RestoreCursorPosition();
        ConsoleHelper.WriteDebug("{0} of {1}.", number, photosetPhotos.Count);
      }, Settings.BatchSizeForParallelProcessing);

      if (!fails.Any())
      {
        Logger.Info("Successfully processed all photos in the photoset.");
      }
      else
      {
        Logger.Error("Processed with errors:");
        foreach (var fail in fails)
        {
          Logger.Error("{0,-20}: {1}", fail.Key, fail.Value);
        }
      }
    }

    public static void SetPermissions(List<Photo> photosetPhotos, bool family, bool friends)
    {
      Console.WriteLine();
      Logger.Info("Setting permissions in the photoset...");

      var fails = ParallelExecute(photosetPhotos,
        photo => Uploader.SetPermissions(photo.PhotoId, false, friends, family),
        Settings.BatchSizeForParallelProcessing);

      if (!fails.Any())
      {
        Logger.Info("Successfully processed all photos in the photoset.");
      }
      else
      {
        Logger.Error("Processed with errors:");
        foreach (var fail in fails)
        {
          Logger.Error("{0,-20}: {1}", fail.Key, fail.Value);
        }
      }
    }

    public static Dictionary<T, string> ParallelExecute<T>(List<T> source, Action<T> action, int batchSize)
    {
      var failures = new ConcurrentDictionary<T, string>();

      ConsoleHelper.WriteDebug("Progress: ");
      ConsoleHelper.SaveCursorPosition();

      int processed = 0,
        total = source.Count;

      ConsoleHelper.WriteDebug("{0} of {1}.", processed, total);

      var locker = new object();

      DateTime start = DateTime.Now;

      while (source.Any())
      {
        // Take no more than {batchSize} files for parallel processing
        var batch = source.Take(batchSize).ToList();

        var tasks = batch
          .Select(item =>
            Task.Factory.StartNew(() =>
            {
              try
              {
                action.Invoke(item);
              }
              catch (Exception e)
              {
                failures.TryAdd(item, e.Message);
              }

              source.Remove(item);

              lock (locker)
              {
                processed += 1;

                DateTime now = DateTime.Now;

                TimeSpan elapsed = now - start;
                long timePerItem = elapsed.Ticks / processed;
                var eta = new TimeSpan((total - processed) * timePerItem);

                ConsoleHelper.RestoreCursorPosition();
                ConsoleHelper.WriteDebug("{0} of {1}. Est. time: {2}. Elapsed: {3}.", processed, total, eta.ToReadableString(), elapsed.ToReadableString());
                ConsoleHelper.WriteDebug("{0,20}", " ");
              }
            }, TaskCreationOptions.LongRunning))
          .ToArray();

        // Wait for batch to be processed
        Task.WaitAll(tasks);

        // Pause for 1 sec after each batch
        //Thread.Sleep(1000);
      }

      ConsoleHelper.SetCursorPosition(0);
      Logger.Debug("Done in {0}.{1,60}", (DateTime.Now - start).ToReadableString(), " ");

      return new Dictionary<T, string>(failures);
    }
  }
}