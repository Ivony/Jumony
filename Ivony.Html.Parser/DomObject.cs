using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ivony.Html.Parser.ContentModels;
using System.Collections;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// 对 IHtmlDomObject 的实现
  /// </summary>
  public abstract class DomObject : IHtmlDomObject, IDataContainer
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
    /// 分析器所提供的内容碎片
    /// </summary>
    protected internal virtual HtmlContentFragment ContentFragment
    {
      get;
      internal set;
    }


    /// <summary>
    /// 获取原始的HTML
    /// </summary>
    public virtual string RawHtml
    {
      get
      {
        //if ( ContentFragment == null )
        return null;

        //return ContentFragment.Html;
      }
    }


    private Hashtable _data = Hashtable.Synchronized( new Hashtable() );

    IDictionary IDataContainer.Data
    {
      get { return _data; }
    }
  }
}
