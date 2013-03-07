using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// 一个特殊的 DOM 节点，默认情况下，特殊 DOM 节点是指用尖括号括起，且首尾第一个字符为"!@%$#"这些特殊字符的标签。
  /// </summary>
  public class DomSpecial : DomNode, IHtmlSpecial, IHtmlTextNode
  {

    private string raw;

    /// <summary>
    /// 创建 DomSpecial 实例
    /// </summary>
    /// <param name="rawHtml"></param>
    public DomSpecial( string rawHtml )
    {
      raw = rawHtml;
    }

    /// <summary>
    /// 原始的 HTML 文本，如果有的话
    /// </summary>
    public override string RawHtml
    {
      get
      {
        return raw;
      }
    }


    /// <summary>
    /// 在渲染时应输出什么。
    /// </summary>
    /// <returns></returns>
    public string EvaluateHtml()
    {
      CheckDisposed();

      return raw;
    }


    /// <summary>
    /// 对象名
    /// </summary>
    protected override string ObjectName
    {
      get { return "Special Node"; }
    }

    #region IHtmlTextNode 成员

    string IHtmlTextNode.HtmlText
    {
      get { return raw; }
    }

    #endregion

  }




}
