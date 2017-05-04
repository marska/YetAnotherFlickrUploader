using CommandLine;
using CommandLine.Text;
using System;

namespace YetAnotherFlickrUploader
{
  public class Options
  {
    public Options()
    {
      Path = Environment.CurrentDirectory;
    }

    [Option('p', "path", Required = false, HelpText = "Input directory path to be processed.")]
    public string Path { get; set; }

    [Option('a', "family", DefaultValue = false, Required = false, HelpText = "Share photoset with family.")]
    public bool ShareWithFamily { get; set; }

    [Option('r', "friends", DefaultValue = false, Required = false, HelpText = "Share photoset with friends.")]
    public bool ShareWithFriends { get; set; }

    [ParserState]
    public IParserState LastParserState { get; set; }

    [HelpOption]
    public string GetUsage()
    {
      return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
    }
  }
}
