
// Type: SafeLiquid.NamingConventions.INamingConvention




using System;

namespace SafeLiquid.NamingConventions
{
  public interface INamingConvention
  {
    StringComparer StringComparer { get; }

    string GetMemberName(string name);

    bool OperatorEquals(string testedOperator, string referenceOperator);
  }
}
