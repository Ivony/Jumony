using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  
  /// <summary>
  /// 用于分析 HTML DOM 结构的正则表达式列表
  /// </summary>
  public static class Regulars
  {

    /// <summary>用于匹配 HTML 元素标签名的正则表达式</summary>
    public static readonly string tagNamePattern = @"(?<tagName>[\w:-\.]+)";


    /// <summary>用于匹配一般属性值的正则表达式</summary>
    public static readonly string normalAttributeValuePattern = @"(?<attrValue>([^'""\s\>][^\s\>]*)?(?=\s|\/\>|\>))";
    /// <summary>用于匹配用双引号包裹的属性值的正则表达式</summary>
    public static readonly string sqouteAttributeValuePattern = @"('(?<attrValue>[^']*)')";
    /// <summary>用于匹配用单引号包裹的属性值的正则表达式</summary>
    public static readonly string dquoteAttributeValuePattern = @"(""(?<attrValue>[^""]*)"")";

    /// <summary>用于匹配用属性值的正则表达式</summary>
    public static readonly string attributeValuePattern = @"((\s*=(\s*#squote|\s*#dquote|#normal))|(?=\s|\/\>|\>))".Replace( "#squote", sqouteAttributeValuePattern ).Replace( "#dquote", dquoteAttributeValuePattern ).Replace( "#normal", normalAttributeValuePattern );
    /// <summary>用于匹配用属性表达式的的正则表达式</summary>
    public static readonly string attributePattern = @"(?<attribute>(?<attrName>[\w-:]+)#attrValue)\s*".Replace( "#attrValue", attributeValuePattern );

    /// <summary>用于匹配用开始标签的正则表达式</summary>
    public static readonly string beginTagPattern = @"<#tagName(\s+(#attribute)*)?\s*(?<selfClosed>/)?>".Replace( "#tagName", tagNamePattern ).Replace( "#attribute", attributePattern );

    /// <summary>用于匹配用结束标签的正则表达式</summary>
    public static readonly string endTagPattern = @"</#tagName\s*>".Replace( "#tagName", tagNamePattern );

    /// <summary>用于匹配用注释标签的正则表达式</summary>
    public static readonly string commentPattern = @"<!--(?<commentText>(.|\n)*?)-->";

    /// <summary>用于匹配用声明标签的正则表达式</summary>
    public static readonly string doctypeDeclarationPattern = @"(<!(?!--)(?<specialText>(.|\n)*?)(?<!--)>)";

    /// <summary>用于匹配用特殊标签的正则表达式</summary>
    public static readonly string specialTagPattern = @"(<\?(?<specialText>(.|\n)*?)\?>)|(<\%(?<specialText>(.|\n)*?)\%>)|(<\#(?<specialText>(.|\n)*?)\#>)|(<\$(?<specialText>(.|\n)*?)\$>)";

  }
}
