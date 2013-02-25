using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Web
{
  public class PartialViewExecutor
  {
    private MethodInfo method;

    private ParameterInfo[] _parameters;

    public PartialViewExecutor( MethodInfo method )
    {
      var name = method.Name;
      if ( !name.StartsWith( PartialRenderAdapter.partialExecutorMethodPrefix ) )
        throw new InvalidOperationException();
      Name = name.Substring( PartialRenderAdapter.partialExecutorMethodPrefix.Length );
      _parameters = method.GetParameters();
    }

    public string Name { get; private set; }


    private Func<JumonyView, object[], string> _executor;

    public string Execute( JumonyView view, IHtmlElement partialElement )
    {

      object[] parameterValues = new object[_parameters.Length];

      foreach ( var parameter in _parameters )
      {
        var value = partialElement.Attribute( parameter.Name ).Value();
        if ( value != null )
          parameterValues[parameter.Position] = ConvertValue( value, parameter.ParameterType );
        else if ( parameter.DefaultValue != null )
          parameterValues[parameter.Position] = parameter.DefaultValue;
      }

      return _executor( view, parameterValues );

    }

    private object ConvertValue( string value, Type type )
    {
      throw new NotImplementedException();
    }

  }
}
