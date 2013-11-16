using System;
using System.ComponentModel;
using System.Collections;
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


  }


  /// <summary>
  /// HTML DOM 模型的对象实现此接口声明自己可以保存一些额外的数据，一般而言，原生的 DOM 模型都应实现此接口以支持一些额外的功能。
  /// </summary>
  public interface IDataContainer : IHtmlDomObject
  {

    /// <summary>
    /// 额外的数据存放容器
    /// </summary>
    IDictionary Data { get; }
  }

}
