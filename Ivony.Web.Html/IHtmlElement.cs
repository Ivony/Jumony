using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ivony.Web.Html
{
  public interface IHtmlElement : IHtmlContainer
  {

    string Name
    {
      get;
    }

    IEnumerable<IHtmlAttribute> Attributes();

    /// <summary>
    /// 提供数据绑定的核心方法。不应从代码中直接调用，而应该使用Bind扩展方法。
    /// </summary>
    /// <param name="context">绑定上下文</param>
    /// <param name="path">绑定路径</param>
    /// <param name="value">绑定值</param>
    /// <param name="nullMode">为空时执行的操作</param>
    [EditorBrowsable( EditorBrowsableState.Never )]
    void BindCore( HtmlBindingContext context, string path, string value, BindingNullBehavior nullBehavior );

    /// <summary>
    /// 添加一个属性
    /// </summary>
    /// <param name="attributeName">属性名</param>
    /// <returns>添加的属性</returns>
    IHtmlAttribute AddAttribute( string attributeName );
  }
}
