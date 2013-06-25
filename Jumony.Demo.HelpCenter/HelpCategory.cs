using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Jumony.Demo.HelpCenter
{
  public class HelpCategory : HelpTopic
  {

    public HelpCategory( HelpEntry entry, string virtualPath )
      : base( entry )
    {
      Topics = FindTopics( virtualPath );
    }

    private static HelpTopic[] FindTopics( string virtualPath )
    {
      return HostingEnvironment.VirtualPathProvider.GetDirectory( virtualPath ).Children.
        Cast<VirtualFileBase>().Select( f => HelpTopic.GetTopic( f.VirtualPath ) )
        .ToArray();
    }

    public HelpTopic[] Topics { get; private set; }

  }
}
