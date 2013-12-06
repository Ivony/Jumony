using Ivony.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 部分视图的响应结果
  /// </summary>
  public class PartialResponse : ICachedResponse
  {
    /// <summary>
    /// 响应内容
    /// </summary>
    public string Content { get; set; }


    /// <summary>
    /// 将响应内容应用到当前响应流
    /// </summary>
    /// <param name="response">当前响应</param>
    public void Apply( HttpResponseBase response )
    {
      response.Write( Content );
    }
  }
}
