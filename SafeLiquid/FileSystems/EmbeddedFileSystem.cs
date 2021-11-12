using SafeLiquid.Exceptions;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SafeLiquid.FileSystems
{
    public class EmbeddedFileSystem : IFileSystem
    {
        protected Assembly Assembly { get; private set; }

        public string Root { get; private set; }

        public EmbeddedFileSystem(Assembly assembly, string root)
        {
            this.Assembly = assembly;
            this.Root = root;
        }

        public string ReadTemplateFile(Context context, string templateName)
        {
            string templatePath = (string)context[templateName];
            Stream manifestResourceStream = this.Assembly.GetManifestResourceStream(this.FullPath(templatePath));
            if (manifestResourceStream == null)
            {
                throw new FileSystemException(Liquid.ResourceManager.GetString("LocalFileSystemTemplateNotFoundException"), new string[1]
        {
          templatePath
        });
            }

            using (StreamReader streamReader = new StreamReader(manifestResourceStream))
                return streamReader.ReadToEnd();
        }

        public string FullPath(string templatePath) => templatePath != null && Regex.IsMatch(templatePath, "^[^.\\/][a-zA-Z0-9_\\/]+$") ? Regex.Replace(Path.Combine(templatePath.Contains("/") ? Path.Combine(this.Root, Path.GetDirectoryName(templatePath)) : this.Root, string.Format("_{0}.liquid", (object)Path.GetFileName(templatePath))), "\\\\|/", ".") : throw new FileSystemException(Liquid.ResourceManager.GetString("LocalFileSystemIllegalTemplateNameException"), new string[1]
        {
      templatePath
        });
    }
}
