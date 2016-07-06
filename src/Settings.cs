using System;
using System.Configuration;

namespace YetAnotherFlickrUploader
{
  public class Settings
  {
    public static readonly int BatchSizeForParallelUpload = Convert.ToInt32(ConfigurationManager.AppSettings["BatchSizeForParallelUpload"]);

    public static readonly int BatchSizeForParallelProcessing = Convert.ToInt32(ConfigurationManager.AppSettings["BatchSizeForParallelProcessing"]);
  }
}
