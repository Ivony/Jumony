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
  public sealed class CssElementSelector : ISelector
  {

    /// <summary>
    /// 创建CSS元素选择器
    /// </summary>
    /// <param name="name">元素名</param>
    /// <param name="attributes">属性选择器</param>
    /// <param name="pseudoClasses">伪类选择器</param>
    public CssElementSelector( string name, CssAttributeSelector[] attributes, ICssPseudoClassSelector[] pseudoClasses )
    {

      if ( string.IsNullOrEmpty( name ) )
        name = "*";

      elementType = name;
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
