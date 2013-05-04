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
    /// 获取文档碎片的管理器，如不支持文档碎片，请返回 null 。
    /// </summary>
    IHtmlFragmentManager FragmentManager
    {
      get;
    }


    /// <summary>
    /// 获取修改 DOM 结构的修改器，如不支持修改 DOM 结构，请返回 null 。
    /// </summary>
    IHtmlDomModifier DomModifier
    {
      get;
    }

    
    /// <summary>
    /// 获取该文档应当遵循的 HTML 规范
    /// </summary>
    HtmlSpecificationBase HtmlSpecification
    {
      get;
    }

  }
}
