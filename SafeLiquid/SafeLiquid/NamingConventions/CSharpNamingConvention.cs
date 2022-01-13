
// Type: SafeLiquid.NamingConventions.CSharpNamingConvention




using System;

namespace SafeLiquid.NamingConventions
{
  public class CSharpNamingConvention : INamingConvention
  {
    public StringComparer StringComparer => StringComparer.Ordinal;

    public string GetMemberName(string name) => name;

    public bool OperatorEquals(string testedOperator, string referenceOperator) => CSharpNamingConvention.UpperFirstLetter(testedOperator).Equals(referenceOperator) || CSharpNamingConvention.LowerFirstLetter(testedOperator).Equals(referenceOperator);

    private static string UpperFirstLetter(string word) => char.ToUpperInvariant(word[0]).ToString() + word.Substring(1);

    private static string LowerFirstLetter(string word) => char.ToLowerInvariant(word[0]).ToString() + word.Substring(1);
  }
}
