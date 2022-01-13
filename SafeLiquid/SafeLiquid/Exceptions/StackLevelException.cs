using System;

namespace SafeLiquid.Exceptions
{
    [Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
    public class StackLevelException : LiquidException
#pragma warning restore CA2229 // Implement serialization constructors
    {
        public StackLevelException(string message)
          : base(string.Format(message))
        {
        }
    }
}