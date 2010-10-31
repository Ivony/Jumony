using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ivony.Html
{
  public interface IHtmlNode
  {
    /// <summary>
    /// 获取节点的父级
    /// </summary>
    IHtmlContainer Parent
    {
      get;
    }

    /// <summary>
    /// 获取节点在原始文档对象树上的对象
    /// </summary>
    object NodeObject
    {
      get;
    }

    /// <summary>
    /// 从文档对象树上移除此节点
    /// </summary>
    void Remove();


    /// <summary>
    /// 获取节点所属的文档
    /// </summary>
    IHtmlDocument Document
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


    object SyncRoot
    {
      get;
    }


  }
}
