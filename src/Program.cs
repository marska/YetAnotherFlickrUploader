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
	public class Program
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

		  if (Processor.FlickrAuthenticate())
		  {
		    return;
		  }

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
	}
}
