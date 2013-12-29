using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Ivony.Html.Web.Binding
{
  public class BindingExpressionBinder : IExpressionBinder
  {
    public string ExpressionName
    {
      get { return "Binding"; }
    }

    public ExpressionType ExpressionType
    {
      get { return ExpressionType.Both; }
    }

    public string Bind( HtmlBindingContext context, IDictionary<string, string> arguments )
    {
      return GetValue( GetDataObject( context, arguments ), arguments );
    }



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
    /// 将数据对象转换为绑定值
    /// </summary>
    /// <param name="dataObject">数据对象</param>
    /// <param name="arguments">绑定参数</param>
    /// <returns>绑定值</returns>
    public static string GetValue( object dataObject, IDictionary<string, string> arguments )
    {

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


  }
}
