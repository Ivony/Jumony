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
  /// 处理{Binding xxx}表达式的绑定器
  /// </summary>
  /// <example>
  /// &lt;a href="{binding path=Link}"&gt;Link&lt;/a&gt;;
  /// </example>
  public class BindingExpressionBinder : IDataObjectExpressionBinder, IElementExpressionBinder
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



      if ( dataObject is string )
        return dataObject;

      var listData = dataObject as IEnumerable;//如果是列表，则包装成 ListDataContext 对象。
      if ( listData != null )
        return new ListDataContext( listData, ListBindingMode.Repeat );


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

    private static string ResolveFormatExpressionEscape( Match match )
    {
      if ( match.Groups["format"].Success )
        return "{0:" + match.Groups["format"].Value.Replace( "{", "{{" ).Replace( "}", "}}" ) + "}";

      else
        return "#";
    }


    object IDataObjectExpressionBinder.GetDataContext( HtmlBindingContext context, IDictionary<string, string> arguments )
    {
      return GetDataObject( context, arguments );
    }

    string IExpressionBinder.GetValue( HtmlBindingContext context, IDictionary<string, string> arguments )
    {
      return GetValue( context, arguments );
    }

  }
}
