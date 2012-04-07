using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Ivony.Fluent;

namespace Ivony.Html
{


  /// <summary>
  /// CSS元素选择器
  /// </summary>
  /// <remarks>
  /// 此类型实例是线程安全的。
  /// </remarks>
  public sealed class CssElementSelector : ICssSelector
  {
    /// <summary>
    /// 匹配CSS元素选择器的正则表达式。
    /// </summary>
    public static readonly Regex elementSelectorRegex = new Regex( Regulars.elementExpressionPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant );


    /// <summary>
    /// 调用此方法预热选择器
    /// </summary>
    public static void WarmUp()
    {
      elementSelectorRegex.IsMatch( "" );
    }


    /// <summary>
    /// 创建一个CSS元素选择器
    /// </summary>
    /// <param name="expression">元素选择表达式</param>
    public static ICssSelector Create( string expression )
    {
      string tagName;

      if ( string.IsNullOrEmpty( expression ) )
        throw new ArgumentNullException( "expression" );

      var match = elementSelectorRegex.Match( expression );
      if ( !match.Success )
        throw new FormatException( string.Format( CultureInfo.InvariantCulture, "元素选择器 \"{0}\" 格式不正确", expression ) );


      if ( match.Groups["name"].Success )
        tagName = match.Groups["name"].Value;
      else
        tagName = "*";

      var _attributeSelectors = match.Groups["attributeSelector"].Captures.Cast<Capture>().Select( c => new CssAttributeSelector( c.Value ) ).ToList();

      if ( match.Groups["identity"].Success )
        _attributeSelectors.Add( new CssAttributeSelector( string.Format( CultureInfo.InvariantCulture, "[id={0}]", match.Groups["identity"].Value ) ) );

      if ( match.Groups["class"].Success )
      {

        foreach ( Capture capture in match.Groups["class"].Captures )
          _attributeSelectors.Add( new CssAttributeSelector( string.Format( CultureInfo.InvariantCulture, "[class~={0}]", capture.Value ) ) );
      }


      var attributeSelectors = _attributeSelectors.ToArray();

      var pseudoClassSelectors = match.Groups["pseudoClassSelector"].Captures.Cast<Capture>().Select( c => CssPseudoClassSelectors.Create( c.Value ) ).ToArray();

      return new CssElementSelector( tagName, attributeSelectors, pseudoClassSelectors );

    }


    private CssElementSelector( string tagName, CssAttributeSelector[] attributes, ICssPseudoClassSelector[] pseudoClasses )
    {

      if ( string.IsNullOrEmpty( tagName ) )
        tagName = "*";

      elementType = tagName;
      attributeSelectors = attributes;
      pseudoClassSelectors = pseudoClasses;
    }



    private readonly string elementType;

    private readonly CssAttributeSelector[] attributeSelectors;

    private readonly ICssPseudoClassSelector[] pseudoClassSelectors;


    /// <summary>
    /// 检查一个元素是否符合选择条件
    /// </summary>
    /// <param name="element">要检查的元素</param>
    /// <returns>是否符合</returns>
    public bool IsEligible( IHtmlElement element )
    {
      if ( element == null )
        return false;


      if ( elementType != "*" && !element.Name.EqualsIgnoreCase( elementType ) )
        return false;

      foreach ( var selector in attributeSelectors )
      {
        if ( !selector.IsEligible( element ) )
          return false;
      }

      foreach ( var selector in pseudoClassSelectors )
      {
        if ( !selector.IsEligible( element ) )
          return false;
      }

      return true;
    }


    /// <summary>
    /// 从元素集合中筛选出符合条件的元素
    /// </summary>
    /// <param name="source">要筛选的元素集合</param>
    /// <returns>筛选结果</returns>
    public IEnumerable<IHtmlElement> Filter( IEnumerable<IHtmlElement> source )
    {
      return source.Where( element => IsEligible( element ) );
    }


    /// <summary>
    /// 返回表示当前选择器的表达式
    /// </summary>
    /// <returns>表示当前选择器的表达式</returns>
    public override string ToString()
    {
      return string.Format( CultureInfo.InvariantCulture, "{0}{1}{2}", elementType.ToUpper( CultureInfo.InvariantCulture ), string.Join( "", attributeSelectors.Select( a => a.ToString() ).ToArray() ), string.Join( "", pseudoClassSelectors.Select( p => p.ToString() ).ToArray() ) );
    }


    /// <summary>
    /// 获取元素名限定条件，如没有限制，则返回"*"
    /// </summary>
    public string ElementName { get { return elementType; } }



  }
}
