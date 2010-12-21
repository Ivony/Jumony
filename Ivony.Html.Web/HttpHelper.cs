using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Ivony.Html.Web
{

  public static class HttpHelper
  {



    private const string requestDataToken = "Jumony_HttpContext_RequestMapResult";

    public static RequestMapResult GetMapperResult( this HttpContext context )
    {
      return (RequestMapResult) context.Items[requestDataToken];
    }

    internal static void SetMapResult( this HttpContext context, RequestMapResult data )
    {
      context.Items[requestDataToken] = data;
    }


  }
}
