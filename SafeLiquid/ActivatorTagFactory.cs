
// Type: SafeLiquid.ActivatorTagFactory




using System;

namespace SafeLiquid
{
    public class ActivatorTagFactory : ITagFactory
    {
        private readonly Type _tagType;
        private readonly string _tagName;

        public string TagName => this._tagName;

        public Template Template { get; set; }

        public ActivatorTagFactory(Template template, Type tagType, string tagName)
        {
            this._tagType = tagType;
            this._tagName = tagName;
            this.Template = template;
        }

        public Tag Create() => (Tag)Activator.CreateInstance(this._tagType, Template);
    }
}
