using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 可以对筛选结果进行缓存的 CSS 选择器
  /// </summary>
  public abstract class CssCacheableSelector : ICssSelector
  {

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

    protected abstract bool IsEligibleCore( IHtmlElement element );

    public static ICssSelector CreateCacheableWrapper( ICssSelector selector )
    {
      var cacheable = selector as CssCacheableSelector;
      if ( cacheable == null )
        cacheable = new CacheableCssSelectorWrapper( selector );

      return cacheable;
    }





    private class CacheableCssSelectorWrapper : CssCacheableSelector
    {

      private ICssSelector _selector;

      public CacheableCssSelectorWrapper( ICssSelector selector )
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
