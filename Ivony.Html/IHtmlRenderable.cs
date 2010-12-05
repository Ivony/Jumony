using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ivony.Html
{
  public interface IHtmlRenderableNode : IHtmlNode
  {

    void Render( TextWriter writer );

  }
}
