using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Ivony.Html.Web.Binding
{

  /// <summary>
  /// 处理{Binding xxx}表达式的绑定器
  /// </summary>
  public class BindingExpressionBinder : IDataContextExpressionBinder, IElementExpressionBinder
  {
    string IExpressionBinder.ExpressionName
    {
      get { return "Binding"; }
    }


    /// <summary>
    /// 根据绑定参数获取数据对象
    /// </summary>
    /// <param name="context">绑定上下文</param>
    /// <param name="arguments">绑定参数</param>
    /// <returns>数据对象</returns>
    public static object GetDataObject( HtmlBindingContext context, IDictionary<string, string> arguments )
    {
      string key;
      object dataObject;

      if ( arguments.TryGetValue( "key", out key ) || arguments.TryGetValue( "name", out key ) )
        context.Data.TryGetValue( key, out dataObject );
      else
        dataObject = context.DataContext;

      if ( dataObject == null )
        return null;


      string path;

      if ( arguments.TryGetValue( "path", out path ) )
        dataObject = DataBinder.Eval( dataObject, path );

      return dataObject;
    }


    /// <summary>
    /// 获取绑定值
    /// </summary>
    /// <param name="context">绑定上下文</param>
    /// <param name="arguments">绑定参数</param>
    /// <returns>绑定值</returns>
    public static string GetValue( HtmlBindingContext context, IDictionary<string, string> arguments )
    {

      var dataObject = GetDataObject( context, arguments );

      if ( dataObject == null )
        return null;

      {
        string format;
        if ( arguments.TryGetValue( "format", out format ) )
        {
          var formattable = dataObject as IFormattable;

          if ( formattable != null )
            return ((IFormattable) dataObject).ToString( format, CultureInfo.InvariantCulture );
        }
      }



      {
        string value;
        if ( arguments.TryGetValue( "value", out value ) )
        {
          if ( Convert.ToBoolean( dataObject ) )
            return value;

          else if ( arguments.TryGetValue( "alternativeValue", out value ) )
            return value;

          else
            return null;
        }
      }



      return dataObject.ToString();

    }


    object IDataContextExpressionBinder.GetDataContext( HtmlBindingContext context, IDictionary<string, string> arguments )
    {
      return GetDataObject( context, arguments );
    }

    string IExpressionBinder.GetValue( HtmlBindingContext context, IDictionary<string, string> arguments )
    {
      return GetValue( context, arguments );
    }

  }
}
