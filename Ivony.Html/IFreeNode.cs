using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ivony.Html
{

  /// <summary>
  /// 定义一个游离的，尚未被分配的 HTML 节点，稍候可以将该节点插入到文档合适位置
  /// </summary>
  public interface IFreeNode : IHtmlNode
  {
    [EditorBrowsable( EditorBrowsableState.Never )]
    IHtmlNode Into( IHtmlContainer container, int index );
    IHtmlNodeFactory Factory { get; }

    //bool CanInsertTo( IHtmlContainer container );
  }


  /// <summary>定义一个游离的，尚未被分配的 HTML 元素</summary>
  public interface IFreeElement : IHtmlElement, IFreeNode { }
  /// <summary>定义一个游离的，尚未被分配的 HTML 文本节点</summary>
  public interface IFreeTextNode : IHtmlTextNode, IFreeNode { }
  /// <summary>定义一个游离的，尚未被分配的 HTML 注释或应当被忽略的 HTML 内容</summary>
  public interface IFreeComment : IHtmlComment, IFreeNode { }
}
