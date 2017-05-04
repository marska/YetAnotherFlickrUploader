using System;
using YetAnotherFlickrUploader.Helpers;

namespace YetAnotherFlickrUploader
{
  public class Program
  {
    private static void Main(string[] args)
    {
      var options = new Options();

      try
      {
        if (CommandLine.Parser.Default.ParseArguments(args, options))
        {
          IProcessor processor = new Processor();
          processor.Start(options);
        }

        Logger.Error("Wrong params. :(");
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
