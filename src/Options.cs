using System;

namespace YetAnotherFlickrUploader
{
  public class Options
  {
    public Options()
    {
      Path = Environment.CurrentDirectory;
    }

    public string Path { get; set; }

    public bool ShareWithFamily { get; set; }

    public bool ShareWithFriends { get; set; }
  }
}
