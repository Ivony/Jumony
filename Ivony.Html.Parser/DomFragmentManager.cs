using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// IHtmlFragmentManager 的实现
  /// </summary>
  public class DomFragmentManager : IHtmlFragmentManager
  {


    private object _sync = new object();

    /// <summary>
    /// 用于同步的对象
    /// </summary>
    public object SyncRoot
    {
      get { return _sync; }
    }

    private DomDocument _document;
    private IList<DomFragment> _fragments;

    /// <summary>
    /// 构造 DomFragmentManager 对象
    /// </summary>
    /// <param name="document">FragmentManager 对象所属的文档</param>
    public DomFragmentManager( DomDocument document )
    {
      lock ( document.SyncRoot )
      {
        if ( document.FragmentManager != null )
          throw new InvalidOperationException();

        _document = document;

        _fragments = new SynchronizedCollection<DomFragment>( SyncRoot );
      }
    }

    /// <summary>
    /// FragmentManager 对象所属的文档
    /// </summary>
    public IHtmlDocument Document
    {
      get { return _document; }
    }

    /// <summary>
    /// 获取所有的文档碎片
    /// </summary>
    public IEnumerable<IHtmlFragment> AllFragments
    {
      get
      {
        return _fragments.Cast<IHtmlFragment>();
      }
    }


    /// <summary>
    /// 创建一个文档碎片
    /// </summary>
    public IHtmlFragment CreateFragment()
    {
      return new DomFragment( this );
    }


    /// <summary>
    /// 分析一段 HTML 成为一个文档碎片
    /// </summary>
    public IHtmlFragment ParseFragment( string html )
    {
      return new DomFragment( this, html );
    }


    /// <summary>
    /// 当文档碎片被分配到文档上时被调用，用于释放碎片。
    /// </summary>
    /// <param name="fragment"></param>
    internal void Allocated( DomFragment fragment )
    {
      lock ( SyncRoot )
      {
        _fragments.Remove( fragment );
      }
    }

    /// <summary>
    /// 文档模型修改器
    /// </summary>
    public DomModifier DomModifier
    {
      get { return _document.DomModifier; }
    }

  }
}
