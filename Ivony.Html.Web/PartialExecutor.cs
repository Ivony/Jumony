using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义部分视图执行程序
  /// </summary>
  public sealed class PartialExecutor
  {
    private ParameterInfo[] _parameters;

    /// <summary>
    /// 创建 PartialExecutor 对象
    /// </summary>
    /// <param name="method">用于处理分部视图的执行方法</param>
    public PartialExecutor( MethodInfo method )
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
      ParameterExpression handlerParamter = Expression.Parameter( typeof( object ), "handler" );
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


    private delegate string Executor( object host, object[] parameters );


    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; }


    private Executor _executor;

    /// <summary>
    /// 执行分部页获取渲染结果
    /// </summary>
    /// <param name="host">分部视图执行方法所属的对象</param>
    /// <param name="partialElement">定义分部页的元素</param>
    /// <returns></returns>
    public string Execute( object host, IHtmlElement partialElement )
    {

      object[] parameterValues = new object[_parameters.Length];

      foreach ( var parameter in _parameters )
      {
        var value = partialElement.Attribute( parameter.Name ).Value();
        if ( value != null )
          parameterValues[parameter.Position] = ConvertValue( value, parameter.ParameterType );
        else
          parameterValues[parameter.Position] = parameter.DefaultValue.IfNull( null );
      }

      return _executor( host, parameterValues );

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
