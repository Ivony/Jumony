using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ivony.Html.Binding
{
  public class DataContext
  {

    public DataContext()
    {
    }


    private Hashtable _dataItems = new Hashtable();


    public object GetData( IHtmlNode node )
    {

      return GetData( node, true );

    }

    protected object GetData( IHtmlNode node, bool asContainer )
    {

      if ( node == null )
        return null;


      if ( asContainer )
      {
        var _container = node as IHtmlContainer;

        if ( _container != null && _dataItems.Contains( _container ) )
          return _dataItems[_container];
      }


      var container = node.Container;

      if ( _dataItems.Contains( container ) )
        return _dataItems[container];

      else
        return GetData( container as IHtmlNode, false );
    }



  }
}
