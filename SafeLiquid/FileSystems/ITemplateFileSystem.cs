
// Type: SafeLiquid.FileSystems.ITemplateFileSystem




namespace SafeLiquid.FileSystems
{
  public interface ITemplateFileSystem : IFileSystem
  {
    Template GetTemplate(Context context, string templateName);
  }
}
