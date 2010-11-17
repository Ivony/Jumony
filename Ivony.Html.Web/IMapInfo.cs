using System;
namespace Ivony.Html.Web
{
  public interface IMapInfo
  {
    IHtmlHandler Handler { get; }
    IHtmlDocument LoadTemplate();
    IRequestMapper Mapper { get; }
    Uri OriginUrl { get; }
    string RewritePath { get; }
  }
}
