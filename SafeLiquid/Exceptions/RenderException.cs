using System;

namespace SafeLiquid.Exceptions
{
    [Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
    public abstract class RenderException : ApplicationException
#pragma warning restore CA2229 // Implement serialization constructors
    {
        protected RenderException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        protected RenderException(string message)
          : base(message)
        {
        }

        protected RenderException()
        {
        }
    }
}
