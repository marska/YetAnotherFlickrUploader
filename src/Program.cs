using System;
using NDesk.Options;
using YetAnotherFlickrUploader.Helpers;

namespace YetAnotherFlickrUploader
{
  public class Program
  {
    static void Main(string[] args)
    {
      var options = new Options();

      var p = new OptionSet
      {
        { "p|path", v => options.Path = v },
        { "a|family", v => options.ShareWithFamily = v != null },
        { "r|friends", v => options.ShareWithFriends = v != null }
      };

      try
      {
        p.Parse(args);

        Processor.Start(options);
      }
      catch (Exception e)
      {
        Logger.Error("Something went wrong... :(", e);
        Environment.Exit(-1);
      }

      Environment.Exit(0);
    }
  }
}
