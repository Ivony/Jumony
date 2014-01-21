using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 处理文本替换的绑定器
  /// </summary>
  public sealed class LiteralBinder : IHtmlBinder
  {

    /// <summary>
    /// 文本替换属性前缀
    /// </summary>
    public static readonly string literalNamespace = "literal:";

    /// <summary>
    /// 绑定元素
    /// </summary>
    /// <param name="context">当前绑定上下文</param>
    /// <param name="element">要绑定的元素</param>
    /// <returns>是否继续进行后面的绑定</returns>
    public bool BindElement( HtmlBindingContext context, IHtmlElement element )
    {

      if ( !element.Document.HtmlSpecification.IsCDataElement( element.Name ) )
        return false;

      lock ( element.SyncRoot )
      {
        var attributes = element.Attributes().Where( a => a.Name.StartsWith( literalNamespace ) );
        var dictionary = attributes.ToDictionary( a => a.Name.Substring( literalNamespace.Length ), a => a.Value() );

        BindElement( element, dictionary );

        attributes.Remove();

      }
      return false;
    }

    private void BindElement( IHtmlElement element, Dictionary<string, string> dictionary )
    {
      var text = element.InnerHtml();

      foreach ( var pair in dictionary )
        text = text.Replace( string.Format( "#{0}#", pair.Key ), pair.Value );


      element.InnerHtml( text );
    }

  }
}
