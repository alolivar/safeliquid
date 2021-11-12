
using System.Collections.Generic;
using System.IO;

namespace SafeLiquid
{
    public class Tag : IRenderable
    {
        public List<object> NodeList { get; protected set; }

        protected string TagName { get; private set; }

        protected string Markup { get; private set; }

        public Template Template { get; set; }

        public Tag(Template template)
        {
            this.Template = template;
        }

        internal virtual void AssertTagRulesViolation(List<object> rootNodeList)
        {
        }

        public virtual void Initialize(string tagName, string markup, List<string> tokens)
        {
            this.TagName = tagName;
            this.Markup = markup;
            this.Parse(tokens);
        }

        protected virtual void Parse(List<string> tokens)
        {
        }

        public string Name => this.GetType().Name.ToLower();

#pragma warning disable CA2119 // Seal methods that satisfy private interfaces
        public virtual void Render(Context context, TextWriter result)
#pragma warning restore CA2119 // Seal methods that satisfy private interfaces
        {
        }

        internal string Render(Context context)
        {
            using (TextWriter result = (TextWriter)new StringWriter(context.FormatProvider))
            {
                this.Render(context, result);
                return result.ToString();
            }
        }

        internal static IDictionary<string, T> GetRegister<T>(
          Context context,
          string registerName)
        {
            if (!context.Registers.ContainsKey(registerName))
                context.Registers[registerName] = (object)new Dictionary<string, T>();
            return context.Registers.Get<IDictionary<string, T>>(registerName);
        }
    }
}
