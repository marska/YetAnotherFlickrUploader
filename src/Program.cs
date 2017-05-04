#define DEBUG

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
                else
                {
                    Logger.Error("Wrong params. :(");
                }

                
            }
            catch (Exception e)
            {
                Logger.Error("Something went wrong... :(", e);
#if DEBUG
                Console.ReadLine();

#else
                Environment.Exit(-1);
#endif
            }
#if DEBUG
            Console.ReadLine();

#else
            Environment.Exit(0);
#endif
        }
    }
}
