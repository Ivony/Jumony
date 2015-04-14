using Ivony.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 处理{eval xxx}表达式的绑定器
  /// </summary>
  /// <example>
  /// &lt;a href="{eval path=Link}"&gt;Link&lt;/a&gt;;
  /// </example>
  public class EvalExpressionBinder : IExpressionBinder, IElementExpressionBinder
  {
    string IExpressionBinder.ExpressionName
    {
      get { return "Eval"; }
    }


    /// <summary>
    /// 根据绑定参数获取数据对象
    /// </summary>
    /// <param name="context">绑定上下文</param>
    /// <param name="expression">绑定表达式</param>
    /// <returns>数据对象</returns>
    public static object GetDataObject( HtmlBindingContext context, BindingExpression expression )
    {
      object dataObject = context.DataModel;




      if ( dataObject != null )
      {
        string path;

        if ( expression.TryGetValue( context, "path", out path ) )
        {

          if ( dataObject == null )
            return null;

          dataObject = DataBinder.Eval( dataObject, path );
        }
      }

      object value;
      if ( expression.TryGetValue( context, "value", out value ) )
      {
        if ( dataObject != null && Convert.ToBoolean( dataObject ) == true )
          return value;

        else if ( expression.TryGetValue( context, "alternativeValue", out value ) )
          return value;

        else
          return null;
      }

      return dataObject;
    }



    /// <summary>
    /// 获取绑定值
    /// </summary>
    /// <param name="context">绑定上下文</param>
    /// <param name="expression">绑定表达式</param>
    /// <returns>绑定值</returns>
    public static object GetValue( HtmlBindingContext context, BindingExpression expression )
    {

      var dataObject = GetDataObject( context, expression );

      if ( dataObject == null )
        return null;


      string format;
      string formattedValue = null;
      if ( expression.TryGetValue( context, "format", out format ) )
      {
        if ( format.Contains( "{0" ) )
          formattedValue = string.Format( CultureInfo.InvariantCulture, format, dataObject );

        else
        {
          var formattable = dataObject as IFormattable;

          if ( formattable != null )
            formattedValue = formattable.ToString( format, CultureInfo.InvariantCulture );

          else
            formattedValue = dataObject.ToString();
        }
      }

      if ( expression.Arguments.ContainsKey( "encoded" ) )
      {

        if ( formattedValue != null )
          return new HtmlString( formattedValue );

        else
          return new HtmlString( dataObject.ToString() );
      }


      return formattedValue ?? dataObject;

    }

    /// <summary>
    /// 尝试格式化绑定值
    /// </summary>
    /// <param name="dataObject">要进行格式化的数据对象</param>
    /// <param name="context">当前绑定上下文</param>
    /// <param name="expression">绑定表达式</param>
    /// <returns>若绑定表达式定义了 format 参数，则返回格式化后的结果，否则返回 null</returns>
    protected static string TryFormatValue( object dataObject, HtmlBindingContext context, BindingExpression expression )
    {

      string format;
      if ( !expression.TryGetValue( context, "format", out format ) )
        return null;


      if ( format == null )
        return dataObject.ToString();


      if ( format.Contains( "{0" ) )
        return string.Format( CultureInfo.InvariantCulture, format, dataObject );


      var formattable = dataObject as IFormattable;

      if ( formattable != null )
        return formattable.ToString( format, CultureInfo.InvariantCulture );

      else
        return dataObject.ToString();
    }



    object IExpressionBinder.GetValue( HtmlBindingContext context, BindingExpression expression )
    {
      return GetValue( context, expression );
    }
  }
}
