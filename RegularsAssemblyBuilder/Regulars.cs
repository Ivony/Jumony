using System;
using System.Collections.Generic;
using System.Text;

namespace RegularsAssemblyBuilder
{

  /// <summary>
  /// 用于分析 HTML DOM 结构的正则表达式列表
  /// </summary>
  public static class Regulars
  {

    /// <summary>用于匹配 HTML 元素标签名的正则表达式</summary>
    public static readonly string tagNamePattern = @"(?<tagName>(?!\d)[\w:_\-\.]+)";


    /// <summary>用于匹配一般属性值的正则表达式</summary>
    public static readonly string normalAttributeValuePattern = @"(?<attrValue>([^'""\s][^\s]*)?(?=\s|$))";
    /// <summary>用于匹配用双引号包裹的属性值的正则表达式</summary>
    public static readonly string sqouteAttributeValuePattern = @"('(?<attrValue>[^']*)')";
    /// <summary>用于匹配用单引号包裹的属性值的正则表达式</summary>
    public static readonly string dquoteAttributeValuePattern = @"(""(?<attrValue>[^""]*)"")";

    /// <summary>用于匹配用属性值的正则表达式</summary>
    public static readonly string attributeValuePattern = @"((\s*=\s*(#squote|#dquote|#normal))|(?=\s|$))".Replace( "#squote", sqouteAttributeValuePattern ).Replace( "#dquote", dquoteAttributeValuePattern ).Replace( "#normal", normalAttributeValuePattern );

    /// <summary>用于匹配用属性名称的的正则表达式</summary>
    public static readonly string attributeNamePattern = @"(?<attrName>[\w:_\-]+)";

    /// <summary>用于匹配用属性表达式的的正则表达式</summary>
    public static readonly string attributePattern = @"(\G|\s)(?<attribute>#attrName#attrValue)".Replace( "#attrName", attributeNamePattern ).Replace( "#attrValue", attributeValuePattern );

    /// <summary>用于匹配用开始标签的正则表达式</summary>
    public static readonly string beginTagPattern = @"<#tagName(?<attributes>([^'""]|(?>'[^']*')|(?>""[^""]*""))*?)(?<selfClosed>/)?>".Replace( "#tagName", tagNamePattern ).Replace( "#attribute", attributePattern );

    /// <summary>用于匹配用结束标签的正则表达式</summary>
    public static readonly string endTagPattern = @"</#tagName\s*>".Replace( "#tagName", tagNamePattern );

    /// <summary>用于匹配用注释标签的正则表达式</summary>
    public static readonly string commentPattern = @"<!--(?<commentText>(.|\n)*?)-->";

    /// <summary>用于匹配用声明标签的正则表达式</summary>
    public static readonly string doctypeDeclarationPattern = @"(<!(?i:DOCTYPE)\s+(?<declaration>(.|\n)*?)>)";

    /// <summary>用于匹配用特殊标签的正则表达式</summary>
    public static readonly string specialTagPattern = @"(<\?(?<specialText>(.|\n)*?)\?>)|(<\%(?<specialText>(.|\n)*?)\%>)|(<\#(?<specialText>(.|\n)*?)\#>)|(<\$(?<specialText>(.|\n)*?)\$>)";

    /// <summary>用于匹配用任意标签的正则表达式</summary>
    public static readonly string tagPattern = string.Format( @"(?<beginTag>{0})|(?<endTag>{1})|(?<comment>{2})|(?<special>{3})|(?<doctype>{4})", Regulars.beginTagPattern, Regulars.endTagPattern, Regulars.commentPattern, Regulars.specialTagPattern, Regulars.doctypeDeclarationPattern );

  }
}
