using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Ivony.Html.Parser;
using System.Collections.ObjectModel;

namespace Ivony.Html.Web
{
  public static class RequestMappers
  {

    private static SynchronizedCollection<IRequestMapper> _mappers = new SynchronizedCollection<IRequestMapper>();


    static RequestMappers()
    {
      _mappers.Add( new RewriteToAshxProvider() );
    }


    public static MapInfo MapRequest( HttpRequest request )
    {
      foreach ( var mapper in _mappers )
      {
        var result = mapper.MapRequest( request );
        if ( result != null )
        {
          result.Mapper = mapper;
          return result;
        }
      }

      return null;
    }
  }
}
