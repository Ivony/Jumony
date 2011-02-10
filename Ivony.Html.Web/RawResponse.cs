using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.IO;

namespace Ivony.Html.Web
{
 
  
  /// <summary>
  /// 定义响应内容，用于缓存
  /// </summary>
  public class RawResponse : ICachedResponse
  {

    /// <summary>
    /// 创建 RawResponse 对象
    /// </summary>
    public RawResponse()
    {
      Headers = new NameValueCollection();

      StatusCode = 200;
      Status = "OK";

      HeaderEncoding = Encoding.UTF8;
      ContentEncoding = Encoding.UTF8;
    }


    public int StatusCode
    {
      get;
      set;
    }

    public string Status
    {
      get;
      set;
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


    public Encoding HeaderEncoding
    {
      get;
      set;
    }

    public Encoding ContentEncoding
    {
      get;
      set;
    }



    public virtual void Apply( HttpResponseBase response )
    {
      response.Clear();

      response.ContentEncoding = ContentEncoding;


      foreach ( var key in Headers.AllKeys )
      {
        foreach ( var value in Headers.GetValues( key ) )
        {
          response.AppendHeader( key, value );
        }
      }


      response.Write( Content );

    }
  }
}
