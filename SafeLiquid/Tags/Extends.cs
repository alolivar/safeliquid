using SafeLiquid.Exceptions;
using SafeLiquid.FileSystems;
using SafeLiquid.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SafeLiquid.Tags
{
    public class Extends : SafeLiquid.Block
    {
        private static readonly Regex Syntax = R.B("^({0})", Liquid.QuotedFragment);
        private string _templateName;

        public Extends(Template template) : base(template) { }

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match match = Extends.Syntax.Match(markup);
            if (!match.Success)
                throw new SyntaxException(Liquid.ResourceManager.GetString("ExtendsTagSyntaxException"), new string[0]);
            this._templateName = match.Groups[1].Value;
            base.Initialize(tagName, markup, tokens);
        }

        internal override void AssertTagRulesViolation(List<object> rootNodeList)
        {
            if (!(rootNodeList[0] is Extends))
                throw new SyntaxException(Liquid.ResourceManager.GetString("ExtendsTagMustBeFirstTagException"), new string[0]);
            this.NodeList.ForEach((Action<object>)(n =>
           {
               if ((!(n is string) || !((string)n).IsNullOrWhiteSpace()) && !(n is Block) && !(n is Comment) && !(n is Extends))
                   throw new SyntaxException(Liquid.ResourceManager.GetString("ExtendsTagUnallowedTagsException"), new string[0]);
           }));
            if (this.NodeList.Count<object>((Func<object, bool>)(o => o is Extends)) > 0)
                throw new SyntaxException(Liquid.ResourceManager.GetString("ExtendsTagCanBeUsedOneException"), new string[0]);
        }

        protected override void AssertMissingDelimitation()
        {
        }

        public override void Render(Context context, TextWriter result)
        {
            if (!(context.Registers["file_system"] is IFileSystem fileSystem1))
                fileSystem1 = Template.FileSystem;
            IFileSystem fileSystem2 = fileSystem1;
            ITemplateFileSystem templateFileSystem = fileSystem2 as ITemplateFileSystem;
            Template template = (Template)null;
            if (templateFileSystem != null)
                template = templateFileSystem.GetTemplate(context, this._templateName);
            if (template == null)
                template = Template.Parse(fileSystem2.ReadTemplateFile(context, this._templateName), Template.PreStrainer);
            List<Block> parentBlocks = this.FindBlocks((object)template.Root, (List<Block>)null);
            List<Block> orphanedBlocks = (List<Block>)context.Scopes[0]["extends"] ?? new List<Block>();
            BlockRenderState blockState = BlockRenderState.Find(context) ?? new BlockRenderState();
            context.Stack((Action)(() =>
           {
               context["blockstate"] = (object)blockState;
               context["extends"] = (object)new List<Block>();
               foreach (Block block1 in this.NodeList.OfType<Block>().Concat<Block>((IEnumerable<Block>)orphanedBlocks))
               {
                   Block block = block1;
                   Block key = parentBlocks.Find((Predicate<Block>)(b => b.BlockName == block.BlockName));
                   if (key != null)
                   {
                       Block block2;
                       if (blockState.Parents.TryGetValue(block, out block2))
                           blockState.Parents[key] = block2;
                       key.AddParent(blockState.Parents, key.GetNodeList(blockState));
                       blockState.NodeLists[key] = block.GetNodeList(blockState);
                   }
                   else if (this.IsExtending(template))
                   {
                       ((List<Block>)context.Scopes[0]["extends"]).Add(block);
                   }
               }
               template.Render(result, RenderParameters.FromContext(context, result.FormatProvider));
           }));
        }

        public bool IsExtending(Template template) => template.Root.NodeList.Any<object>((Func<object, bool>)(node => node is Extends));

        private List<Block> FindBlocks(object node, List<Block> blocks)
        {
            if (blocks == null)
                blocks = new List<Block>();
            if (node.RespondTo("NodeList"))
            {
                ((List<object>)node.Send("NodeList"))?.ForEach((Action<object>)(n =>
      {
          Block block = n as Block;
          if (block != null && blocks.All<Block>((Func<Block, bool>)(bl => bl.BlockName != block.BlockName)))
              blocks.Add(block);
          this.FindBlocks(n, blocks);
      }));
            }

            return blocks;
        }
    }
}
