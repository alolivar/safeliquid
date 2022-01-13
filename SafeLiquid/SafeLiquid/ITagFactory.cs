
// Type: SafeLiquid.ITagFactory




namespace SafeLiquid
{
  public interface ITagFactory
  {
    string TagName { get; }

    Tag Create();
  }
}
