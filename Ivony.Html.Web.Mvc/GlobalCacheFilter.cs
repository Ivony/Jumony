using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ivony.Fluent;

namespace Ivony.Html.Web.Mvc
{
  public sealed class GlobalCacheFilter : CacheFilterBase
  {

    internal GlobalCacheFilter() { }

    protected override CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {

      var httpMethod = context.HttpContext.Request.HttpMethod;
      if ( !httpMethod.EqualsIgnoreCase( "get" ) && !httpMethod.EqualsIgnoreCase( "header" ) )//如果不是GET或Header请求，都不予缓存。
        return null;


      //如果存在 Cacheable 筛选器，则不再提供默认策略。
      if ( action.GetCustomAttributes( typeof( CacheableAttribute ), true ).Any() || action.ControllerDescriptor.GetCustomAttributes( typeof( CacheableAttribute ), true ).Any() )
        return null;

      return MvcEnvironment.CreateCachePolicy( context, action, parameters );
    }
  }
}
