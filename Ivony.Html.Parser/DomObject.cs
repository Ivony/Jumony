using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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


    /// <summary>
    /// 获取原始的HTML
    /// </summary>
    public virtual string RawHtml
    {
      get { return null; }
    }
  }
}
