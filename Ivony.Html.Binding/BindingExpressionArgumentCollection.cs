using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 定义绑定表达式参数列表
  /// </summary>
  public sealed class BindingExpressionArgumentCollection
  {

    private bool _completed = false;
    private object _sync = new object();
    private Hashtable _dictionary = new Hashtable();


    internal BindingExpressionArgumentCollection() { }

    /// <summary>
    /// 创建 BindingExpressionArgumentCollection 对象
    /// </summary>
    /// <param name="dictionary">包含参数值列表的字典</param>
    public BindingExpressionArgumentCollection( IDictionary<string, object> dictionary )
    {
      foreach ( var pair in dictionary )
      {
        var name = pair.Key;
        var value = pair.Value;


        var expression = value as BindingExpression;
        if ( expression != null )
          Add( name, expression );

        if ( value == null )
          Add( name );

        var str = value as string;
        if ( str == null )
          throw new InvalidOperationException();

        Add( name, str );
      }

      _completed = true;
    }

    internal void Add( string name, string value = null )
    {
      if ( _completed )
        throw new InvalidOperationException();

      _dictionary.Add( name, value );
    }

    internal void Add( string name, BindingExpression expression )
    {
      if ( _completed )
        throw new InvalidOperationException();

      if ( expression == null )
      {
        Add( name, (string) null );
        return;
      }


      _dictionary.Add( name, expression );
    }


    internal object this[string name]
    {
      get { return _dictionary[name]; }
    }


    internal void SetCompleted()
    {
      _completed = true;
    }

    public object SyncRoot
    {
      get { return _sync; }
    }


    /// <summary>
    /// 尝试获取指定类型的值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="evaluator">绑定表达式计算转换器 （一般是BindingContext）</param>
    /// <param name="name">参数名称</param>
    /// <param name="value">获取到的参数值</param>
    /// <returns>是否成功获取</returns>
    public bool TryGetValue<T>( IBindingExpressionEvaluator evaluator, string name, out T value, bool throwIfConvertFailed = false )
    {
      if ( typeof( BindingExpression ).IsAssignableFrom( typeof( T ) ) )
        throw new InvalidOperationException();


      object obj;

      if ( !TryGetValue( evaluator, name, out obj ) )   //尝试获取值，若不成功直接返回 false
      {
        value = default( T );
        return false;
      }



      if ( evaluator.TryConvertValue( obj, out value ) )//尝试转换值类型，若成功直接返回 true
        return true;


      if ( throwIfConvertFailed )                       //若值类型转换失败，且需要抛出异常
      {
        if ( obj == null )
          throw new InvalidCastException( string.Format( "无法将 null 值转换为类型为 {0} 的对象，找不到合适的类型转换器。", typeof( T ).AssemblyQualifiedName ) );

        else
          throw new InvalidCastException( string.Format( "无法将类型为 {0} 的对象转换为类型为 {1} 的对象，不存在类型转换或找不到合适的类型转换器。", obj.GetType().AssemblyQualifiedName, typeof( T ).AssemblyQualifiedName ) );
      }


      return false;
    }


    /// <summary>
    /// 尝试获取参数原始值对象
    /// </summary>
    /// <param name="evaluator">绑定表达式计算转换器 （一般是BindingContext）</param>
    /// <param name="name">参数名称</param>
    /// <param name="value">获取到的参数值</param>
    /// <returns>是否成功获取</returns>
    public bool TryGetValue( IBindingExpressionEvaluator evaluator, string name, out object value )
    {

      if ( !_dictionary.ContainsKey( name ) )
      {
        value = null;
        return false;
      }


      var rawObject = _dictionary[name];
      var expression = rawObject as BindingExpression;

      if ( expression != null )
        value = evaluator.GetValue( (BindingExpression) rawObject );


      else
        value = (string) rawObject;


      return true;

    }


    /// <summary>
    /// 获取指定类型的值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="evaluator">绑定表达式计算转换器 （一般是BindingContext）</param>
    /// <param name="name">参数名称</param>
    /// <param name="value">获取到的参数值</param>
    /// <returns>是否成功获取</returns>
    public T GetValue<T>( IBindingExpressionEvaluator evaluator, string name )
    {
      T value;
      if ( !TryGetValue( evaluator, name, out value, true ) )
        throw new ArgumentException( string.Format( "找不到名为 {0} 的参数值", name ) );

      return value;
    }

  }
}
