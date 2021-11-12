using SafeLiquid.Util;
using System.IO;
using System.Text.RegularExpressions;

namespace SafeLiquid.Tags
{
    public class Comment : RawBlock
    {
        private static readonly Regex ShortHandRegex = R.C(Liquid.CommentShorthand);

        public static string FromShortHand(string theString)
        {
            if (theString == null)
                return theString;
            Match match = Comment.ShortHandRegex.Match(theString);
            return !match.Success ? theString : string.Format("{{% comment %}}{0}{{% endcomment %}}", (object)match.Groups[1].Value);
        }

        public Comment(Template template) : base(template) { }

        public override void Render(Context context, TextWriter result)
        {
        }
    }
}
