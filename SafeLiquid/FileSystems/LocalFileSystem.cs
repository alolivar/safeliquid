
// Type: SafeLiquid.FileSystems.LocalFileSystem




using SafeLiquid.Exceptions;
using System.IO;
using System.Text.RegularExpressions;

namespace SafeLiquid.FileSystems
{
    public class LocalFileSystem : IFileSystem
    {
        public string Root { get; set; }

        public LocalFileSystem(string root) => this.Root = root;

        public string ReadTemplateFile(Context context, string templateName)
        {
            string templatePath = (string)context[templateName];
            string path = this.FullPath(templatePath);
            return File.Exists(path) ? File.ReadAllText(path) : throw new FileSystemException(Liquid.ResourceManager.GetString("LocalFileSystemTemplateNotFoundException"), new string[1]
            {
        templatePath
            });
        }

        public string FullPath(string templatePath)
        {
            if (templatePath == null || !Regex.IsMatch(templatePath, "^[^.\\/][a-zA-Z0-9_\\/]+$"))
            {
                throw new FileSystemException(Liquid.ResourceManager.GetString("LocalFileSystemIllegalTemplateNameException"), new string[1]
                {
          templatePath
                });
            }

            string path = templatePath.Contains("/") ? Path.Combine(Path.Combine(this.Root, Path.GetDirectoryName(templatePath)), string.Format("_{0}.liquid", (object)Path.GetFileName(templatePath))) : Path.Combine(this.Root, string.Format("_{0}.liquid", (object)templatePath));
            string str = Regex.Escape(this.Root);
            return Regex.IsMatch(Path.GetFullPath(path), string.Format("^{0}", (object)str)) ? path : throw new FileSystemException(Liquid.ResourceManager.GetString("LocalFileSystemIllegalTemplatePathException"), new string[1]
            {
        Path.GetFullPath(path)
            });
        }
    }
}
