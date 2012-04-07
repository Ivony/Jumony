using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using Ivony.Fluent;
using System.Diagnostics;
using System.Globalization;

namespace Ivony.Html
{

  /// <summary>
  /// 提供一系列静态和扩展方法来辅助使用 CSS 选择器。
  /// </summary>
  public static class CssSelector
  {

    private static readonly Regex cssSelectorRegex = new Regex( "^" + Regulars.cssSelectorPattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant );


    /// <summary>
    /// 创建一个 CSS 选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <returns>CSS 选择器</returns>
    public static ICssSelector Create( string expression )
    {
      if ( expression == null )
        throw new ArgumentNullException( "expression" );

      return Create( expression, null );
    }


    //选择器缓存
    private static Dictionary<string, ICssSelector[]> _selectorCache = new Dictionary<string, ICssSelector[]>( StringComparer.Ordinal );
    private static object _cacheSync = new Object();

    /// <summary>
    /// 创建一个 CSS 选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <param name="scope">范畴限定</param>
    /// <returns>选择器</returns>
    public static ICssSelector Create( string expression, IHtmlContainer scope )
    {

      if ( expression == null )
        throw new ArgumentNullException( "expression" );


      ICssSelector[] selectors;

      if ( _selectorCache.ContainsKey( expression ) )
        selectors = _selectorCache[expression];

      else
      {


        var match = cssSelectorRegex.Match( expression );

        if ( !match.Success )
          throw new FormatException( "无法识别的CSS选择器" );

        selectors = match.Groups["selector"].Captures.Cast<Capture>().Select( c => CssCasecadingSelector.Create( c.Value ) ).ToArray();
      }

      lock ( _cacheSync )
      {
        _selectorCache[expression] = selectors;
      }


      return new CssMultipleSelector( selectors.Select( s => CssCasecadingSelector.Create( s, scope ) ).ToArray() );
    }



    /// <summary>
    /// 执行CSS选择器搜索
    /// </summary>
    /// <param name="expression">CSS选择器表达式</param>
    /// <param name="scope">CSS选择器和搜索范畴</param>
    /// <returns>搜索结果</returns>
    public static IEnumerable<IHtmlElement> Search( string expression, IHtmlContainer scope )
    {

      if ( expression == null )
        throw new ArgumentNullException( "expression" );

      if ( scope == null )
        throw new ArgumentNullException( "scope" );


      try
      {
        var selector = Create( expression, scope );
        return selector.Filter( scope.Descendants() );
      }
      catch ( Exception e )
      {
        if ( e.Data != null && !e.Data.Contains( "selector expression" ) )
          e.Data["selector expression"] = expression;

        throw;
      }
    }

    /// <summary>
    /// 使用选择器从元素集中筛选出符合选择器要求的元素
    /// </summary>
    /// <param name="selector">选择器</param>
    /// <param name="source">源元素集</param>
    /// <returns>筛选结果</returns>
    public static IEnumerable<IHtmlElement> Filter( this ICssSelector selector, IEnumerable<IHtmlElement> source )
    {
      return source.Where( e => selector.IsEligible( e ) );
    }



    /// <summary>
    /// 调用此方法预热选择器
    /// </summary>
    public static void WarmUp()
    {
      cssSelectorRegex.IsMatch( "" );
      CssCasecadingSelector.WarmUp();
    }

  }
}
