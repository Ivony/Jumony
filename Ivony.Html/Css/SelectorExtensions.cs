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
  public static class SelectorExtensions
  {


    /// <summary>
    /// 使用选择器从元素集中筛选出符合选择器要求的元素
    /// </summary>
    /// <param name="selector">选择器</param>
    /// <param name="source">源元素集</param>
    /// <returns>筛选结果</returns>
    public static IEnumerable<IHtmlElement> Filter( this ISelector selector, IEnumerable<IHtmlElement> source )
    {

      if ( selector == null )
        throw new ArgumentNullException( "selector" );

      if ( source == null )
        return null;

      return source.Where( selector.IsEligible );
    }


    /// <summary>
    /// 使用选择器从元素集中筛选出符合选择器要求的元素
    /// </summary>
    /// <param name="source">源元素集</param>
    /// <param name="selector">选择器</param>
    /// <returns>筛选结果</returns>
    public static IEnumerable<IHtmlElement> FilterBy( this IEnumerable<IHtmlElement> source, ISelector selector )
    {
      if ( source == null )
        return null;

      if ( selector == null )
        return source;

      return source.Where( selector.IsEligible );
    }

    /// <summary>
    /// 使用选择器从元素集中筛选出符合选择器要求的元素
    /// </summary>
    /// <param name="source">源元素集</param>
    /// <param name="selector">选择器</param>
    /// <returns>筛选结果</returns>
    public static IEnumerable<IHtmlElement> FilterBy( this IEnumerable<IHtmlElement> source, string selector )
    {
      if ( source == null )
        return null;

      if ( string.IsNullOrEmpty( selector ) )
        return source;

      return FilterBy( source, CssParser.ParseSelector( selector ) );
    }


    /// <summary>
    /// 检查元素是否符合指定选择器要求，并缓存结果于元素当前文档版本
    /// </summary>
    /// <param name="selector">选择器</param>
    /// <param name="element">元素</param>
    /// <returns>是否符合选择器要求</returns>
    public static bool IsEligibleBuffered( this ISelector selector, IHtmlElement element )
    {
      if ( selector == null )
        throw new ArgumentNullException( "selector" );

      if ( element == null )
        return selector.IsEligible( element );

      var cacheContainer = element.Document as IVersionCacheContainer;
      if ( cacheContainer == null )
        return selector.IsEligible( element );


      lock ( cacheContainer.SyncRoot )
      {
        var cache = cacheContainer.CurrenctVersionCache[selector] as Dictionary<IHtmlElement, bool>;

        if ( cache != null )
        {

          bool result;
          if ( cache.TryGetValue( element, out result ) )
            return result;
        }

        else
          cacheContainer.CurrenctVersionCache[selector] = cache = new Dictionary<IHtmlElement, bool>();

        return cache[element] = selector.IsEligible( element );

      }
    }




  }
}
