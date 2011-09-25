using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ivony.Fluent;

namespace Ivony.Html.Web.Mvc
{
  /// <summary>
  /// IMvcCachePolicyProvider 的一个标准抽象实现
  /// </summary>
  public abstract class MvcCachePolicyProvider : IMvcCachePolicyProvider
  {

    private static readonly string typeNamePostfix = "CachePolicyProvider";


    protected MvcCachePolicyProvider()
    {
      type = GetType();
      if ( !type.Name.EndsWith( typeNamePostfix ) )
        throw new InvalidOperationException( "派生类类名不正确，应以 CachePolicyProvider 结尾" );

      controllerName = type.Name.Substring( 0, type.Name.Length - typeNamePostfix.Length );


    }

    private Type type;
    private string controllerName;


    public CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {
      if ( !action.ControllerDescriptor.ControllerName.EqualsIgnoreCase( controllerName ) )
        return null;

      var _action = FindAction( action );

      throw new NotImplementedException();
    }

    private object FindAction( ActionDescriptor action )
    {
      throw new NotImplementedException();
    }







  }
}
