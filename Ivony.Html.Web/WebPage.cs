using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ivony.Html.Web
{
  
  /// <summary>
  /// 代表一个 Web 页面，其包含一个HTML文档和URI地址，以及缓存信息。
  /// </summary>
  public class WebPage
  {

    /// <summary>
    /// 创建一个 Web 页面
    /// </summary>
    /// <param name="document">HTML 文档</param>
    /// <param name="url">文档地址</param>
    /// <param name="cacheKey">文档缓存键</param>
    public WebPage( IHtmlDocument document, Uri url, string cacheKey )
    {
      Document = document;
      Url = url;
      CacheKey = cacheKey;
    }


    /// <summary>
    /// HTML文档
    /// </summary>
    public IHtmlDocument Document
    {
      private set;
      get;
    }

    /// <summary>
    /// 文档地址
    /// </summary>
    public Uri Url
    {
      private set;
      get;
    }

    /// <summary>
    /// 文档缓存键
    /// </summary>
    public string CacheKey
    {
      private set;
      get;
    }



    /// <summary>
    /// 将文档呈现为HTML格式
    /// </summary>
    /// <returns>呈现的HTML</returns>
    public string Render()
    {
      using ( var writer = new StringWriter() )
      {
        Render( writer );
        return writer.ToString();
      }
    }


    /// <summary>
    /// 将文档呈现为HTML格式
    /// </summary>
    /// <param name="writer">用于写入HTML文本的编写器</param>
    public virtual void Render( TextWriter writer )
    {
      Document.Render( writer );
    }


    /// <summary>
    /// 在文档中查找符合选择器要求的元素
    /// </summary>
    /// <param name="selector">选择器</param>
    /// <returns>符合要求的元素</returns>
    public IEnumerable<IHtmlElement> Find( string selector )
    {
      return Document.Find( selector );
    }

  }
}
