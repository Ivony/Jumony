using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.IO;

namespace Ivony.Html.Web
{
  public class RawResponse : ICachedResponse
  {

    public RawResponse()
    {
      Headers = new NameValueCollection();
      Encoding = Encoding.UTF8;
    }


    public NameValueCollection Headers
    {
      get;
      set;
    }

    public string Content
    {
      get;
      set;
    }

    public Encoding Encoding
    {
      get;
      set;
    }



    public virtual void Apply( HttpResponseBase response )
    {
      response.Clear();

      foreach ( var key in Headers.AllKeys )
      {
        foreach ( var value in Headers.GetValues( key ) )
        {
          response.AppendHeader( key, value );
        }
      }

      response.ContentEncoding = Encoding;

      response.Charset = Encoding.WebName;

      response.Write( Content );

    }

  }
}
