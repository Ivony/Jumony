using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 定义文档碎片管理器，用于分析和创建文档碎片
  /// </summary>
  public interface IHtmlFragmentManager
  {

    /// <summary>
    /// 文档碎片提供程序所属的文档
    /// </summary>
    IHtmlDocument Document
    {
      get;
    }


    /// <summary>
    /// 获取所有尚未分配的文档碎片
    /// </summary>
    IEnumerable<IHtmlFragment> AllFragments
    {
      get;
    }


    /// <summary>
    /// 创建一个文档碎片
    /// </summary>
    /// <returns></returns>
    IHtmlFragment CreateFragment();

    /// <summary>
    /// 分析并创建一个文档碎片
    /// </summary>
    /// <param name="html">要分析用于创建文档碎片的 HTML</param>
    /// <returns></returns>
    IHtmlFragment ParseFragment( string html );

  }

}