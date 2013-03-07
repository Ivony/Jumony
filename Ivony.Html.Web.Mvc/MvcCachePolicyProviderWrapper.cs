using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using Ivony.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 包装原始的 ICachePolicyProvider 对象，使其成为一个 IMvcCachePolicyProvider 对象
  /// </summary>
  public class MvcCachePolicyProviderWrapper : IMvcCachePolicyProvider
  {

    private ICachePolicyProvider _provider;

    /// <summary>
    /// 创建 MvcCachePolicyProviderWrapper 对象
    /// </summary>
    /// <param name="provider">需要被包装的原始的 ICachePolicyProvider 对象</param>
    public MvcCachePolicyProviderWrapper( ICachePolicyProvider provider )
    {
      _provider = provider;
    }


    CachePolicy IMvcCachePolicyProvider.CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {
      if ( context.IsChildAction )
        return null;

      return _provider.CreateCachePolicy( context.HttpContext );
    }



    CachePolicy ICachePolicyProvider.CreateCachePolicy( HttpContextBase context )
    {
      return _provider.CreateCachePolicy( context );
    }
  }
}
