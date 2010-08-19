using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Web.Html
{
  public class HtmlCssSelector
  {

    public const string dquoteTextPattern = @"(""(?<quoteText>(\\.|[^""\\])*)"")";
    public const string squoteTextPattern = @"('(?<quoteText>(\\.|[^'\\])*)')";
    public const string quoteTextPattern = "(" + dquoteTextPattern + "|" + squoteTextPattern + ")";

    public const string relativeExpressionPattern = @"(?<relative>(\p{Zs}+~\p{Zs}+)|(\p{Zs}+\+\p{Zs}+)|(\p{Zs}+\>\p{Zs}+)|\p{Zs}+)";
    public const string relativeExpressionPatternNoGroup = @"((\p{Zs}+~\p{Zs}+)|(\p{Zs}+\+\p{Zs}+)|(\p{Zs}+\>\p{Zs}+)|\p{Zs}+)";

    public static readonly string attributeExpressionPattern = string.Format( @"\[(?<name>\w+)((?<separator>(\|=)|(\*=)|(\~=)|(\$=)|(\!=)|(\^=)|=)(?<value>{0}|[^]]*))?\]", quoteTextPattern );
    public static readonly string attributeExpressionPatternNoGroup = string.Format( @"\[\w+(((\|=)|(\*=)|(\~=)|(\$=)|(\!=)|(\^=)|=)({0}|[^]]*))?\]", quoteTextPattern );

    public static readonly string elementExpressionPattern = string.Format( @"(?<elementSelector>(?<name>\w+)?((#(?<identity>\w+))|(\.(?<class>\w+)))?(?<attributeSelector>{0})*)", attributeExpressionPatternNoGroup );
    public static readonly string elementExpressionPatternNoGroup = string.Format( @"((\w+)?((#(\w+))|(\.(\w+)))?({0})*)", attributeExpressionPatternNoGroup );

    public static readonly string extraExpressionPattern =string.Format( "{0}{1}", relativeExpressionPattern, elementExpressionPattern );
    public static readonly string extraExpressionPatternNoGroup = string.Format( "(?<extra>{0}{1})", relativeExpressionPatternNoGroup, elementExpressionPatternNoGroup );

    public static readonly string cssSelectorPattern = string.Format( "{0}{1}*", elementExpressionPattern, extraExpressionPatternNoGroup );
    public static readonly string cssSelectorPatternNoGroup = string.Format( "{0}{1}*", elementExpressionPatternNoGroup, extraExpressionPatternNoGroup );


    public static readonly Regex cssSelectorRegex = new Regex( "^" + cssSelectorPattern + "$", RegexOptions.Compiled );

    public static readonly Regex extraRegex = new Regex( "^" + extraExpressionPattern + "$", RegexOptions.Compiled );



    private string _expression;


    /// <summary>
    /// 创建一个CSS选择器实例
    /// </summary>
    /// <param name="expression"></param>
    public HtmlCssSelector( string expression )
    {
      _expression = expression;

      var match = cssSelectorRegex.Match( expression );
      if ( !match.Success )
        throw new FormatException();


      _selector = new PartSelector() { ElementSelector = new ElementSelector( match.Groups["elementSelector"].Value ) };

      foreach ( var extraExpression in match.Groups["extra"].Captures.Cast<Capture>().Select( c => c.Value ) )
      {
        var extraMatch = extraRegex.Match( extraExpression );

        if ( !extraMatch.Success )
          throw new FormatException();

        var relative = extraMatch.Groups["relative"].Value.Trim();
        var elementSelector = extraMatch.Groups["elementSelector"].Value.Trim();

        var newPartSelector = new PartSelector() { ElementSelector = new ElementSelector( elementSelector ), Relative = relative, ParentSelector = _selector };
        _selector = newPartSelector;

      }
    }

    public override string ToString()
    {
      return _selector.ToString();
    }


    private PartSelector _selector;


    private class PartSelector
    {
      public string Relative { get; set; }
      public ElementSelector ElementSelector { get; set; }

      public PartSelector ParentSelector { get; set; }


      public bool Allows( IHtmlElement element, IHtmlContainer scope )
      {

        if ( !ElementSelector.Allows( element ) )
          return false;

        if ( Relative == null )
          return true;

        else if ( Relative == ">" )
          return element.Parent.Equals( scope )? false : ParentSelector.Allows( element.Parent as IHtmlElement, scope );

        else if ( Relative == "" )
          return element.Ancestors().TakeWhile( e => !e.Equals( scope ) ).Any( e => ParentSelector.Allows( e, scope ) );

        else if ( Relative == "+" )
          return ParentSelector.Allows( element.PreviousElement(), scope );

        else if ( Relative == "~" )
          return element.ElementsBeforeSelf().Any( e => ParentSelector.Allows( e, scope ) );

        else
          throw new FormatException();
      }

      public override string ToString()
      {
        if ( Relative == null )
          return ElementSelector.ToString();

        else if ( Relative == "" )
          return string.Format( "{0} {1}", ParentSelector, ElementSelector );

        else
          return string.Format( "{0} {1} {2}", ParentSelector, Relative, ElementSelector );
      }

    }


    /// <summary>
    /// 在指定容器子代元素和指定范畴下搜索满足选择器的所有元素
    /// </summary>
    /// <param name="container">容器，其所有子代元素被列入搜索范围</param>
    /// <param name="asScope">指定选择器在计算父元素时，是否不超出指定容器的范畴</param>
    /// <remarks>
    /// 选择器的工作原理是从最里层的元素选择器开始搜索，逐步验证其父元素是否满足父选择器的规则。如果asScope参数为true，则选择器在验证父元素时，将不超出container的。考虑下面的文档结构：
    /// <![CDATA[
    /// <html>
    ///   <body>
    ///     <ul id="outer">
    ///       <li id="item">
    ///         <ul "inner">
    ///           <li>123</li>
    ///           <li>456</li>
    ///         </ul>
    ///         <ol>
    ///           <li>abc</li>
    ///         </ol>
    ///       </li>
    ///     </ul>
    ///   </body>
    /// </html>
    /// ]]>
    /// 当使用选择器"#item ul li"来选择元素时，我们将得到正确的结果，即123和456两个节点。
    /// 但如果我们将#item元素当作上下文且asScope参数为false来选择"ul li"元素时，可能会不能得到预期的结果，会发现abc元素也被选择了。这是因为选择器在查找父级元素限定时，会查找到id为outter的ul元素。为了解决此问题，请将asScope参数设置为true。
    /// </remarks>
    /// <returns>搜索到的所有元素</returns>
    public IEnumerable<IHtmlElement> Find( IHtmlContainer container, bool asScope )
    {

      var elements = container.Descendant();

      return elements.Where( element => _selector.Allows( element, asScope ? container : null ) );

    }






    public class ElementSelector
    {

      private static readonly Regex regex = new Regex( elementExpressionPattern, RegexOptions.Compiled );

      public ElementSelector( string expression )
      {

        if ( string.IsNullOrEmpty( expression ) )
          throw new FormatException();

        var match = regex.Match( expression );
        if ( !match.Success )
          throw new FormatException();


        if ( match.Groups["name"].Success )
          tagName = match.Groups["name"].Value;
        else
          tagName = "*";

        attributeSelectors = match.Groups["attributeSelector"].Captures.Cast<Capture>().Select( c => new AttributeSelector( c.Value ) ).ToList();

        if ( match.Groups["identity"].Success )
          attributeSelectors.Add( new AttributeSelector( string.Format( "[id={0}]", match.Groups["identity"].Value ) ) );

        if ( match.Groups["class"].Success )
          attributeSelectors.Add( new AttributeSelector( string.Format( "[class*={0}]", match.Groups["class"].Value ) ) );
      }


      private string tagName;

      private IList<AttributeSelector> attributeSelectors;


      public bool Allows( IHtmlElement element )
      {
        if ( element == null )
          return false;


        if ( tagName != "*" && !string.Equals( element.Name, tagName, StringComparison.InvariantCultureIgnoreCase ) )
          return false;

        foreach ( var selector in attributeSelectors )
        {
          if ( !selector.Allows( element ) )
            return false;
        }

        return true;
      }


      public override string ToString()
      {
        return string.Format( "{0}{1}", tagName.ToUpper(), string.Join( "", attributeSelectors.Select( a => a.ToString() ).ToArray() ) );
      }

    }



    public class AttributeSelector
    {


      private string name;
      private string separator;
      private string value;

      private string exp;


      private static readonly Regex regex = new Regex( attributeExpressionPattern, RegexOptions.Compiled );


      private delegate bool ValueMatcher( string exp, string value );

      private Dictionary<string, ValueMatcher> matchers = new Dictionary<string, ValueMatcher>()
      {
        { "^=", ( exp, value ) => value != null && value.StartsWith( exp ) },
        { "$=", ( exp, value ) => value != null && value.EndsWith( exp ) },
        { "*=", ( exp, value ) => value != null && value.Contains( exp ) },
        { "~=", ( exp, value ) => value != null && value.Split( ' ' ).Contains( exp ) },
        { "!=", ( exp, value ) => value != exp },
        { "=",  ( exp, value ) => value == exp }
      };





      public AttributeSelector( string expression )
      {

        exp = expression;

        var match = regex.Match( expression );

        if ( !match.Success )
          throw new FormatException();


        name = match.Groups["name"].Value;
        if ( match.Groups["separator"].Success )
        {
          separator = match.Groups["separator"].Value;
          if ( match.Groups["quoteText"].Success )
            value = match.Groups["quoteText"].Value;
          else
            value = match.Groups["value"].Value;
        }

      }


      public bool Allows( IHtmlElement element )
      {

        string _value = null;

        var attribute = element.Attribute( name );

        if ( separator == null )//如果没有运算符，那么表示判断属性是否存在
          return attribute != null;

        if ( attribute != null )
          _value = attribute.Value;

        return matchers[separator]( value, _value );
      }


      public override string ToString()
      {
        if ( separator != null )
          return string.Format( "[{0}{1}'{2}']", name, separator, value.Replace( "'", "\\'" ).Replace( "\"", "\\\"" ) );
        else
          return string.Format( "[{0}]", name );
      }

    }



    public class PseudoClassSelector
    {

    }

  }
}
