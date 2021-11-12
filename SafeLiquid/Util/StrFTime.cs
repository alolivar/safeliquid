
// Type: SafeLiquid.Util.StrFTime




using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SafeLiquid.Util
{
  public static class StrFTime
  {
    private const string GROUP_FLAGS = "flags";
    private const string GROUP_WIDTH = "width";
    private const string GROUP_DIRECTIVE = "directive";
    private const string SPECIFIER_REGEX = "%(?<flags>[-_0^:#])*(?<width>[1-9][0-9]*)?(?<directive>[a-zA-Z%])";
    private static readonly Dictionary<string, StrFTime.DateTimeDelegate> Formats = new Dictionary<string, StrFTime.DateTimeDelegate>()
    {
      {
        "a",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("ddd", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "A",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("dddd", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "b",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("MMM", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "B",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("MMMM", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "c",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("ddd MMM dd HH:mm:ss yyyy", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "C",
        (StrFTime.DateTimeDelegate) (dateTime => ((int) Math.Floor(Convert.ToDouble(dateTime.ToString("yyyy", (IFormatProvider) CultureInfo.CurrentCulture)) / 100.0)).ToString((IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "d",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("dd", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "D",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("MM/dd/yy", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "e",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("%d", (IFormatProvider) CultureInfo.CurrentCulture).PadLeft(2, ' '))
      },
      {
        "F",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("yyyy-MM-dd", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "g",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.GetIso8601WeekOfYear("g"))
      },
      {
        "G",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.GetIso8601WeekOfYear("G"))
      },
      {
        "h",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("MMM", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "H",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("HH", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "I",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("hh", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "j",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.DayOfYear.ToString((IFormatProvider) CultureInfo.CurrentCulture).PadLeft(3, '0'))
      },
      {
        "k",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("%H", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "l",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("%h", (IFormatProvider) CultureInfo.CurrentCulture).PadLeft(2, ' '))
      },
      {
        "L",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("fff", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "m",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("MM", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "M",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.Minute.ToString((IFormatProvider) CultureInfo.CurrentCulture).PadLeft(2, '0'))
      },
      {
        "n",
        (StrFTime.DateTimeDelegate) (dateTime => "\n")
      },
      {
        "N",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("ffffff", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "3N",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("fff", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "6N",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("ffffff", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "p",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("tt", (IFormatProvider) CultureInfo.CurrentCulture).ToUpper())
      },
      {
        "P",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("tt", (IFormatProvider) CultureInfo.CurrentCulture).ToLower())
      },
      {
        "r",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("hh:mm:ss tt", (IFormatProvider) CultureInfo.CurrentCulture).ToUpper())
      },
      {
        "R",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("HH:mm", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "s",
        (StrFTime.DateTimeDelegate) (dateTime => ((int) (dateTime - new DateTime(1970, 1, 1)).TotalSeconds).ToString((IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "S",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("ss", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "t",
        (StrFTime.DateTimeDelegate) (dateTime => "\t")
      },
      {
        "T",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("HH:mm:ss", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "u",
        (StrFTime.DateTimeDelegate) (dateTime => (dateTime.DayOfWeek == DayOfWeek.Sunday ? (int) (dateTime.DayOfWeek + 7) : (int) dateTime.DayOfWeek).ToString((IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "U",
        (StrFTime.DateTimeDelegate) (dateTime => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, DayOfWeek.Sunday).ToString((IFormatProvider) CultureInfo.CurrentCulture).PadLeft(2, '0'))
      },
      {
        "v",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("%d-MMM-yyyy", (IFormatProvider) CultureInfo.CurrentCulture).ToUpper().PadLeft(11, ' '))
      },
      {
        "V",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.GetIso8601WeekOfYear("V"))
      },
      {
        "w",
        (StrFTime.DateTimeDelegate) (dateTime => ((int) dateTime.DayOfWeek).ToString((IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "W",
        (StrFTime.DateTimeDelegate) (dateTime => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday).ToString((IFormatProvider) CultureInfo.CurrentCulture).PadLeft(2, '0'))
      },
      {
        "x",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("MM/dd/yy", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "X",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("HH:mm:ss", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "y",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("yy", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "Y",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("yyyy", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "z",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("%K", (IFormatProvider) CultureInfo.CurrentCulture).Replace(":", string.Empty))
      },
      {
        ":z",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("%K", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "Z",
        (StrFTime.DateTimeDelegate) (dateTime => dateTime.ToString("zzz", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "%",
        (StrFTime.DateTimeDelegate) (dateTime => "%")
      }
    };
    private static readonly Dictionary<string, StrFTime.DateTimeOffsetDelegate> OffsetFormats = new Dictionary<string, StrFTime.DateTimeOffsetDelegate>()
    {
      {
        "s",
        (StrFTime.DateTimeOffsetDelegate) (dateTimeOffset => ((long) (dateTimeOffset - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds).ToString((IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "z",
        (StrFTime.DateTimeOffsetDelegate) (dateTimeOffset => dateTimeOffset.ToString("%K", (IFormatProvider) CultureInfo.CurrentCulture).Replace(":", string.Empty))
      },
      {
        ":z",
        (StrFTime.DateTimeOffsetDelegate) (dateTimeOffset => dateTimeOffset.ToString("%K", (IFormatProvider) CultureInfo.CurrentCulture))
      },
      {
        "Z",
        (StrFTime.DateTimeOffsetDelegate) (dateTimeOffset => dateTimeOffset.ToString("zzz", (IFormatProvider) CultureInfo.CurrentCulture))
      }
    };

    public static string ToStrFTime(this DateTime dateTime, string format) => Regex.Replace(format, "%(?<flags>[-_0^:#])*(?<width>[1-9][0-9]*)?(?<directive>[a-zA-Z%])", (MatchEvaluator) (specifier => StrFTime.SpecifierEvaluator(specifier.Groups[0].Value, (IEnumerable<string>) specifier.Groups["flags"].Captures.Cast<Capture>().Select<Capture, string>((Func<Capture, string>) (capture => capture.Value)).ToList<string>(), specifier.Groups["width"].Captures.Cast<Capture>().Select<Capture, int?>((Func<Capture, int?>) (capture => new int?(Convert.ToInt32(capture.Value)))).FirstOrDefault<int?>(), specifier.Groups["directive"].Captures.Cast<Capture>().Select<Capture, string>((Func<Capture, string>) (capture => capture.Value)).FirstOrDefault<string>(), (object) dateTime)));

    public static string ToStrFTime(this DateTimeOffset dateTimeOffset, string format) => Regex.Replace(format, "%(?<flags>[-_0^:#])*(?<width>[1-9][0-9]*)?(?<directive>[a-zA-Z%])", (MatchEvaluator) (specifier => StrFTime.SpecifierEvaluator(specifier.Groups[0].Value, (IEnumerable<string>) specifier.Groups["flags"].Captures.Cast<Capture>().Select<Capture, string>((Func<Capture, string>) (capture => capture.Value)).ToList<string>(), specifier.Groups["width"].Captures.Cast<Capture>().Select<Capture, int?>((Func<Capture, int?>) (capture => new int?(Convert.ToInt32(capture.Value)))).FirstOrDefault<int?>(), specifier.Groups["directive"].Captures.Cast<Capture>().Select<Capture, string>((Func<Capture, string>) (capture => capture.Value)).FirstOrDefault<string>(), (object) dateTimeOffset)));

    private static string SpecifierEvaluator(
      string specifier,
      IEnumerable<string> flags,
      int? width,
      string directive,
      object source)
    {
      directive = StrFTime.PreProcessDirective(directive, flags, width);
      string seed;
      if (StrFTime.OffsetFormats.ContainsKey(directive) && source is DateTimeOffset dateTimeOffset2)
      {
        seed = StrFTime.OffsetFormats[directive](dateTimeOffset2);
      }
      else
      {
        if (!StrFTime.Formats.ContainsKey(directive))
          return specifier;
        seed = StrFTime.Formats[directive](!(source is DateTimeOffset dateTimeOffset3) ? (DateTime) source : dateTimeOffset3.DateTime);
      }
      return flags.ToList<string>().Aggregate<string, string>(seed, (Func<string, string, string>) ((current, flag) => StrFTime.ApplyFlag(flag, width ?? 2, current)));
    }

    private static string PreProcessDirective(
      string directive,
      IEnumerable<string> flags,
      int? width)
    {
      string str = string.Concat(flags);
      if ("z".Equals(directive, StringComparison.Ordinal) && ":".Equals(str, StringComparison.Ordinal))
        return str + directive;
      if ("N".Equals(directive, StringComparison.Ordinal))
      {
        int? nullable = width;
        int num1 = 3;
        if (!(nullable.GetValueOrDefault() == num1 & nullable.HasValue))
        {
          nullable = width;
          int num2 = 6;
          if (!(nullable.GetValueOrDefault() == num2 & nullable.HasValue))
            goto label_6;
        }
        return width.ToString() + directive;
      }
label_6:
      return directive;
    }

    private static string ApplyFlag(string flag, int padwidth, string str)
    {
      if (!(flag == "-"))
      {
        if (!(flag == "_"))
        {
          if (!(flag == "0"))
          {
            if (flag == "^")
              return str.ToUpper();
            if (flag == ":" || flag == "#")
              return str;
            throw new ArgumentException("Invalid flag passed to ApplyFlag", nameof (flag));
          }
          return str.TrimStart('0').PadLeft(padwidth, '0');
        }
        return str.TrimStart('0').PadLeft(padwidth, ' ');
      }
      return str.TrimStart('0');
    }

    private static string GetIso8601WeekOfYear(this DateTime dateTime, string directive)
    {
      switch (CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(dateTime))
      {
        case DayOfWeek.Monday:
        case DayOfWeek.Tuesday:
        case DayOfWeek.Wednesday:
          dateTime = dateTime.AddDays(3.0);
          break;
      }
      if (directive == "G")
        return dateTime.ToString("yyyy", (IFormatProvider) CultureInfo.CurrentCulture);
      if (directive == "g")
        return dateTime.ToString("yy", (IFormatProvider) CultureInfo.CurrentCulture);
      if (directive == "V")
        return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString().PadLeft(2, '0');
      throw new ArgumentException("Invalid directive passed to GetIso8601WeekOfYear", nameof (directive));
    }

    private delegate string DateTimeDelegate(DateTime dateTime);

    private delegate string DateTimeOffsetDelegate(DateTimeOffset dateTimeOffset);
  }
}
