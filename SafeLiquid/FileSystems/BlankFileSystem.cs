
// Type: SafeLiquid.FileSystems.BlankFileSystem




using SafeLiquid.Exceptions;

namespace SafeLiquid.FileSystems
{
  public class BlankFileSystem : IFileSystem
  {
    public string ReadTemplateFile(Context context, string templateName) => throw new FileSystemException(Liquid.ResourceManager.GetString("BlankFileSystemDoesNotAllowIncludesException"), new string[0]);
  }
}
