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



    public delegate bool CssSelectorCombinatorSelector( ISelector leftSelector, IHtmlElement element );


    public CssElementSelector RightSelector { get; private set; }

    public ISelector LeftSelector { get; private set; }

    public char CombinatorSymbol { get; private set; }

    protected CssSelectorCombinatorSelector Combinator
    {
      get;
      private set;
    }


    /// <summary>
    /// 创建 CSS 层叠选择器对象
    /// </summary>
    /// <param name="relativeSelector">关系选择器</param>
    /// <param name="lastSelector">附加的最后一个选择器</param>
    private CssCasecadingSelector( ISelector leftSelector, char combinator, CssElementSelector rightSelector )
    {

      if ( leftSelector == null )
        throw new ArgumentNullException( "leftSelector" );

      if ( rightSelector == null )
        throw new ArgumentNullException( "rightSelector" );


      RightSelector = rightSelector;
      LeftSelector = leftSelector;
      Combinator = CreateCombinator( combinator );
      CombinatorSymbol = combinator;

    }

    protected CssSelectorCombinatorSelector CreateCombinator( char combinator )
    {

      switch ( combinator )
      {
        case '>':
          return ( selector, element ) => selector.IsEligible( element.Parent() );

        case ' ':
          return ( selector, element ) => element.Ancestors().Any( e => selector.IsEligible( e ) );

        case '+':
          return ( selector, element ) => selector.IsEligible( element.PreviousElement() );

        case '~':
          return ( selector, element ) => element.SiblingsBeforeSelf().Any( e => selector.IsEligible( e ) );

        default:
          throw new NotSupportedException();

      }

    }



    /// <summary>
    /// 将层叠选择器与另一个选择器使用指定连接符合并为一个
    /// </summary>
    /// <param name="left">合并后在左边的选择器</param>
    /// <param name="combinator">连接符</param>
    /// <param name="right">合并后在右边的选择器</param>
    /// <returns>合并后的选择器</returns>
    public static CssCasecadingSelector Combine( CssCasecadingSelector left, char combinator, ISelector right )
    {
      if ( left == null )
        throw new ArgumentNullException( "left" );

      if ( right == null )
        throw new ArgumentNullException( "right" );


      var casecadingSelector = right as CssCasecadingSelector;
      if ( casecadingSelector != null )
        return Combine( left, combinator, casecadingSelector );

      var elementSelector = right as CssElementSelector;
      if ( casecadingSelector != null )
        return Combine( left, combinator, elementSelector );

      throw new NotSupportedException();
    }


    /// <summary>
    /// 将层叠选择器与另一个选择器使用指定连接符合并为一个
    /// </summary>
    /// <param name="left">合并后在左边的选择器</param>
    /// <param name="combinator">连接符</param>
    /// <param name="right">合并后在右边的选择器</param>
    /// <returns>合并后的选择器</returns>
    public static CssCasecadingSelector Combine( CssCasecadingSelector left, char combinator, CssCasecadingSelector right )
    {
      if ( left == null )
        throw new ArgumentNullException( "left" );

      if ( right == null )
        throw new ArgumentNullException( "right" );


      return Combine( Combine( left, combinator, right.LeftSelector ), right.CombinatorSymbol, right.RightSelector );
    }


    /// <summary>
    /// 将层叠选择器与另一个选择器使用指定连接符合并为一个
    /// </summary>
    /// <param name="left">合并后在左边的选择器</param>
    /// <param name="combinator">连接符</param>
    /// <param name="right">合并后在右边的选择器</param>
    /// <returns>合并后的选择器</returns>
    public static CssCasecadingSelector Combine( CssCasecadingSelector left, char combinator, CssElementSelector right )
    {
      return new CssCasecadingSelector( left, combinator, right );
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



      if ( !RightSelector.IsEligible( element ) )
        return false;

      return Combinator( LeftSelector, element );

    }



    /// <summary>
    /// 返回表示当前选择器的表达式
    /// </summary>
    /// <returns>表示当前选择器的表达式</returns>
    public override string ToString()
    {
      return string.Format( CultureInfo.InvariantCulture, "{0}{1}{2}", LeftSelector, CombinatorSymbol, RightSelector );
    }


  }
}
