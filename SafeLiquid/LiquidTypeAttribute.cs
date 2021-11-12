
// Type: SafeLiquid.LiquidTypeAttribute




using System;

namespace SafeLiquid
{
  [AttributeUsage(AttributeTargets.Class)]
  public class LiquidTypeAttribute : Attribute
  {
    public string[] AllowedMembers { get; private set; }

    public LiquidTypeAttribute(params string[] allowedMembers) => this.AllowedMembers = allowedMembers;
  }
}
