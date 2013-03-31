using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 HTML 规范抽象基类
  /// </summary>
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



    /// <summary>
    /// 判断元素是否为块级元素
    /// </summary>
    /// <param name="element">需要判断的元素</param>
    /// <returns>是否为块级元素</returns>
    public abstract bool IsBlockElement( IHtmlElement element );

    /// <summary>
    /// 判断元素是否为行内元素
    /// </summary>
    /// <param name="element">需要判断的元素</param>
    /// <returns>是否为行内元素</returns>
    public abstract bool IsInlineElement( IHtmlElement element );

    /// <summary>
    /// 判断元素是否为特殊元素
    /// </summary>
    /// <param name="element">需要判断的元素</param>
    /// <returns>是否为特殊元素</returns>
    public abstract bool IsSpecialElement( IHtmlElement element );

    /// <summary>
    /// 判断元素是否为表单输入元素
    /// </summary>
    /// <param name="element">需要判断的元素</param>
    /// <returns>是否为表单输入元素</returns>
    public abstract bool IsFormInputElement( IHtmlElement element );

    /// <summary>
    /// 判断元素是否为样式设置元素
    /// </summary>
    /// <param name="element">需要判断的元素</param>
    /// <returns>是否为样式设置元素</returns>
    public abstract bool IsStylingElement( IHtmlElement element );




    /// <summary>
    /// 确认元素文本内容格式
    /// </summary>
        /// <summary>
    /// 判断元素是否为列表定义��断的元素</param>
    /// <returns>是否为特殊元素</returns>
    public abstract bool IsSpecialElement( 样式设置元素</returns>
    public abstract bool IsList内容格式
    /// </summary>
    //    /// <summary>
    /// 判断一个属性值的值是否应被视为URI。
    /// </summary>
    /// <param name="attribute">要检查的属性</param>
    /// <returns>其值是否应被视为URI</returns>
    public abstract bool IsUriValue( IHtmlAttribute attribute );

    /// <summary>
    /// 判断一个属性值的值是否应被视为脚本。
    /// </summary>
    /// <param name="attribute">要检查的属性</param>
    /// <returns>其值是否应被视为脚本</returns>
    public abstract bool IsScriptValue( IHtmlAttribute attribute );

    /// <summary>
    /// 判断一个属性是否为标记属性。
    /// </summary>
    /// <param name="attribute">要检查的属性</param>
    /// <returns>是否为标记属性</returns>
    public abstract bool IsMarkupValue( IHtmlAttribute attribute );   /// <param name="element">要确认文本内容格式的元素</param>
    /// <returns>文本内容格式</returns>
    public abstract TextMode ElementTextMode( IHtmlElement element );


    /// <summary>
    /// 获取与之相匹配的 CSS 样式规范
    /// </summary>
    /// <returns></returns>
    public abstract CssStyleSpecificationBase GetCssStyleSpecification();

  }


  /// <summary>
  /// 文本模式
  /// </summary>
  public enum TextMode
  {
    /// <summary>普通，内容当作 HTML 来解释</summary>
    Normal,
    /// <summary>CData，内容当作文本来解释</summary>
    CData,
    /// <summary>预格式化，不合并空白字符</summary>
    Preformated,
    /// <summary>非文本，元素没有文本内容</summary>
    NonText
  }


}
