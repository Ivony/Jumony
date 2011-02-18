using System;
using System.ComponentModel;
namespace Ivony.Html
{

  /// <summary>
  /// 表示一个 HTML DOM 的对象
  /// </summary>
  public interface IHtmlDomObject
  {


    /// <summary>
    /// 获取在原始文档对象树上的对象，如果原始对象不存在，返回null
    /// </summary>
    object RawObject
    {
      get;
    }

    /// <summary>
    /// 获取 DOM 对象的原始 HTML，如果不支持，返回null
    /// </summary>
    [EditorBrowsable( EditorBrowsableState.Advanced )]
    string RawHtml
    {
      get;
    }




    /// <summary>
    /// 获取 DOM 对象所属的文档
    /// </summary>
    IHtmlDocument Document
    {
      get;
    }


    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    object SyncRoot
    {
      get;
    }
  }
}
