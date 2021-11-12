
using System;

namespace SafeLiquid.Exceptions
{
    [Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
    public class SyntaxException : LiquidException
#pragma warning restore CA2229 // Implement serialization constructors
    {
        public SyntaxException(string message, params string[] args)
          : base(string.Format(message, (object[])args))
        {
        }
    }
}
