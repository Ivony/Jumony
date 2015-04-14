using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 协助进行数据绑定的帮助类型
  /// </summary>
  public static class DataBinder
  {


    private static Regex expressionRegex = new Regex( @"^(?<part>((^|\.)(?<name>\w+))|(\[(?<name>\w+)\])|(\((?<name>\w+)\)))+$$", RegexOptions.Compiled | RegexOptions.ExplicitCapture );

    public static object Eval( object container, string expression )
    {
      if ( string.IsNullOrWhiteSpace( expression ) )
        throw new ArgumentNullException( "expression" );

      expression = expression.Trim();

      var match = expressionRegex.Match( expression );

      if ( !match.Success )
        throw new FormatException( string.Format( "无法识别表达式 \"{0}\"", expression ) );


      var result = container;

      foreach ( var item in match.Groups["part"].Captures.Cast<Capture>() )
      {

        if ( item.Value.StartsWith( "(" ) || item.Value.StartsWith( "[" ) )
          result = GetIndexedPropertyValue( result, item.FindCaptures( match.Groups["name"] ).First().Value );

        else
          result = GetPropertyValue( result, item.FindCaptures( match.Groups["name"] ).First().Value );
      }


      return result;
    }

    private static object GetPropertyValue( object container, string name )
    {
      if ( container == null )
        return null;

      var dynamic = container as IDynamicMetaObjectProvider;
      if ( dynamic != null )
        return DynamicBinder.GetPropertyValue( container, name );



      var propertyDescriptor = TypeDescriptor.GetProperties( container ).Find( name, true );
      if ( propertyDescriptor == null )
        return null;

      return propertyDescriptor.GetValue( container );
    }

    private static object GetIndexedPropertyValue( object container, string name )
    {
      if ( container == null )
        return null;




      int index;
      if ( int.TryParse( name, out index ) )
      {
        var list = container as IList;
        if ( list != null )
          return list[index];
      }

      var property = container.GetType().GetProperty( "Item", BindingFlags.Public | BindingFlags.Instance );
      if ( property == null )
        return null;

      return property.GetValue( container, new[] { name } );
    }
  }
}
