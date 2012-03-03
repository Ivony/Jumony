using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{
  public class ClientCachePolicyModule : IHttpModule
  {
    public void Dispose()
    {
    }

    public void Init( HttpApplication context )
    {

      context.BeginRequest += OnBeginRequest;
      context.PreSendRequestHeaders += OnPreSendRequestHeaders;

    }

    public void OnBeginRequest( object sender, EventArgs e )
    {
      var context = HttpContext.Current;
      context.Items[ClientCachePolicy.Token] = new ClientCachePolicy();
    }

    public void OnPreSendRequestHeaders( object sender, EventArgs e )
    {
      var context = HttpContext.Current;
      var policy = context.Items[ClientCachePolicy.Token] as ClientCachePolicy;

      if ( policy != null )
        policy.ApplyClientCachePolicy( new HttpContextWrapper( context ).Response );
    }
  }
}
