using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义被缓存的响应内容
  /// </summary>
  public interface ICachedResponse
  {

    /// <summary>
    /// 将缓存的内容应用到响应
    /// </summary>
    /// <param name="response">响应对象</param>
    void Apply( HttpResponseBase response );

  }


  public interface IClientCacheableResponse : ICachedResponse
  {
    string CreateETag();
  }

}
