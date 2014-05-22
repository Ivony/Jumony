using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ivony.Html
{


  /// <summary>
  /// 定义 HTML 渲染上下文
  /// </summary>
  public interface IHtmlRenderContext
  {

    /// <summary>
    /// 用于输出渲染的文本输出器
    /// </summary>
    TextWriter Writer { get; }

    /// <summary>
    /// 获取当前上下文中所有的渲染代理
    /// </summary>
    IHtmlRenderAdapter[] RenderAdapters { get; }

  }

  /// <summary>
  /// HTML 渲染上下文
  /// </summary>
  public class HtmlRenderContext : IHtmlRenderContext
  {

    internal HtmlRenderContext( TextWriter writer, IHtmlRenderAdapter[] adapters )
    {

      if ( writer == null )
        throw new ArgumentNullException( "writer" );

      Writer = writer;
      RenderAdapters = adapters ?? new IHtmlRenderAdapter[0];
      Data = Hashtable.Synchronized( new Hashtable() );
    }


    /// <summary>
    /// 获取本次渲染所使用的渲染代理
    /// </summary>
    public IHtmlRenderAdapter[] RenderAdapters { get; private set; }

    /// <summary>
    /// 用于输出渲染的文本输出器
    /// </summary>
    public TextWriter Writer { get; private set; }


    /// <summary>
    /// 获取当前渲染上下文数据容器，这些数据仅在这一次上下文中有效
    /// </summary>
    public Hashtable Data { get; private set; }


  }
}
