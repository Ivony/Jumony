using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Web.Html
{
  public class HtmlBindingSheet
  {



  }

  public class HtmlBindingRule
  {
    string _selector;
    Dictionary<string, string> settings;

    private Regex cssRulesRegex = new Regex( @"(\s*(?<name>[\w-]+):(?<value>.*?);)+" );


    public HtmlBindingRule( string rule )
    {

    }
  }
}
