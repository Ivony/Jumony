using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// IHtmlTextNode 的一个实现
  /// </summary>
  public class DomTextNode : DomNode, IHtmlTextNode
  {

    private readonly string raw;

    /// <summary>
    /// 创建 DomTextNode 对象
    /// </summary>
    /// <param name="rawHtml"></param>
    public DomTextNode( string rawHtml )
    {
      raw = rawHtml;
    }


    /// <summary>
    /// 获取一个名称，用于识别节点类型，此属性总是返回 "TextNode"
    /// </summary>
    protected override string ObjectName
    {
      get { return "TextNode"; }
    }



    /// <summary>
    /// 获取文本节点所代表的 HTML 文本
    /// </summary>
    public string HtmlText
    {
      get
      {
        var element = this.Parent();
        if ( element != null && Document.HtmlSpecification.IsCDataElement( element.Name ) )
          return raw;



        return HtmlEncoding.HtmlEncode( HtmlEncoding.HtmlDecode( raw ) );
      }
    }


    /// <summary>
    /// 获取原始的 HTML 文本
    /// </summary>
    public override string RawHtml
    {
      get
      {
        return raw;
      }
    }
  }
}
