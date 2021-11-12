
// Type: SafeLiquid.NamingConventions.RubyNamingConvention




using SafeLiquid.Util;
using System;
using System.Text.RegularExpressions;

namespace SafeLiquid.NamingConventions
{
  public class RubyNamingConvention : INamingConvention
  {
    private static readonly Regex _regex1 = R.C("([A-Z]+)([A-Z][a-z])");
    private static readonly Regex _regex2 = R.C("([a-z\\d])([A-Z])");

    public StringComparer StringComparer => StringComparer.OrdinalIgnoreCase;

    public string GetMemberName(string name) => RubyNamingConvention._regex2.Replace(RubyNamingConvention._regex1.Replace(name, "$1_$2"), "$1_$2").ToLowerInvariant();

    public bool OperatorEquals(string testedOperator, string referenceOperator) => this.GetMemberName(testedOperator).Equals(referenceOperator);
  }
}
