using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{

  /// <summary>
  /// DOCTYPE 声明节点
  /// </summary>
  public sealed class HtmlDoctypeDeclaration : HtmlContentFragment
  {

    public HtmlDoctypeDeclaration( HtmlContentFragment info ) : base( info ) { }

  }
}
