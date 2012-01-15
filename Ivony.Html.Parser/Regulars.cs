using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public static class Regulars
  {

    public static readonly string tagNamePattern = @"(?<tagName>[\w:\.]+)";


    public static readonly string normalAttributeValuePattern = @"(?<attrValue>([^'""\s\>][^\s\>]*)?(?=\s|\/\>|\>))";
    public static readonly string sqouteAttributeValuePattern = @"('(?<attrValue>[^']*)')";
    public static readonly string dquoteAttributeValuePattern = @"(""(?<attrValue>[^""]*)"")";

    public static readonly string attributeValuePattern = @"((\s*=(\s*#squote|\s*#dquote|#normal))|(?=\s|\/\>|\>))".Replace( "#squote", sqouteAttributeValuePattern ).Replace( "#dquote", dquoteAttributeValuePattern ).Replace( "#normal", normalAttributeValuePattern );
    public static readonly string attributePattern = @"(?<attribute>(?<attrName>[\w-:]+)#attrValue)\s*".Replace( "#attrValue", attributeValuePattern );

    public static readonly string beginTagPattern = @"<#tagName(\s+(#attribute)*)?\s*(?<selfClosed>/)?>".Replace( "#tagName", tagNamePattern ).Replace( "#attribute", attributePattern );

    public static readonly string endTagPattern = @"</#tagName\s*>".Replace( "#tagName", tagNamePattern );

    public static readonly string commentPattern = @"<!--(?<commentText>(.|\n)*?)-->";

    public static readonly string doctypeDeclarationPattern = @"(<!(?!--)(?<specialText>(.|\n)*?)(?<!--)>)";

    public static readonly string specialTagPattern = @"(<\?(?<specialText>(.|\n)*?)\?>)|(<\%(?<specialText>(.|\n)*?)\%>)|(<\#(?<specialText>(.|\n)*?)\#>)|(<\$(?<specialText>(.|\n)*?)\$>)";

  }
}
