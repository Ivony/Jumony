using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  /// <summary>
  /// HTML 渲染上下文
  /// </summary>
  public class HtmlRenderContext
  {

    internal HtmlRenderContext( TextWriter writer, IHtmlRenderAdapter[] adapters )
    {

      if ( writer == null )
        throw new ArgumentNullException( "writer" );

      Writer = writer;
      Adapters = adapters ?? new IHtmlRenderAdapter[0];
      Data = Hashtable.Synchronized( new Hashtable() );
    }

    internal IHtmlRenderAdapter[] Adapters { get; private set; }

    /// <summary>
    /// 用于输出渲染的文本输出器
    /// </summary>
    public TextWriter Writer { get; private set; }


    /// <summary>
    /// 获取当前渲染上下文数据容器，这些数据仅在这一次上下文中有效
    /// </summary>
    public Hashtable Data { get; private set; }


    /// <summary>
    /// 将对象直接写入渲染输出
    /// </summary>
    /// <param name="value">要写入渲染输出的对象</param>
    public void Write( object value )
    {
      Writer.Write( value );
    }


    /// <summary>
    /// 将字符串直接写入渲染输出
    /// </summary>
    /// <param name="value">要写入渲染输出的对象</param>
    public void Write( string value )
    {
      Writer.Write( value );
    }
  }
}
