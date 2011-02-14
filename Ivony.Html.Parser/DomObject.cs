using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public abstract class DomObject : IHtmlDomObject
  {
    public object RawObject
    {
      get { return this; }
    }

    public abstract IHtmlDocument Document
    {
      get;
    }


    private readonly object _sync = new object();

    public object SyncRoot
    {
      get { return _sync; }
    }


    public string RawHtml
    {
      get { return null; }
    }
  }
}
