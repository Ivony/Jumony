using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{
  public interface IMvcCachePolicyProvider
  {

    CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action );

  }

  public class MvcCachePolicyProvider : IMvcCachePolicyProvider
  {

    private IHtmlCachePolicyProvider _provider;
    
    public MvcCachePolicyProvider( IHtmlCachePolicyProvider provider )
    {
      _provider = provider;
    }


    CachePolicy IMvcCachePolicyProvider.CreateCachePolicy( ControllerContext context, ActionDescriptor action )
    {
      return _provider.CreateCachePolicy( context.HttpContext );
    }
  }


}
