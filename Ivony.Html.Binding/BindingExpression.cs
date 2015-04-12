using Ivony.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Binding
{




  /// <summary>
  /// 定义绑定表达式，绑定表达式可以由属性值或者元素来定义。
  /// </summary>
  public abstract partial class BindingExpression : IBindingExpressionValueObject
  {


    /// <summary>
    /// 表达式名称
    /// </summary>
    public abstract string Name { get; }


    /// <summary>
    /// 派生类实现此属性提供参数值列表
    /// </summary>
    internal abstract IDictionary<string, IBindingExpressionValueObject> Arguments { get; }




    /// <summary>
    /// 获取所有参数的值
    /// </summary>
    /// <param name="evaluator">绑定表达式计算转换器 （一般是BindingContext）</param>
    /// <returns>所有参数和参数值</returns>
    public IDictionary<string, object> GetValues( IBindingExpressionEvaluator evaluator )
    {
      return Arguments.ToDictionary( pair => pair.Key, pair => pair.Value.GetValue( evaluator ) );
    }


    /// <summary>
    /// 尝试获取参数指定类型的值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="evaluator">绑定表达式计算转换器 （一般是BindingContext）</param>
    /// <param name="name">参数名称</param>
    /// <param name="value">获取到的参数值</param>
    /// <param name="throwIfConvertFailed">当类型转换失败的时候是否应当抛出异常</param>
    /// <returns>是否成功获取</returns>
    public bool TryGetValue<T>( IBindingExpressionEvaluator evaluator, string name, out T value, bool throwIfConvertFailed = true )
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
    /// 尝试获取参数值
    /// </summary>
    /// <param name="evaluator">绑定表达式计算转换器 （一般是BindingContext）</param>
    /// <param name="name">参数名称</param>
    /// <param name="value">获取到的参数值</param>
    /// <returns>是否成功获取</returns>
    public bool TryGetValue( IBindingExpressionEvaluator evaluator, string name, out object value )
    {

      if ( !Arguments.ContainsKey( name ) )
      {
        value = null;
        return false;
      }

      value = Arguments[name].GetValue( evaluator );
      return true;
    }


    /// <summary>
    /// 获取指定类型的值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="evaluator">绑定表达式计算转换器 （一般是BindingContext）</param>
    /// <param name="name">参数名称</param>
    /// <returns>是否成功获取</returns>
    public T GetValue<T>( IBindingExpressionEvaluator evaluator, string name )
    {
      T value;
      if ( !TryGetValue( evaluator, name, out value, true ) )
        throw new KeyNotFoundException();

      return value;
    }



    /// <summary>
    /// 解析绑定表达式
    /// </summary>
    /// <param name="expression">要从中解析的绑定表达式的字符串</param>
    /// <returns>解析后的结果</returns>
    public static BindingExpression ParseExpression( string expression )
    {
      return ParseExpression( expression, 0 );
    }

    /// <summary>
    /// 解析绑定表达式
    /// </summary>
    /// <param name="expression">要从中解析的绑定表达式的字符串</param>
    /// <param name="index">解析绑定表达式的开始位置</param>
    /// <returns>解析后的结果</returns>
    public static BindingExpression ParseExpression( string expression, int index )
    {

      if ( string.IsNullOrEmpty( expression ) )
        return null;

      if ( expression[index] != '{' )
        return null;

      if ( tokenizer == null )
        tokenizer = new BindingExpressionParser();

      return tokenizer.Parse( expression, index );

    }




    [ThreadStatic]
    private static BindingExpressionParser tokenizer;//设置为线程内单例，提高利用率。




    private class ParsedBindingExpression : BindingExpression
    {

      private string _name;
      private Dictionary<string, IBindingExpressionValueObject> _arguments;


      public ParsedBindingExpression( string name ) : this( name, new Dictionary<string, IBindingExpressionValueObject>() ) { }

      internal ParsedBindingExpression( string name, Dictionary<string, IBindingExpressionValueObject> arguments )
      {
        _name = name;
        _arguments = arguments;
      }



      public override string Name
      {
        get { return _name; }
      }

      internal override IDictionary<string, IBindingExpressionValueObject> Arguments
      {
        get { return _arguments; }
      }


      public override string ToString()
      {
        return string.Format( "{{{0} {1}}}", Name, string.Join( ",", Arguments.Select( pair => pair.Key + "=" + pair.Value ) ) );
      }

    }

    object IBindingExpressionValueObject.GetValue( IBindingExpressionEvaluator evaluator )
    {
      return evaluator.GetValue( this );
    }

  }
}
