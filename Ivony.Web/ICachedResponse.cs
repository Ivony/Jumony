using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Web
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


  /// <summary>
  /// 可以进行客户端缓存的响应
  /// </summary>
  public interface IClientCacheableResponse : ICachedResponse
  {
    /// <summary>
    /// 创建响应的 ETag 标识
    /// </summary>
    /// <returns>响应的 ETag 标识</returns>
    string CreateETag();
  }

}
