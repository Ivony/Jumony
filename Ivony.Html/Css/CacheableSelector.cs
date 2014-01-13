using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 可以对筛选结果进行缓存的 CSS 选择器
  /// </summary>
  public abstract class CacheableSelector : ISelector
  {

    /// <summary>
    /// 检查元素是否符合选择器要求（此方法会自动缓存结果）
    /// </summary>
    /// <param name="element">要检查的元素</param>
    /// <returns>是否符合选择器的要求</returns>
    public virtual bool IsEligible( IHtmlElement element )
    {
      if ( element == null )
        return false;

      var cacheContainer = element.Document as IVersionCacheContainer;
      if ( cacheContainer == null )
        return IsEligibleCore( element );


      lock ( cacheContainer.SyncRoot )
      {
        var cache = cacheContainer.CurrenctVersionCache[this] as Dictionary<IHtmlElement, bool>;

        if ( cache != null )
        {

          bool result;
          if ( cache.TryGetValue( element, out result ) )
            return result;
        }

        else
          cacheContainer.CurrenctVersionCache[this] = cache = new Dictionary<IHtmlElement, bool>();

        return cache[element] = IsEligibleCore( element );

      }
    }


    /// <summary>
    /// 派生类实现此方法检查元素是否符合选择器要求
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    protected abstract bool IsEligibleCore( IHtmlElement element );

    /// <summary>
    /// 创建现有 CSS 选择器的自动缓存包装
    /// </summary>
    /// <param name="selector">已有的 CSS 选择器</param>
    /// <returns>对已有选择器的自动缓存的包装</returns>
    public static ISelector CreateCacheableWrapper( ISelector selector )
    {
      var cacheable = selector as CacheableSelector;
      if ( cacheable == null )
        cacheable = new CacheableCssSelectorWrapper( selector );

      return cacheable;
    }





    private class CacheableCssSelectorWrapper : CacheableSelector
    {

      private ISelector _selector;

      public CacheableCssSelectorWrapper( ISelector selector )
      {
        _selector = selector;
      }

      protected override bool IsEligibleCore( IHtmlElement element )
      {
        return _selector.IsEligible( element );
      }

      public override string ToString()
      {
        return _selector.ToString();
      }

      public override bool Equals( object obj )
      {
        return _selector.Equals( obj );
      }

      public override int GetHashCode()
      {
        return _selector.GetHashCode();
      }

    }
  }
}
