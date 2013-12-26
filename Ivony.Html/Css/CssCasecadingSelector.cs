using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Ivony.Fluent;
using Ivony.Html;

namespace Ivony.Html
{

  /// <summary>
  /// CSS层叠选择器
  /// </summary>
  /// <remarks>
  /// 层叠选择器的表达式分析过程是从左至右，而处理则是从右至左，采取从左至右的方式分析主要考虑到正则工作模式和效率问题。但由于处理方式是从右至左。
  /// 简单的说只有一个元素选择器所构成的层叠选择器，其元素选择器是位于右边的。
  /// </remarks>
  public partial class CssCasecadingSelector : ISelector
  {


    /// <summary>
    /// 创建 CSS 层叠选择器对象
    /// </summary>
    /// <param name="relativeSelector">关系选择器</param>
    /// <param name="lastSelector">附加的最后一个选择器</param>
    public CssCasecadingSelector( CssRelativeSelector relativeSelector, ISelector lastSelector )
    {

      if ( relativeSelector == null )
        throw new ArgumentNullException( "relativeSelector" );

      if ( lastSelector == null )
        throw new ArgumentNullException( "lastSelector" );


      RelativeSelector = relativeSelector;
      LastSelector = lastSelector;

    }




    /// <summary>
    /// 创建层叠选择器实例
    /// </summary>
    /// <param name="leftSelector">左选择器</param>
    /// <param name="combinator">结合符</param>
    /// <param name="rightSelector">右选择器</param>
    public static CssCasecadingSelector Create( ISelector leftSelector, char combinator, ISelector rightSelector )
    {
      if ( leftSelector == null )
        throw new ArgumentNullException( "leftSelector" );

      if ( rightSelector == null )
        throw new ArgumentNullException( "rightSelector" );


      var relativeSelctor = CreateRelativeSelector( leftSelector, combinator );
      var casecadingSelector = rightSelector as CssCasecadingSelector;

      if ( casecadingSelector != null )
        return Combine( relativeSelctor, casecadingSelector );

      return new CssCasecadingSelector( relativeSelctor, rightSelector );
    }

    /// <summary>
    /// 合并关系选择器和层叠选择器
    /// </summary>
    /// <param name="relativeSelector">关系选择器</param>
    /// <param name="selector">层叠选择器</param>
    /// <returns></returns>
    internal static CssCasecadingSelector Combine( CssRelativeSelector relativeSelector, CssCasecadingSelector selector )
    {
      relativeSelector = Combine( relativeSelector, selector.RelativeSelector );
      return new CssCasecadingSelector( relativeSelector, selector.LastSelector );
    }


    /// <summary>
    /// 合并两个关系选择器
    /// </summary>
    /// <param name="left">左关系选择器</param>
    /// <param name="right">右关系选择器</param>
    /// <returns>合并后的关系选择器</returns>
    public static CssRelativeSelector Combine( CssRelativeSelector left, CssRelativeSelector right )
    {

      if ( left == null )
        throw new ArgumentNullException( "left" );

      if ( right == null )
        throw new ArgumentNullException( "right" );


      var selector = right.LeftSelector;
      var combinator = right.Combinator;


      var casecadingSelector = selector as CssCasecadingSelector;
      if ( casecadingSelector != null )                                                                        //如果右边关系选择器的左选择器是一个层叠选择器
        return CreateRelativeSelector( CssCasecadingSelector.Combine( left, casecadingSelector ), combinator );//将层叠选择器与左边的关系选择器合并，得到一个新的层叠选择器，再根据结合符创建新的关系选择器。
      
      /*
       * 举例说明如下：
       * 假设左关系选择器是 p>，右关系选择器是 div>ul>li+
       * 则右关系选择器的左选择器(selector)是div>ul>li，结合符(combinator)是+。
       * 将selector与左边的关系选择器合并，得到：p>div>ul>li，再与结合符结合得到新的关系选择器：p>div>ul>li+
       */

      var elementSelector = selector as CssElementSelector;
      if ( elementSelector != null )
        return CreateRelativeSelector( new CssCasecadingSelector( left, elementSelector ), combinator );


      throw new NotSupportedException();
    }



    /// <summary>
    /// 关系选择器
    /// </summary>
    public CssRelativeSelector RelativeSelector
    {
      get;
      private set;
    }

    /// <summary>
    /// 最后一个选择器
    /// </summary>
    public ISelector LastSelector
    {
      get;
      private set;
    }


    /// <summary>
    /// 创建关系选择器
    /// </summary>
    /// <param name="leftSelector">左选择器</param>
    /// <param name="combanitor">关系结合符</param>
    /// <returns>关系选择器</returns>
    private static CssRelativeSelector CreateRelativeSelector( ISelector leftSelector, char combanitor )
    {
      if ( combanitor == '>' )
        return new CssParentRelativeSelector( leftSelector );

      else if ( combanitor == ' ' )
        return new CssAncetorRelativeSelector( leftSelector );

      else if ( combanitor == '+' )
        return new CssPreviousRelativeSelector( leftSelector );

      else if ( combanitor == '~' )
        return new CssSiblingsRelativeSelector( leftSelector );

      throw new NotSupportedException( "不支持的关系运算符" );
    }





    /// <summary>
    /// 检查元素是否符合选择条件
    /// </summary>
    /// <param name="element">要检查的元素</param>
    /// <returns>是否符合选择条件</returns>
    public bool IsEligible( IHtmlElement element )
    {
      if ( element == null )
        return false;



      if ( !LastSelector.IsEligible( element ) )
        return false;

      return RelativeSelector.IsEligibleBuffered( element );

    }



    /// <summary>
    /// 返回表示当前选择器的表达式
    /// </summary>
    /// <returns>表示当前选择器的表达式</returns>
    public override string ToString()
    {
      return string.Format( CultureInfo.InvariantCulture, "{0}{1}", RelativeSelector, LastSelector );
    }


    /// <summary>
    /// 创建层叠选择器
    /// </summary>
    /// <param name="relativeSelector">前置关系选择器</param>
    /// <param name="selector">右选择器</param>
    /// <returns></returns>
    public static ISelector Create( CssRelativeSelector relativeSelector, ISelector selector )
    {

      if ( relativeSelector == null )
        throw new ArgumentNullException( "relativeSelector" );

      if ( selector == null )
        throw new ArgumentNullException( "selector" );


      var elementSelector = selector as CssElementSelector;
      if ( elementSelector != null )
        return new CssCasecadingSelector( relativeSelector, elementSelector );

      var casecadingSelector = selector as CssCasecadingSelector;
      if ( casecadingSelector != null )
        return Combine( relativeSelector, casecadingSelector );

      var multipleSelector = selector as CssMultipleSelector;
      if ( multipleSelector != null )
        return CssMultipleSelector.Combine( relativeSelector, multipleSelector );


      throw new NotSupportedException();
    }


    /// <summary>
    /// 创建层叠选择器
    /// </summary>
    /// <param name="elements">作为范畴限定的元素集</param>
    /// <param name="expression">选择器表达式</param>
    /// <returns>层叠选择器</returns>
    public static ISelector Create( IEnumerable<IHtmlElement> elements, string expression )
    {

      if ( elements == null )
        throw new ArgumentNullException( "elements" );

      if ( expression == null )
        throw new ArgumentNullException( "expression" );

      var selector = CssParser.ParseSelector( expression );

      if ( elements.IsNullOrEmpty() )
        return selector;

      var relativeSelector = new CssAncetorRelativeSelector( new CssElementsRestrictionSelector( elements ) );

      return Create( relativeSelector, selector );
    }


  }
}
