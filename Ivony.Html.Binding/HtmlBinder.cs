using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{
  public static class HtmlBinder
  {
    internal static void BindData( IHtmlElement element, object p )
    {
      throw new NotImplementedException();
    }
  }

  public interface IHtmlElementBinder
  {
    void BindData( IHtmlElement element, object data );
  }

  public class HtmlElementBinder : IHtmlElementBinder
  {

    public void BindData( IHtmlElement element, object data )
    {

      if ( IsComplexData( data.GetType() ) )
        BindComplexData( element, data );
      else
        BindSimpleData( element, data );


    }

    private bool IsComplexData( Type dataType )
    {
      if ( dataType.IsPrimitive )
        return false;

      if ( dataType.IsEnum )
        return false;

      if ( dataType.IsAssignableFrom( typeof( IHtmlContent ) ) )
        return false;

      if ( dataType.IsAssignableFrom( typeof( IFormattable ) ) )
        return false;

      return true;
    }

    private void BindSimpleData( IHtmlElement element, object data )
    {
      var htmlContent = data as IHtmlContent;

      if ( htmlContent != null )
        element.InnerHtml( htmlContent.ToHtmlString() );
      else
        element.InnerText( data.ToString() );
    }

    private void BindComplexData( IHtmlElement element, object data )
    {
      var descriptors = Decomposition( data );

      foreach ( var item in descriptors )
      {

        if ( IsComplexData( item.DataType ) )
        {
          var target = FindTarget( element, item.ReferenceName );

          if ( item.Binder != null )
            item.Binder.BindData( target, item.Data );
          else
            HtmlBinder.BindData( target, item.Data );
        }

      }
    }

    private IHtmlElement FindTarget( IHtmlElement element, string name )
    {
      throw new NotImplementedException();
    }


    private IEnumerable<DataDescriptor> Decomposition( object data )
    {
      throw new NotImplementedException();
    }

  }


  public class DataDescriptor
  {

    public string ReferenceName
    {
      get;
      set;
    }

    public Type DataType
    {
      get;
      set;
    }

    public object Data
    {
      get;
      set;
    }

    public IHtmlElementBinder Binder
    {
      get;
      set;
    }

  }




}
