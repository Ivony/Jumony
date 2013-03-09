using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jumony.Demo.HelpCenter
{
  public class HelpTopic
  {
    public HelpTopic( HelpEntry entry )
    {

      Entry = entry;
      SubTopics = new List<HelpTopic>();

    }


    public string Name
    {
      get { return Entry.Name; }
    }

    public HelpEntry Entry
    {
      get;
      private set;
    }

    public List<HelpTopic> SubTopics
    {
      get;
      private set;
    }




  }
}