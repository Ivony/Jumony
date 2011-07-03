using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// HTML 元素属性
  /// </summary>
  /// <remarks>
  /// 与XML和HTML标准DOM模型不同，在Jumony里面，Attribute不被认为是一个节点（不从IHtmlNode继承）。
  /// 这是因为IHtmlNode被定义为位置敏感的DOM对象，而Attribute是非位置敏感的（在元素中定义的顺序无关紧要）。
  /// 这与LINQ to XML的模型是一致的。
  /// </remarks>
  public interface IHtmlAttribute : IHtmlDomObject
  {

    /// <summary>
    /// 获取属性所属的元素
    /// </summary>
    IHtmlElement Element
    {
      get;
    }

    /// <summary>
    /// 属性名
    /// </summary>
    string Name
    {
      get;
    }

    /// <summary>
    /// 属性值
    /// </summary>
    string AttributeValue
    {
      get;
    }

  }
}
