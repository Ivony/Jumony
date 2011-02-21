using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 定义一个 HTML 文档
  /// </summary>
  public interface IHtmlDocument : IHtmlContainer
  {

    /// <summary>
    /// 获取文档内容的统一资源位置
    /// </summary>
    Uri DocumentUri
    {
      get;
    }


    /// <summary>
    /// 获取文档的声明信息，可以是xml声明，也可以是DTD。如果不被支持，则返回null。
    /// </summary>
    string DocumentDeclaration
    {
      get;
    }



    /// <summary>
    /// 创建一个文档碎片
    /// </summary>
    /// <returns></returns>
    IHtmlFragment CreateFragment();

    /// <summary>
    /// 创建一个文档碎片
    /// </summary>
    /// <param name="html">要分析用于创建文档碎片的 HTML</param>
    /// <returns></returns>
    IHtmlFragment CreateFragment( string html );

    /// <summary>
    /// 获取文档节点构造器，如果不支持，则返回null
    /// </summary>
    /// <returns>文档节点构造器</returns>
    IHtmlNodeFactory GetNodeFactory();

  }
}
