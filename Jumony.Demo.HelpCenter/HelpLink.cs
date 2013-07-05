using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumony.Demo.HelpCenter
{
  public class HelpLink
  {

    public HelpLink( string title, string path )
    {
      Title = title;
      Path = path;
    }


    public string Title { get; private set; }
    public string Path { get; private set; }

  }
}
