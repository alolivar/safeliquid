using System;

namespace SafeLiquid.Exceptions
{
    [Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
    public class FilterNotFoundException : LiquidException
#pragma warning restore CA2229 // Implement serialization constructors
    {
        public FilterNotFoundException(string message, FilterNotFoundException innerException)
          : base(message, (Exception)innerException)
        {
        }

        public FilterNotFoundException(string message, params string[] args)
          : base(string.Format(message, (object[])args))
        {
        }

        public FilterNotFoundException(string message)
          : base(message)
        {
        }
    }
}
