using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{

  /// <summary>
  /// 描述一个 HTML 开始标签
  /// </summary>
  public sealed class HtmlBeginTag : HtmlContentFragment
  {


    /// <summary>
    /// 创建一个 HtmlBeginTag 对象
    /// </summary>
    /// <param name="info">应当被认为是 HTML 结束标签的 HTML 片段</param>
    /// <param name="tagName">标签名</param>
    /// <param name="selfClosed">是否自结束标签</param>
    /// <param name="attibutes">属性设置</param>
    public HtmlBeginTag( HtmlContentFragment info, string tagName, bool selfClosed, IEnumerable<HtmlAttributeSetting> attibutes )
      : base( info )
    {
      TagName = tagName;
      Attributes = attibutes;
      SelfClosed = selfClosed;
    }

    /// <summary>
    /// 标签名
    /// </summary>
    public string TagName
    {
      get;
      private set;
    }


    /// <summary>
    /// 是否为自结束标签
    /// </summary>
    public bool SelfClosed
    {
      get;
      private set;
    }


    /// <summary>
    /// 标签的属性设置
    /// </summary>
    public IEnumerable<HtmlAttributeSetting> Attributes
    {
      get;
      private set;
    }


  }


}
