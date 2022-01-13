
// Type: SafeLiquid.RenderParameters




using System;
using System.Collections.Generic;

namespace SafeLiquid
{
  public class RenderParameters
  {
    private ErrorsOutputMode _erorsOutputMode = ErrorsOutputMode.Display;
    private int _maxIterations;
    private int _timeout;

    public Context Context { get; set; }

    public Hash LocalVariables { get; set; }

    public IEnumerable<Type> Filters { get; set; }

    public Hash Registers { get; set; }

    [Obsolete("Use ErrorsOutputMode instead")]
    public bool RethrowErrors
    {
      get => this.ErrorsOutputMode == ErrorsOutputMode.Rethrow;
      set => this.ErrorsOutputMode = value ? ErrorsOutputMode.Rethrow : ErrorsOutputMode.Display;
    }

    public ErrorsOutputMode ErrorsOutputMode
    {
      get => this._erorsOutputMode;
      set => this._erorsOutputMode = value;
    }

    public SyntaxCompatibility SyntaxCompatibilityLevel { get; set; }

    public int MaxIterations
    {
      get => this._maxIterations;
      set => this._maxIterations = value;
    }

    public IFormatProvider FormatProvider { get; }

    public RenderParameters(IFormatProvider formatProvider)
    {
      this.FormatProvider = formatProvider ?? throw new ArgumentNullException(nameof (formatProvider));
      this.SyntaxCompatibilityLevel = Template.DefaultSyntaxCompatibilityLevel;
    }

    public int Timeout
    {
      get => this._timeout;
      set => this._timeout = value;
    }

    internal void Evaluate(
      Template template,
      out Context context,
      out Hash registers,
      out IEnumerable<Type> filters)
    {
      if (this.Context != null)
      {
        context = this.Context;
        registers = (Hash) null;
        filters = (IEnumerable<Type>) null;
        context.RestartTimeout();
      }
      else
      {
        List<Hash> environments = new List<Hash>();
        if (this.LocalVariables != null)
          environments.Add(this.LocalVariables);
        if (template.IsThreadSafe)
        {
          context = new Context(template, environments, new Hash(), new Hash(), this.ErrorsOutputMode, this.MaxIterations, this.Timeout, this.FormatProvider)
          {
            SyntaxCompatibilityLevel = this.SyntaxCompatibilityLevel,
            Strainer = template.Strainer
          };
        }
        else
        {
          environments.Add(template.Assigns);
          context = new Context(template, environments, template.InstanceAssigns, template.Registers, this.ErrorsOutputMode, this.MaxIterations, this.Timeout, this.FormatProvider)
          {
            SyntaxCompatibilityLevel = this.SyntaxCompatibilityLevel,
            Strainer = template.Strainer
          };
        }
        registers = this.Registers;
        filters = this.Filters;
      }
    }

    public static RenderParameters FromContext(
      Context context,
      IFormatProvider formatProvider)
    {
      return context != null ? new RenderParameters(formatProvider)
      {
        Context = context
      } : throw new ArgumentNullException(nameof (context));
    }
  }
}
