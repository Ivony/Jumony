using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Web.Binding
{

  public class BindingExpressionBuilder
  {


    private static readonly Regex bindingExpressionRegex = new Regex( "^" + Regulars.BindingExpressionPattern + "$", RegexOptions.IgnoreCase | RegexOptions.Compiled );

    /// <summary>
    /// 通过属性绑定表达式创建绑定对象
    /// </summary>
    /// <param name="attribute">HTML 属性对象</param>
    /// <returns>如果属性值可以被解析为绑定表达式，返回绑定对象</returns>
    public IBinding CreateBinding( IHtmlAttribute attribute )
    {
      var expression = attribute.Value();

      if ( expression == null )
        return null;

      var match = bindingExpressionRegex.Match( expression );
      if ( match == null || !match.Success )
        return null;

      return CreateBinding( match );

    }

    private IBinding CreateBinding( Match match )
    {
      var args = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );

      foreach ( Capture capture in match.Groups["args"].Captures )
      {
        var name = capture.FindCaptures( match.Groups["name"] ).FirstOrDefault().Value;
        var value = capture.FindCaptures( match.Groups["value"] ).FirstOrDefault().Value;

        args[name] = value;
      }

      return CreateBinding( args );

    }

    private IBinding CreateBinding( Dictionary<string, string> args )
    {
      throw new NotImplementedException();
    }
  }
}
