using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// 对 IHtmlDomObject 的实现
  /// </summary>
  public abstract class DomObject : IHtmlDomObject
  {
    object IHtmlDomObject.RawObject
    {
      get { return this; }
    }


    /// <summary>
    /// 获取 DOM 对象所属的文档
    /// </summary>
    public abstract IHtmlDocument Document
    {
      get;
    }


    private readonly object _sync = new object();

    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    public object SyncRoot
    {
      get { return _sync; }
    }


    /// <summary>
    /// 获取原始的HTML
    /// </summary>
    public virtual string RawHtml
    {
      get { return null; }
    }
  }
}
