using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ivony.Html
{

  /// <summary>
  /// 定义一种 HTML 转换器，其可以自定义 HTML 节点的渲染规则。
  /// </summary>
  public interface IHtmlRenderAdapter
  {

    /// <summary>
    /// 尝试呈现指定的节点
    /// </summary>
    /// <param name="node">要渲染的节点</param>
    /// <param name="context">当前渲染上下文</param>
    /// <returns>是否进行了自定义渲染，若返回false，则使用默认渲染</returns>
    /// <remarks>
    /// 若返回值为 false ，则系统其后会自行渲染这个节点，所以不应对 writer 写入一些东西后返回 false。
    /// </remarks>
    bool Render( IHtmlNode node, IHtmlRenderContext context );

  }
}
