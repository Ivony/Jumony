using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ivony.Fluent;

namespace Ivony.Web
{

  /// <summary>
  /// 客户端缓存策略支持模块
  /// </summary>
  public sealed class ClientCachePolicyModule : IHttpModule
  {
    void IHttpModule.Dispose()
    {
    }

    void IHttpModule.Init( HttpApplication context )
    {

      context.BeginRequest += OnBeginRequest;
      context.PreSendRequestHeaders += OnPreSendRequestHeaders;

    }

    private void OnBeginRequest( object sender, EventArgs e )
    {

      var context = sender.CastTo<HttpApplication>().Context;
      context.Items[ClientCachePolicy.Token] = new ClientCachePolicy();
    }

    private void OnPreSendRequestHeaders( object sender, EventArgs e )
    {
      var context = sender.CastTo<HttpApplication>().Context;
      var policy = context.Items[ClientCachePolicy.Token] as ClientCachePolicy;

      if ( policy != null )
        policy.ApplyClientCachePolicy( new HttpContextWrapper( context ).Response );
    }
  }
}
