using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{
  /// <summary>
  /// 可使用母板页的内容视图
  /// </summary>
  public interface IContentView : IView
  {

    void InitializeMaster( IMasterView master );

    IHtmlRenderAdapter CreateContentAdapter( IMasterView master );
  }
}
