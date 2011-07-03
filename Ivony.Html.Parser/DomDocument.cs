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
    /// <param name="uri">文档的 URL</param>
    public DomDocument( Uri uri )
    {
      _uri = uri;
      _manager = new DomFragmentManager( this );
    }


    private Uri _uri;

    /// <summary>
    /// 获取文档的 URL
    /// </summary>
    public Uri DocumentUri
    {
      get { return _uri; }
      internal set { _uri = value; }
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



    private DomNodeCollection _nodeCollection;

    /// <summary>
    /// 获取节点容器
    /// </summary>
    public DomNodeCollection NodeCollection
    {
      get
      {
        if ( _nodeCollection == null )
          _nodeCollection = new DomNodeCollection( this );

        return _nodeCollection;
      }
    }

    /// <summary>
    /// 获取所有子节点
    /// </summary>
    public IEnumerable<IHtmlNode> Nodes()
    {
      if ( _nodeCollection == null )
        _nodeCollection = new DomNodeCollection( this );

      return _nodeCollection.HtmlNodes;
    }



    private DomFragmentManager _manager;
    public IHtmlFragmentManager FragmentManager
    {
      get { return _manager; }
    }


    private DomModifier _modifier = new DomModifier();
    public IHtmlDomModifier DomModifier
    {
      get { return _modifier; }
    }



    private object _sync = new object();

    public object SyncRoot
    {
      get { return _sync; }
    }



  }
}
