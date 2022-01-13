using System;

namespace SafeLiquid.Exceptions
{
    [Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
    public class ContextException : LiquidException
#pragma warning restore CA2229 // Implement serialization constructors
    {
        public ContextException(string message, params string[] args)
          : base(string.Format(message, (object[])args))
        {
        }

        public ContextException()
        {
        }
    }
}
