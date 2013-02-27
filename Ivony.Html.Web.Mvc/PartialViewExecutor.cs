using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
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


      _executor = CreateExecutor( method );

    }

    private Executor CreateExecutor( MethodInfo methodInfo )
    {
      ParameterExpression handlerParamter = Expression.Parameter( typeof( JumonyViewHandler ), "handler" );
      ParameterExpression argsParameter = Expression.Parameter( typeof( object[] ), "parameters" );


      List<Expression> list = new List<Expression>();
      ParameterInfo[] parameters = methodInfo.GetParameters();
      for ( int i = 0; i < parameters.Length; i++ )
      {
        ParameterInfo parameterInfo = parameters[i];
        BinaryExpression expression = Expression.ArrayIndex( argsParameter, Expression.Constant( i ) );
        UnaryExpression item = Expression.Convert( expression, parameterInfo.ParameterType );
        list.Add( item );
      }


      UnaryExpression instance = Expression.Convert( handlerParamter, methodInfo.ReflectedType );
      MethodCallExpression methodCallExpression = Expression.Call( instance, methodInfo, list );


      Expression<Executor> result = Expression.Lambda<Executor>( methodCallExpression, new ParameterExpression[] { handlerParamter, argsParameter } );
      return result.Compile();
    }


    private delegate string Executor( JumonyViewHandler handler, object[] parameters );

    public string Name { get; private set; }


    private Executor _executor;

    public string Execute( JumonyViewHandler handler, IHtmlElement partialElement )
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

      return _executor( handler, parameterValues );

    }

    private object ConvertValue( string value, Type type )
    {
      var converter = TypeDescriptor.GetConverter( type );
      if ( !converter.CanConvertFrom( typeof( string ) ) )
        throw new InvalidOperationException( string.Format( "无法将参数从字符串转换为 {0} 类型", type.FullName ) );

      return converter.ConvertTo( value, type );
    }
  }
}
