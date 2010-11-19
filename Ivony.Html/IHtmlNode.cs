using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ivony.Html
{
  public interface IHtmlNode : IHtmlObject
  {
    /// <summary>
    /// 获取节点的父级
    /// </summary>
    IHtmlContainer Parent
    {
      get;
    }


    /// <summary>
    /// 获取节点的原始 HTML，如果不支持，请返回null
    /// </summary>
    [EditorBrowsable( EditorBrowsableState.Advanced )]
    string RawHtml
    {
      get;
    }


    /// <summary>
    /// 从文档对象树上移除此节点
    /// </summary>
    void Remove();

  }
}
