
// Type: SafeLiquid.Exceptions.BreakInterrupt




namespace SafeLiquid.Exceptions
{
  public class BreakInterrupt : InterruptException
  {
    public BreakInterrupt()
      : base("Misplaced 'break' statement")
    {
    }
  }
}
