
// Type: SafeLiquid.Exceptions.ArgumentException


using System.Resources;
using System;

[assembly:NeutralResourcesLanguage("en-US")]

namespace SafeLiquid.Exceptions
{
  [Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
  public class ArgumentException : LiquidException
#pragma warning restore CA2229 // Implement serialization constructors
  {
    public ArgumentException(string message, params string[] args)
      : base(string.Format(message, (object[]) args))
    {
    }

    public ArgumentException()
    {
    }
  }
}
