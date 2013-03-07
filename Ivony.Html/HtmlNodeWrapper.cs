using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  /// <summary>
  /// IHtmlNode 的包裹类
  /// </summary>
  public abstract class HtmlNodeWrapper : IHtmlNode
  {

    /// <summary>
    /// 被包装的 IHtmlNode 对象
    /// </summary>
    protected abstract IHtmlNode Node
    {
      get;
    }

    #region IHtmlNode 成员

    IHtmlContainer IHtmlNode.Container
    {
      get { return Node.Container; }
    }

    object IHtmlDomObject.RawObject
    {
      get { return Node.RawObject; }
    }

    IHtmlDocument IHtmlDomObject.Document
    {
      get { return Node.Document; }
    }

    string IHtmlDomObject.RawHtml
    {
      get { return Node.RawHtml; }
    }

    #endregion


    /// <summary>
    /// 用作特定类型的哈希函数。
    /// </summary>
    /// <returns>当前对象的哈希代码。</returns>
    public override int GetHashCode()
    {
      return Node.GetHashCode();
    }


    /// <summary>
    /// 确定指定的 System.Object 是否等于当前的 System.Object。
    /// </summary>
    /// <param name="obj">与当前的 System.Object 进行比较的 System.Object。</param>
    /// <returns>如果指定的 System.Object 等于当前的 System.Object，则为 true；否则为 false。</returns>
    public override bool Equals( object obj )
    {
      return Node.Equals( obj );
    }


  }
}
