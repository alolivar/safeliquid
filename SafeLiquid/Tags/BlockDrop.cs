
// Type: SafeLiquid.Tags.BlockDrop




using System.IO;

namespace SafeLiquid.Tags
{
  public class BlockDrop : Drop
  {
    private readonly Block _block;
    private readonly TextWriter _result;

    public BlockDrop(Block block, TextWriter result)
    {
      this._block = block;
      this._result = result;
    }

    public void Super() => this._block.CallSuper(this.Context, this._result);
  }
}
