
// Type: SafeLiquid.Util.Range




using System;
using System.Collections.Generic;

namespace SafeLiquid.Util
{
  internal static class Range
  {
    internal static long Succ(long val) => val + 1L;

    internal static int Succ(int val) => val + 1;

    internal static short Succ(short val) => (short) ((int) val + 1);

    internal static sbyte Succ(sbyte val) => (sbyte) ((int) val + 1);

    internal static ulong Succ(ulong val) => val + 1UL;

    internal static uint Succ(uint val) => val + 1U;

    internal static ushort Succ(ushort val) => (ushort) ((uint) val + 1U);

    internal static byte Succ(byte val) => (byte) ((uint) val + 1U);

    internal static char Succ(char val) => (char) ((uint) val + 1U);

    internal static DateTime Succ(DateTime val) => val.AddDays(1.0);

    internal static string Succ(string val)
    {
      int num = -1;
      for (int index = val.Length - 1; index >= 0 && num == -1; --index)
      {
        if (char.IsLetterOrDigit(val[index]))
          num = index;
      }
      return num == val.Length - 1 || num == -1 ? Range.Succ(val, val.Length) : Range.Succ(val, num + 1) + val.Substring(num + 1);
    }

    internal static string Succ(string val, int length)
    {
      char ch = val[length - 1];
      switch (ch)
      {
        case '9':
          return (length > 1 ? Range.Succ(val, length - 1) : "1") + "0";
        case 'Z':
          return (length > 1 ? Range.Succ(val, length - 1) : "A") + "A";
        case 'z':
          return (length > 1 ? Range.Succ(val, length - 1) : "a") + "a";
        default:
          return val.Substring(0, length - 1) + ((char) ((uint) ch + 1U)).ToString();
      }
    }

    public static IEnumerable<T> Inclusive<T>(
      T start,
      T finish,
      Func<T, T> succ,
      Comparison<T> comp)
    {
      for (T value = start; comp(value, finish) <= 0; value = succ(value))
        yield return value;
    }

    internal static int Comp<T>(T a, T b) where T : IComparable<T>
    {
      if ((object) a != null)
        return a.CompareTo(b);
      return (object) b != null ? -1 : 0;
    }

    public static IEnumerable<T> Inclusive<T>(T start, T finish, Func<T, T> succ) where T : IComparable<T> => Range.Inclusive<T>(start, finish, succ, new Comparison<T>(Range.Comp<T>));

    public static IEnumerable<DateTime> Inclusive(
      DateTime start,
      DateTime finish)
    {
      return Range.Inclusive<DateTime>(start, finish, new Func<DateTime, DateTime>(Range.Succ), new Comparison<DateTime>(Range.Comp<DateTime>));
    }

    public static IEnumerable<string> Inclusive(string start, string finish) => Range.Inclusive<string>(start, finish, new Func<string, string>(Range.Succ), new Comparison<string>(Range.Comp<string>));

    public static IEnumerable<int> Inclusive(int start, int finish) => Range.Inclusive<int>(start, finish, new Func<int, int>(Range.Succ), new Comparison<int>(Range.Comp<int>));
  }
}
