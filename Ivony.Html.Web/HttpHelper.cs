using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{

  public static class HttpHelper
  {


    private const string originUrlToken = "Jumony_HttpContext_OriginUrl";

    public static Uri GetOriginUri( this HttpContext context )
    {
      return (Uri) context.Items[originUrlToken];
    }

    internal static void SetOriginUrl( this HttpContext context )
    {
      context.Items[originUrlToken] = context.Request.Url;
    }



    private const string requestDataToken = "Jumony_HttpContext_RequestMapResult";

    public static MapInfo GetMapperResult( this HttpContext context )
    {
      return (MapInfo) context.Items[requestDataToken];
    }

    internal static void SetMapResult( this HttpContext context, MapInfo data )
    {
      context.Items[requestDataToken] = data;
    }


  }

}
