using Ivony.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

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
    /// <param name="arguments">绑定参数</param>
    /// <returns>数据对象</returns>
    public static object GetDataObject( HtmlBindingContext context, BindingExpression expression )
    {
      object dataObject;

      dataObject = context.DataModel;

      {
        string path;

        if ( expression.TryGetValue( context, "path", out path ) )
        {
          
          if ( dataObject == null )
            return null;

          dataObject = DataBinder.Eval( dataObject, path );
        }
      }

      {
        object value;
        if ( expression.TryGetValue( context, "value", out value ) )
        {
          if ( Convert.ToBoolean( dataObject ) )
            return value;

          else if ( expression.TryGetValue( context, "alternativeValue", out value ) )
            return value;

          else
            return null;
        }
      }



      return dataObject;
    }



    /// <summary>
    /// 获取绑定值
    /// </summary>
    /// <param name="context">绑定上下文</param>
    /// <param name="arguments">绑定参数</param>
    /// <returns>绑定值</returns>
    public static string GetValue( HtmlBindingContext context, BindingExpression expression )
    {

      var dataObject = GetDataObject( context, expression );

      if ( dataObject == null )
        return null;

      {
        string format;
        if ( expression.TryGetValue( context, "format", out format ) )
        {

          if ( format.Contains( "{0" ) )
            return string.Format( CultureInfo.InvariantCulture, format, dataObject );

          else
          {
            var formattable = dataObject as IFormattable;

            if ( formattable != null )
              return formattable.ToString( format, CultureInfo.InvariantCulture );

            else
              return dataObject.ToString();
          }
        }
      }



      return dataObject.ToString();

    }




    object IExpressionBinder.GetValue( HtmlBindingContext context, BindingExpression expression )
    {
      return GetValue( context, expression );
    }
  }
}
