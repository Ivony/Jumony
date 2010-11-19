using System;
using System.ComponentModel;
namespace Ivony.Html
{

  /// <summary>
  /// 表示一个 HTML 对象
  /// </summary>
  public interface IHtmlObject
  {


    /// <summary>
    /// 获取在原始文档对象树上的对象
    /// </summary>
    object RawObject
    {
      get;
    }


    /// <summary>
    /// 获取节点所属的文档
    /// </summary>
    IHtmlDocument Document
    {
      get;
    }

    object SyncRoot
    {
      get;
    }
  }
}
