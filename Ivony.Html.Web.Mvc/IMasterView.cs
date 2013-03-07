using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web
{
  
  /// <summary>
  /// 定义母板视图
  /// </summary>
  public interface IMasterView
  {

    /// <summary>
    /// 初始化母板视图
    /// </summary>
    /// <param name="context">视图上下文</param>
    void Initialize( ViewContext context );

    /// <summary>
    /// 渲染母板视图
    /// </summary>
    /// <param name="view">内容视图</param>
    /// <returns>渲染结果</returns>
    string Render( IContentView view );

  }
}
