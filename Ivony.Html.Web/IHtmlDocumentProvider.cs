using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace Ivony.Html.Web
{
 
  /// <summary>
  /// IHtmlDocument 对象创建适配器
  /// </summary>
  public interface IHtmlDocumentProvider
  {

    /// <summary>
    /// 创建 IHtmlDocument 对象
    /// </summary>
    /// <returns>创建的文档对象</returns>
    IHtmlDocument CreateDocument();

  }


}
