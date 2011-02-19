using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{
  /// <summary>
  /// IHtmlDocument 的实现
  /// </summary>
  public class DomDocument : DomObject, IHtmlDocument, IDomContainer
  {

    /// <summary>
    /// 创建 DomDocument 对象
    /// </summary>
    /// <param name="url">文档的 URL</param>
    public DomDocument( Uri url )
    {
      _url = url;
    }


    private Uri _url;

    /// <summary>
    /// 获取文档的 URL
    /// </summary>
    public Uri DocumentUri
    {
      get { return _url; }
    }

    /// <summary>
    /// 获取当前对象所属的文档，总是返回自身
    /// </summary>
    public override IHtmlDocument Document
    {
      get { return this; }
    }

    /// <summary>
    /// 获取文档类型的声明，不支持，总是返回null
    /// </summary>
    public string DocumentDeclaration
    {
      get { return null; }
    }

    /// <summary>
    /// 获取用于创建游离节点的节点工厂
    /// </summary>
    /// <returns></returns>
    public virtual IHtmlNodeFactory GetNodeFactory()
    {
      return new DomFactory( this );
    }



    private DomNodeCollection collection;

    /// <summary>
    /// 获取节点容器
    /// </summary>
    public DomNodeCollection NodeCollection
    {
      get
      {
        if ( collection == null )
          collection = new DomNodeCollection( this );

        return collection;
      }
    }

    /// <summary>
    /// 获取所有子节点
    /// </summary>
    public IEnumerable<IHtmlNode> Nodes()
    {
      if ( collection == null )
        collection = new DomNodeCollection( this );

      return collection;
    }
  }
}
