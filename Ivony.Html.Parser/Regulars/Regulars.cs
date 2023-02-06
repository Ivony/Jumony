using System.Text.RegularExpressions;

namespace Ivony.Html.Parser
{
    public static class Regulars
    {
        /// <summary>用于匹配 HTML 元素标签名的正则表达式</summary>
        private static readonly string tagNamePattern = @"(?<tagName>[A-Za-z][A-Za-z0-9\-_:\.]*)";

        /// <summary>用于匹配一般属性值的正则表达式</summary>
        private static readonly string normalAttributeValuePattern = @"(?<attrValue>([^'""\s][^\s]*)?(?=\s|$))";
        /// <summary>用于匹配用双引号包裹的属性值的正则表达式</summary>
        private static readonly string sqouteAttributeValuePattern = @"('(?<attrValue>[^']*)')";
        /// <summary>用于匹配用单引号包裹的属性值的正则表达式</summary>
        private static readonly string dquoteAttributeValuePattern = @"(""(?<attrValue>[^""]*)"")";

        /// <summary>用于匹配用属性值的正则表达式</summary>
        private static readonly string attributeValuePattern = @"((\s*=\s*(#squote|#dquote|#normal))|(?=\s|$))".Replace( "#squote", sqouteAttributeValuePattern ).Replace( "#dquote", dquoteAttributeValuePattern ).Replace( "#normal", normalAttributeValuePattern );

        /// <summary>用于匹配用属性名称的的正则表达式</summary>
        private static readonly string attributeNamePattern = @"(?<attrName>[\w:_\-]+)";

        /// <summary>用于匹配用属性表达式的的正则表达式</summary>
        private static readonly string attributePattern = @"(\G|\s)(?<attribute>#attrName#attrValue)".Replace( "#attrName", attributeNamePattern ).Replace( "#attrValue", attributeValuePattern );

        /// <summary>用于匹配用开始标签的正则表达式</summary>
        private static readonly string beginTagPattern = @"<#tagName(?<attributes>([^=]|(?>=\w*'[^']*')|(?>=\w*""[^""]*"")|=)*?)(?<selfClosed>/)?>".Replace( "#tagName", tagNamePattern );

        /// <summary>用于匹配用结束标签的正则表达式</summary>
        private static readonly string endTagPattern = @"</(#tagName)?[^>]*>".Replace( "#tagName", tagNamePattern );

        /// <summary>用于匹配用注释标签的正则表达式</summary>
        private static readonly string commentPattern = @"<!--(?<commentText>(.|\n)*?)-->";

        /// <summary>用于匹配用声明标签的正则表达式</summary>
        private static readonly string doctypeDeclarationPattern = @"(<!(?i:DOCTYPE)\s+(?<declaration>.*?)>)";

        /// <summary>用于匹配用特殊标签的正则表达式</summary>
        private static readonly string specialTagPattern = @"<(?<symbol>[\?\%\#\$])(?<specialText>.*?)\k<symbol>>";

        public class TagName : Regex
        {
            public TagName() : base("^" + Regulars.tagNamePattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture)
            {
            }
        }

        public class AttributeName:Regex
        {
            public AttributeName() : base("^"+Regulars.attributeNamePattern+"$", RegexOptions.Compiled | RegexOptions.CultureInvariant |  RegexOptions.Singleline |RegexOptions.ExplicitCapture)
            {
            }
        }

        public class Attribute:Regex
        {
            public Attribute() : base(Regulars.attributePattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture)
            {

            }
        }

        public class BeginTag :Regex
        {
            public BeginTag() : base(@"\G" + Regulars.beginTagPattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture)
            {

            }
        }

        public class EndTag : Regex
        {
            public EndTag() : base(@"\G" + Regulars.endTagPattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture)
            {

            }
        }

        public class DoctypeDeclaration:Regex
        {
            public DoctypeDeclaration() : base(@"\G" + Regulars.doctypeDeclarationPattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture)
            {

            }
        }

        public class CommentTag : Regex
        {
            public CommentTag() : base(@"\G" + Regulars.commentPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture)
            {

            }
        }

        public class SpecialTag : Regex
        {
            public SpecialTag() : base(@"\G" + Regulars.specialTagPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture)
            {

            }
        }

        public class HtmlTag : Regex
        {
            public HtmlTag() : base(@"\G(?<tag>\<.+?\>)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture)
            {

            }
        }
    }
}
