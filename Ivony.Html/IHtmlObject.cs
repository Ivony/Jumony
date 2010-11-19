using System;
using System.ComponentModel;
namespace Ivony.Html
{
  public interface IHtmlObject
  {


    /// <summary>
    /// 获取节点在原始文档对象树上的对象
    /// </summary>
    object NodeObject
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
