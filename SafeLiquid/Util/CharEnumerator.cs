
// Type: SafeLiquid.Util.CharEnumerator




using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SafeLiquid.Util
{
  internal class CharEnumerator : IEnumerator<char>, IDisposable, IEnumerator
  {
    private readonly string str;
    private int index;

    internal CharEnumerator(string str)
    {
      this.str = str != null ? str : throw new ArgumentException("String must not be null", nameof (str));
      this.index = -1;
    }

    public bool MoveNext()
    {
      if (this.index < this.str.Length - 1)
      {
        ++this.index;
        return true;
      }
      this.index = this.str.Length;
      return false;
    }

    public bool AppendNext(StringBuilder sb)
    {
      if (!this.MoveNext())
        return false;
      sb.Append(this.Current);
      return true;
    }

    public bool HasNext() => this.index < this.str.Length - 1;

    object IEnumerator.Current => (object) this.str[this.index];

    public char Current => this.str[this.index];

    public char Previous => this.str[this.index - 1];

    public char Next => this.str[this.index + 1];

    public int Remaining => this.str.Length != this.index ? this.str.Length - this.index - 1 : 0;

    public int Position => this.index + 1;

    public void Reset() => this.index = -1;

    public void Dispose() => this.Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.index = this.str.Length;
    }
  }
}
