using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{
  
  /// <summary>
  /// 代表一个 HTML 文档内容片段
  /// </summary>
  public class HtmlContentFragment
  {

    /// <summary>
    /// 创建 HtmlContentFragment 对象
    /// </summary>
    /// <param name="reader">HTML 读取分析器</param>
    /// <param name="startIndex">开始位置</param>
    /// <param name="length">内容长度</param>
    public HtmlContentFragment( IHtmlReader reader, int startIndex, int length )
    {

      if ( reader == null )
        throw new ArgumentNullException( "reader" );

      if ( startIndex < 0 )
        throw new ArgumentOutOfRangeException( "startIndex" );

      if ( length <= 0 )
        throw new ArgumentOutOfRangeException( "length" );


      Reader = reader;
      StartIndex = startIndex;
      Length = length;


    }


    internal HtmlContentFragment( HtmlContentFragment info )
    {
      Reader = info.Reader;
      StartIndex = info.StartIndex;
      Length = info.Length;
    }


    /// <summary>
    /// 创建内容片段的读取分析器
    /// </summary>
    public IHtmlReader Reader
    {
      get;
      private set;
    }

    
    /// <summary>
    /// 内容片段的开始位置
    /// </summary>
    public int StartIndex
    {
      get;
      private set;
    }

    
    /// <summary>
    /// 内容片段的长度
    /// </summary>
    public int Length
    {
      get;
      private set;
    }


    /// <summary>
    /// HTML 内容
    /// </summary>
    public string Html
    {
      get { return Reader.HtmlText.Substring( StartIndex, Length ); }
    }

    
    /// <summary>
    /// 获取 HTML 内容
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return Html;
    }

  }
}
