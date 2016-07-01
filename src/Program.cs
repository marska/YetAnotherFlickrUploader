using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetAnotherFlickrUploader.Helpers;
using YetAnotherFlickrUploader.Services;
using FlickrNet;

namespace YetAnotherFlickrUploader
{
	class Program
	{
		static ModesEnum _mode;

		static void Main(string[] args)
		{
			#region Parse args

			string path = null;
			string modeSwitch = null;

			if (args != null)
			{
				int i = 0;
				while (i < args.Length)
				{
					string arg = args[i++];
					if (arg.StartsWith("--"))
					{
						modeSwitch = arg;
					}
					else
					{
						path = arg;
					}
				}
			}

			if (string.IsNullOrEmpty(path))
			{
				path = Environment.CurrentDirectory;
			}

			_mode = Options.GetModeFromArgs(modeSwitch);

			#endregion

			#region Authenticate with Flickr API

			Logger.Debug("Authenticating...");

			var token = Authenticate();

			if (token == null)
			{
				Logger.Error("Could not authenticate.");
				return;
			}

			Logger.Info("Authenticated as " + token.FullName + ".");

			Uploader.UserId = token.UserId;
			Uploader.Flickr = FlickrManager.GetAuthInstance();

			#endregion

			try
			{
        Processor.Start(path, _mode);
			}
			catch (Exception e)
			{
				Console.WriteLine();
				Logger.Error("Upload failed.", e);
			}
		}

		static OAuthAccessToken Authenticate()
		{
			OAuthAccessToken token = FlickrManager.OAuthToken;

			if (token == null || token.Token == null)
			{
				ConsoleHelper.WriteInfoLine("Requesting access token...");

				Flickr flickr = FlickrManager.GetInstance();
				OAuthRequestToken requestToken = flickr.OAuthGetRequestToken("oob");

				string url = flickr.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write);

				Process.Start(url);

				ConsoleHelper.WriteInfo("Verifier: ");
				string verifier = Console.ReadLine();

				token = flickr.OAuthGetAccessToken(requestToken, verifier);
				FlickrManager.OAuthToken = token;
			}

			return token;
		}
	}
}
