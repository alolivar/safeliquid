
// Type: SafeLiquid.Exceptions.MaximumIterationsExceededException




namespace SafeLiquid.Exceptions
{
  internal class MaximumIterationsExceededException : RenderException
  {
    public MaximumIterationsExceededException(string message, params string[] args)
      : base(string.Format(message, (object[]) args))
    {
    }

    public MaximumIterationsExceededException()
    {
    }
  }
}
