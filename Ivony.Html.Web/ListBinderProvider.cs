using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{
  public class ListBinderProvider
  {

    public static IHtmlElementBinder CreateListBinder( ListDataContext dataContext, IHtmlElement scope )
    {


      if ( GeneralListBinder.CanBind( scope ) )
        return new GeneralListBinder( scope, dataContext.ListData );


      return null;

    }

  }
}
