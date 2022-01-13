
using System;

namespace SafeLiquid.Exceptions
{
    [Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
    public class VariableNotFoundException : LiquidException
#pragma warning restore CA2229 // Implement serialization constructors
    {
        public VariableNotFoundException(string message, params string[] args)
          : base(string.Format(message, (object[])args))
        {
        }

        public VariableNotFoundException(string message)
          : base(message)
        {
        }
    }
}