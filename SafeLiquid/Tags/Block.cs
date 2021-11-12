
// Type: SafeLiquid.Tags.Block




using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SafeLiquid.Tags
{
    public class Block : SafeLiquid.Block
    {
        private static readonly Regex Syntax = R.C("(\\w+)");

        internal string BlockName { get; set; }

        public Block(Template template) : base(template) { }

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match match = Block.Syntax.Match(markup);
            if (!match.Success)
                throw new SyntaxException(Liquid.ResourceManager.GetString("BlockTagSyntaxException"), new string[0]);
            this.BlockName = match.Groups[1].Value;
            if (tokens == null)
                return;
            base.Initialize(tagName, markup, tokens);
        }

        internal override void AssertTagRulesViolation(List<object> rootNodeList) => rootNodeList.ForEach((Action<object>)(n =>
       {
           Block b1 = n as Block;
           if (b1 == null)
               return;
           List<object> all = rootNodeList.FindAll((Predicate<object>)(o => o is Block block2 && b1.BlockName == block2.BlockName));
           if (all != null && all.Count > 1)
           {
               throw new SyntaxException(Liquid.ResourceManager.GetString("BlockTagAlreadyDefinedException"), new string[1]
           {
          b1.BlockName
           });
           }
       }));

        public override void Render(Context context, TextWriter result)
        {
            BlockRenderState blockState = BlockRenderState.Find(context);
            context.Stack((Action)(() =>
           {
               context["block"] = (object)new BlockDrop(this, result);
               this.RenderAll(this.GetNodeList(blockState), context, result);
           }));
        }

        internal List<object> GetNodeList(BlockRenderState blockState) => blockState != null ? blockState.GetNodeList(this) : this.NodeList;

        public void AddParent(Dictionary<Block, Block> parents, List<object> nodeList)
        {
            Block block1;
            if (parents.TryGetValue(this, out block1))
            {
                block1.AddParent(parents, nodeList);
            }
            else
            {
                Block block2 = new Block(Template);
                block2.Initialize(this.TagName, this.BlockName, (List<string>)null);
                block2.NodeList = new List<object>((IEnumerable<object>)nodeList);
                parents[this] = block2;
            }
        }

        public void CallSuper(Context context, TextWriter result)
        {
            BlockRenderState blockRenderState = BlockRenderState.Find(context);
            Block block;
            if (blockRenderState == null || !blockRenderState.Parents.TryGetValue(this, out block) || block == null)
                return;
            block.Render(context, result);
        }
    }
}
