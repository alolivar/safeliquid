using SafeLiquid.FileSystems;
using SafeLiquid.NamingConventions;
using SafeLiquid.Tags;
using SafeLiquid.Tags.Html;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace SafeLiquid
{
    public class Template
    {
        private readonly Dictionary<Type, Func<object, object>> SafeTypeTransformers;
        private readonly Dictionary<Type, Func<object, object>> ValueTypeTransformers;
        private Hash _registers;
        private Hash _assigns;
        private Hash _instanceAssigns;
        private List<Exception> _errors;
        private bool? _isThreadSafe;

        public static readonly INamingConvention NamingConvention  = new RubyNamingConvention();

        public IFileSystem FileSystem { get; set; } = new BlankFileSystem();

        public static SyntaxCompatibility DefaultSyntaxCompatibilityLevel { get; set; } = SyntaxCompatibility.DotLiquid20;

        public static bool DefaultIsThreadSafe { get; set; }

        private Dictionary<string, Tuple<ITagFactory, Type>> Tags { get; set; }

        public Strainer Strainer { get; set; }

        public PreStrainer PreStrainer { get; set; }

        public Tokenizer Tokenizer { get; set; }

        public Context Context { get; set; }

        public Template(PreStrainer preStrainer = null)
        {

            if (preStrainer == null)
            {
                preStrainer = new PreStrainer();
            }

            preStrainer.GlobalFilter(typeof(LiquidDateFilters));
            preStrainer.GlobalFilter(typeof(LiquidArrayFilters));


            Tags = new Dictionary<string, Tuple<ITagFactory, Type>>();
            SafeTypeTransformers = new Dictionary<Type, Func<object, object>>();
            ValueTypeTransformers = new Dictionary<Type, Func<object, object>>();
            this.PreStrainer = preStrainer ?? new PreStrainer();
            this.PreStrainer.GlobalFilter(typeof(StandardFilters));
            Tokenizer = new Tokenizer(this);
            RegisterTag<Assign>("assign");
            RegisterTag<SafeLiquid.Tags.Block>("block");
            RegisterTag<Capture>("capture");
            RegisterTag<Case>("case");
            RegisterTag<Comment>("comment");
            RegisterTag<Cycle>("cycle");
            RegisterTag<Extends>("extends");
            RegisterTag<For>("for");
            RegisterTag<Break>("break");
            RegisterTag<Continue>("continue");
            RegisterTag<If>("if");
            RegisterTag<IfChanged>("ifchanged");
            RegisterTag<Include>("include");
            RegisterTag<Literal>("literal");
            RegisterTag<Unless>("unless");
            RegisterTag<Raw>("raw");
            RegisterTag<TableRow>("tablerow");


            RegisterTag<JsonTag>(JsonTag.JsonTagName);
            RegisterTag<IsIntegerTag>(IsIntegerTag.IsIntTagName);
            RegisterTag<IsStringTag>(IsStringTag.IsStringTagName);
            RegisterTag<IsBooleanTag>(IsBooleanTag.IsBooleanTagName);
            RegisterTag<IsArrayTag>(IsArrayTag.IsArrayTagName);
            RegisterTag<IsTimeSpanTag>(IsTimeSpanTag.IsTimeSpanTagName);
            RegisterTag<MinLengthTag>(MinLengthTag.MinLengthTagName);
            RegisterTag<MaxLengthTag>(MaxLengthTag.MaxLengthTagName);
            RegisterTag<RequiredTag>(RequiredTag.RequiredTagName);
            RegisterTag<OneOfTag>(OneOfTag.OneOfTagName);
            RegisterTag<MinValueTag>(MinValueTag.MinValueTagName);
            RegisterTag<MaxValueTag>(MaxValueTag.MaxValueTagName);
            RegisterTag<ErrorTag>(ErrorTag.ErrorTagName);
            RegisterTag<ParamTag>(ParamTag.ParamTagName);

        }

        public void SetContext(Context context)
        {
            this.Context = context;
            this.Strainer = Strainer.Create(context, PreStrainer);
            Context.Strainer = this.Strainer;
        }

        public void RegisterTag<T>(string name) where T : Tag
        {
            Type tagType = typeof(T);
            Tags[name] = new Tuple<ITagFactory, Type>((ITagFactory)new ActivatorTagFactory(this, tagType, name), tagType);
        }

        public void RegisterTagFactory(ITagFactory tagFactory) => Tags[tagFactory.TagName] = new Tuple<ITagFactory, Type>(tagFactory, (Type)null);

        public Type GetTagType(string name)
        {
            Tuple<ITagFactory, Type> tuple;
            Tags.TryGetValue(name, out tuple);
            return tuple.Item2;
        }

        internal bool IsRawTag(string name)
        {
            Tuple<ITagFactory, Type> tuple;
            Tags.TryGetValue(name, out tuple);
            return typeof(RawBlock).IsAssignableFrom(tuple?.Item2);
        }

        internal Tag CreateTag(string name)
        {
            Tag tag = (Tag)null;
            Tuple<ITagFactory, Type> tuple;
            Tags.TryGetValue(name, out tuple);
            if (tuple != null)
                tag = tuple.Item1.Create();
            return tag;
        }


        public void RegisterSafeType(Type type, string[] allowedMembers) => RegisterSafeType(type, (Func<object, object>)(x => (object)new DropProxy(x, allowedMembers)));

        public void RegisterSafeType(
          Type type,
          string[] allowedMembers,
          Func<object, object> func)
        {
            RegisterSafeType(type, (Func<object, object>)(x => (object)new DropProxy(x, allowedMembers, func)));
        }

        public void RegisterSafeType(Type type, Func<object, object> func) => SafeTypeTransformers[type] = func;

        public void RegisterValueTypeTransformer(Type type, Func<object, object> func) => ValueTypeTransformers[type] = func;

        public Func<object, object> GetValueTypeTransformer(Type type)
        {
            Func<object, object> func;
            if (ValueTypeTransformers.TryGetValue(type, out func))
                return func;
            foreach (Type implementedInterface in type.GetTypeInfo().ImplementedInterfaces)
            {
                if (ValueTypeTransformers.TryGetValue(implementedInterface, out func) || implementedInterface.GetTypeInfo().IsGenericType && ValueTypeTransformers.TryGetValue(implementedInterface.GetGenericTypeDefinition(), out func))
                    return func;
            }
            return (Func<object, object>)null;
        }

        public Func<object, object> GetSafeTypeTransformer(Type type)
        {
            Func<object, object> func;
            if (SafeTypeTransformers.TryGetValue(type, out func))
                return func;
            foreach (Type implementedInterface in type.GetTypeInfo().ImplementedInterfaces)
            {
                if (SafeTypeTransformers.TryGetValue(implementedInterface, out func) || implementedInterface.GetTypeInfo().IsGenericType && SafeTypeTransformers.TryGetValue(implementedInterface.GetGenericTypeDefinition(), out func))
                    return func;
            }
            return (Func<object, object>)null;
        }

        public static Template Parse(string source, PreStrainer preStrainer = null)
        {
            preStrainer = preStrainer ?? new PreStrainer();
            Template template = new Template(preStrainer);
            template.ParseInternal(source);
            return template;
        }

        public Document Root { get; set; }

        public Hash Registers => this._registers = this._registers ?? new Hash();

        public Hash Assigns => this._assigns = this._assigns ?? new Hash();

        public Hash InstanceAssigns => this._instanceAssigns = this._instanceAssigns ?? new Hash();

        public List<Exception> Errors => this._errors = this._errors ?? new List<Exception>();

        public bool IsThreadSafe => this._isThreadSafe ?? Template.DefaultIsThreadSafe;


        public Template ParseInternal(string source)
        {
            source = Literal.FromShortHand(source);
            source = Comment.FromShortHand(source);
            this.Root = new Document(this);
            this.Root.Initialize((string)null, (string)null, Tokenize(source));
            return this;
        }

        public void MakeThreadSafe() => this._isThreadSafe = new bool?(true);

        public string Render(IFormatProvider formatProvider = null)
        {
            formatProvider = formatProvider ?? (IFormatProvider)CultureInfo.CurrentCulture;
            return this.Render(new RenderParameters(formatProvider));
        }

        public string Render(Hash localVariables, IFormatProvider formatProvider = null)
        {
            using (StringWriter stringWriter = new StringWriter(formatProvider ?? (IFormatProvider)CultureInfo.CurrentCulture))
            {
                return this.Render((TextWriter)stringWriter, new RenderParameters(stringWriter.FormatProvider)
                {
                    LocalVariables = localVariables
                });
            }
        }

        public string Render(RenderParameters parameters)
        {
            using (StringWriter stringWriter = new StringWriter(parameters.FormatProvider))
                return this.Render((TextWriter)stringWriter, parameters);
        }

        public string Render(TextWriter writer, RenderParameters parameters)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            this.RenderInternal(writer, parameters);
            return writer.ToString();
        }

        public void Render(Stream stream, RenderParameters parameters)
        {
            StreamWriter streamWriter = (StreamWriter)new Template.StreamWriterWithFormatProvider(stream, parameters.FormatProvider);
            this.RenderInternal((TextWriter)streamWriter, parameters);
            streamWriter.Flush();
        }

        private void RenderInternal(TextWriter result, RenderParameters parameters)
        {
            if (this.Root == null)
                return;
            Context context;
            Hash registers;
            IEnumerable<Type> filters;
            parameters.Evaluate(this, out context, out registers, out filters);

            this.SetContext(context);

            if (!this.IsThreadSafe)
            {
                if (registers != null)
                    this.Registers.Merge((IDictionary<string, object>)registers);
                if (filters != null)
                    context.AddFilters(filters);
            }
            try
            {
                this.Root.Render(context, result);
            }
            finally
            {
                if (!this.IsThreadSafe)
                    this._errors = context.Errors;
            }
        }

        internal List<string> Tokenize(string source)
        {
            var tokenization = Tokenizer.Tokenize(source);
            return tokenization;
        }
        private class StreamWriterWithFormatProvider : StreamWriter
        {
            public StreamWriterWithFormatProvider(Stream stream, IFormatProvider formatProvider)
              : base(stream)
            {
                this.FormatProvider = formatProvider;
            }

            public override IFormatProvider FormatProvider { get; }
        }
    }
}
