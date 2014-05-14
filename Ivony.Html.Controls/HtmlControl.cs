using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Controls
{

  /// <summary>
  /// 定义 HTML 控件
  /// </summary>
  public abstract class HtmlControl
  {

    internal void ProcessControl( IHtmlElement element, IHtmlRenderContext renderContext )
    {

      HtmlElement = element;

      Init();

      Load();

      RenderCore( renderContext );

      Unload();

    }

    protected IHtmlElement HtmlElement { get; private set; }


    protected virtual void Init()
    {
    }


    protected virtual void Load()
    {
    }


    protected virtual void RenderCore( IHtmlRenderContext renderContext )
    {
    }


    protected virtual void Unload()
    {
    }

  }
}
