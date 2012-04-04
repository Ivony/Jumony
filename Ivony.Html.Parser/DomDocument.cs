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
  public class DomDocument : DomObject, IHtmlDocument, IDomContainer, INotifyDomChanged
  {

    /// <summary>
    /// 创建 DomDocument 对象
    /// </summary>
    /// <param name="uri">文档的 URL</param>
    public DomDocument( Uri uri )
    {
      _uri = uri;
      _manager = new DomFragmentManager( this );
      _modifier = new DomModifier();

      _modifier.HtmlDomChanged += OnDomChanged;
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
    /// <summary>
    /// 文档碎片管理器
    /// </summary>
    public IHtmlFragmentManager FragmentManager
    {
      get { return _manager; }
    }


    private DomModifier _modifier;

    /// <summary>
    /// 文档模型修改器
    /// </summary>
    public DomModifier DomModifier
    {
      get { return _modifier; }
    }

    /// <summary>
    /// 文档模型修改器
    /// </summary>
    IHtmlDomModifier IHtmlDocument.DomModifier
    {
      get { return _modifier; }
    }



    private object _sync = new object();

    /// <summary>
    /// 用于同步操作的同步对象
    /// </summary>
    public object SyncRoot
    {
      get { return _sync; }
    }



    /// <summary>
    /// 当文档任何部分被修改时会引发的事件。
    /// </summary>
    public event EventHandler<HtmlDomChangedEventArgs> HtmlDomChanged;

    /// <summary>
    /// 引发 HtmlDomChanged 事件
    /// </summary>
    /// <param name="sender">引发事件的事件源</param>
    /// <param name="e">HtmlDomChanged 事件参数</param>
    protected virtual void OnDomChanged( object sender, HtmlDomChangedEventArgs e )
    {
      if ( HtmlDomChanged != null )
      {
        if ( object.Equals( this, e.Node.Document ) )
          HtmlDomChanged( sender, e );
      }
    }
  }
}
