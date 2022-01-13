
// Type: SafeLiquid.Exceptions.ContinueInterrupt




namespace SafeLiquid.Exceptions
{
  public class ContinueInterrupt : InterruptException
  {
    public ContinueInterrupt()
      : base("Misplaced 'continue' statement")
    {
    }
  }
}
