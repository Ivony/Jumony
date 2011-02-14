using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 HTML 文档节点
  /// </summary>
  public interface IHtmlNode : IHtmlDomObject
  {
    /// <summary>
    /// 获取节点的容器
    /// </summary>
    IHtmlContainer Container
    {
      get;
    }



    /// <summary>
    /// 从文档对象树上移除此节点
    /// </summary>
    void Remove();

  }
}
