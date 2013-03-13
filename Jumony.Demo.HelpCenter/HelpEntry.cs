using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jumony.Demo.HelpCenter
{
  public class HelpEntry
  {

    public string Title { get; set; }

    public string VirtualPath { get; set; }

    public string Category { get; set; }

    public string Name { get; set; }

    public IDictionary<string, string> SubTitles { get; set; }

  }
}