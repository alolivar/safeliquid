
// Type: SafeLiquid.Exceptions.LiquidException




using System;

namespace SafeLiquid.Exceptions
{
    [Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
    public abstract class LiquidException : ApplicationException
#pragma warning restore CA2229 // Implement serialization constructors
    {
        protected LiquidException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        protected LiquidException(string message)
          : base(message)
        {
        }

        protected LiquidException()
        {
        }
    }
}
