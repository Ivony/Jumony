using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html
{
  public interface IHtmlDocument : IHtmlContainer
  {
    /// <summary>
    /// 获取文档的声明信息，可以是xml声明，也可以是DTD。如果不被支持，则返回null。
    /// </summary>
    string DocumentDeclaration
    {
      get;
    }


    /// <summary>
    /// 获取一个节点的句柄或标识
    /// </summary>
    /// <param name="node">要获取标识的节点</param>
    /// <returns>节点的标识</returns>
    string Handle( IHtmlNode node );

    /// <summary>
    /// 通过句柄或标识获取节点
    /// </summary>
    /// <param name="handler">节点的标识</param>
    /// <returns>标识的节点</returns>
    IHtmlNode Handle( string handler );


  }
}
