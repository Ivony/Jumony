using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web.Mvc
{
  public class MvcMapping : RequestMapping
  {

    public MvcMapping( string virtualPath, IHtmlHandler handler )
      : base( MvcRequestMapper.Instance, handler )
    {
      TemplatePath = virtualPath;
    }

  }

  public class MvcRequestMapper : IRequestMapper
  {

    private static readonly MvcRequestMapper _instance = new MvcRequestMapper();

    public static MvcRequestMapper Instance
    {
      get { return _instance; }
    }

    RequestMapping IRequestMapper.MapRequest( HttpRequestBase request )
    {
      throw new NotSupportedException();
    }
  }

}
