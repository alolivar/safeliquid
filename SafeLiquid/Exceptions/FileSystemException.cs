using System;

namespace SafeLiquid.Exceptions
{
    [Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
    public class FileSystemException : LiquidException
#pragma warning restore CA2229 // Implement serialization constructors
    {
        public FileSystemException(string message, params string[] args)
          : base(string.Format(message, (object[])args))
        {
        }
    }
}
