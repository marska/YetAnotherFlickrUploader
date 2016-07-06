using System;

namespace YetAnotherFlickrUploader.Extensions
{
  public static class TimeSpanExtension
  {
    public static string ToReadableString(this TimeSpan value)
    {
      //return new DateTime(value.Ticks).ToString("hh:mm:ss");

      string formatted;
      if (value.TotalDays > 1)
      {
        formatted = string.Format("{0}{1}",
          string.Format("{0:0} day{1}, ", value.Days, value.Days == 1 ? String.Empty : "s"),
          value.TotalHours > 0 ? string.Format("{0:0} hour{1}, ", value.Hours, value.Hours == 1 ? String.Empty : "s") : string.Empty);
      }
      else if (value.TotalHours > 1)
      {
        formatted = string.Format("{0}{1}",
          string.Format("{0:0} hour{1}, ", value.Hours, value.Hours == 1 ? String.Empty : "s"),
          value.TotalMinutes > 0 ? string.Format("{0:0} minute{1}, ", value.Minutes, value.Minutes == 1 ? String.Empty : "s") : string.Empty);
      }
      else if (value.TotalMinutes > 1)
      {
        formatted = string.Format("{0}{1}",
          string.Format("{0:0} minute{1}, ", value.Minutes, value.Minutes == 1 ? String.Empty : "s"),
          value.TotalSeconds > 0 ? string.Format("{0:0} second{1}", value.Seconds, value.Seconds == 1 ? String.Empty : "s") : string.Empty);
      }
      else if (value.TotalSeconds > 1)
      {
        formatted = string.Format("{0:0} second{1}", value.Seconds, value.Seconds == 1 ? String.Empty : "s");
      }
      else
      {
        formatted = "0 seconds";
      }

      if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

      return formatted;
    }
  }
}

