
// Type: SafeLiquid.Liquid




using SafeLiquid.Tags;
using SafeLiquid.Tags.Html;
using SafeLiquid.Util;
using SafeLiquid.Properties;
using System.Resources;

namespace SafeLiquid
{
  public static class Liquid
  {
    internal static readonly ResourceManager ResourceManager = new ResourceManager(typeof (Resources));
    public static readonly string FilterSeparator = R.Q("\\|");
    public static readonly string ArgumentSeparator = R.Q(",");
    public static readonly string FilterArgumentSeparator = R.Q(":");
    public static readonly string VariableAttributeSeparator = R.Q(".");
    public static readonly string TagStart = R.Q("\\{\\%");
    public static readonly string TagEnd = R.Q("\\%\\}");
    public static readonly string VariableSignature = R.Q("\\(?[\\w\\-\\.\\[\\]]\\)?");
    public static readonly string VariableSegment = R.Q("[\\w\\-]");
    public static readonly string VariableStart = R.Q("\\{\\{");
    public static readonly string VariableEnd = R.Q("\\}\\}");
    public static readonly string VariableIncompleteEnd = R.Q("\\}\\}?");
    public static readonly string QuotedString = R.Q("\"[^\"]*\"|'[^']*'");
    public static readonly string QuotedFragment = string.Format(R.Q("{0}|(?:[^\\s,\\|'\"]|{0})+"), (object) Liquid.QuotedString);
    public static readonly string QuotedAssignFragment = string.Format(R.Q("{0}|(?:[^\\s\\|'\"]|{0})+"), (object) Liquid.QuotedString);
    public static readonly string TagAttributes = string.Format(R.Q("(\\w+)\\s*\\:\\s*({0})"), (object) Liquid.QuotedFragment);
    public static readonly string AnyStartingTag = R.Q("\\{\\{|\\{\\%");
    public static readonly string PartialTemplateParser = string.Format(R.Q("{0}.*?{1}|{2}.*?{3}"), (object) Liquid.TagStart, (object) Liquid.TagEnd, (object) Liquid.VariableStart, (object) Liquid.VariableIncompleteEnd);
    public static readonly string TemplateParser = string.Format(R.Q("({0}|{1})"), (object) Liquid.PartialTemplateParser, (object) Liquid.AnyStartingTag);
    public static readonly string VariableParser = string.Format(R.Q("\\[[^\\]]+\\]|{0}+\\??"), (object) Liquid.VariableSegment);
    public static readonly string LiteralShorthand = R.Q("^(?:\\{\\{\\{\\s?)(.*?)(?:\\s*\\}\\}\\})$");
    public static readonly string CommentShorthand = R.Q("^(?:\\{\\s?\\#\\s?)(.*?)(?:\\s*\\#\\s?\\})$");
    public static bool UseRubyDateFormat = false;

  }
}
