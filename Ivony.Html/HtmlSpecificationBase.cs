using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public abstract class HtmlSpecificationBase
  {

    /// <summary>
    /// 确认一个元素是否为 CData 元素
    /// </summary>
    /// <remarks>
    /// 对于 CData 元素，解析器不会将开始标签到结束标签之间的任何符号视为 HTML 符号的一部分。
    /// </remarks>
    /// <param name="elementName">元素名</param>
    /// <returns>是否为 CData 元素</returns>
    public abstract bool IsCDataElement( string elementName );

    /// <summary>
    /// 确认一个元素是否拥有可选的结束标签
    /// </summary>
    /// <param name="elementName">元素名</param>
    /// <returns>是否拥有可选的结束标签</returns>
    public abstract bool IsOptionalEndTag( string elementName );

    /// <summary>
    /// 确认一个元素是否不能拥有结束标签和子元素
    /// </summary>
    /// <param name="elementName">元素名</param>
    /// <returns>是否不能拥有结束标签和子元素</returns>
    public abstract bool IsForbiddenEndTag( string elementName );

    /// <summary>
    /// 检查可选结束标签在当前位置是否需要立即关闭
    /// </summary>
    /// <param name="openTag">当前开放的可选结束标签</param>
    /// <param name="nextTag">HTML 分析器遇到的下一个标签</param>
    /// <returns>是否需要立即关闭</returns>
    public abstract bool ImmediatelyClose( string openTag, string nextTag );




    public abstract bool IsBlockElement( IHtmlElement element );
    public abstract bool IsInlineElement( IHtmlElement element );
    public abstract bool IsSpecialElement( IHtmlElement element );
    public abstract bool IsFormInputElement( IHtmlElement element );
    public abstract bool IsStylingElement( IHtmlElement element );




    /// <summary>
    /// 确认元素文本内容格式
    /// </summary>
    /// <param name="element">要确认文本内容格式的元素</param>
    /// <returns>文本内容格式</returns>
    public abstract TextMode ElementTextMode( IHtmlElement element );

  }

  public enum TextMode
  {
    Normal,
    CData,
    Freformated
  }


}
