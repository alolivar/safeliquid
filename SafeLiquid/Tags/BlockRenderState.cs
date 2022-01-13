
// Type: SafeLiquid.Tags.BlockRenderState




using System.Collections.Generic;

namespace SafeLiquid.Tags
{
  internal class BlockRenderState
  {
    public Dictionary<Block, Block> Parents { get; private set; }

    public Dictionary<Block, List<object>> NodeLists { get; private set; }

    public BlockRenderState()
    {
      this.Parents = new Dictionary<Block, Block>();
      this.NodeLists = new Dictionary<Block, List<object>>();
    }

    public List<object> GetNodeList(Block block)
    {
      List<object> nodeList;
      if (!this.NodeLists.TryGetValue(block, out nodeList))
        nodeList = block.NodeList;
      return nodeList;
    }

    public static BlockRenderState Find(Context context)
    {
      foreach (Hash scope in context.Scopes)
      {
        object obj;
        if (scope.TryGetValue("blockstate", out obj))
          return obj as BlockRenderState;
      }
      return (BlockRenderState) null;
    }
  }
}
