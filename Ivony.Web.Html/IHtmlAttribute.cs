using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html
{
  public interface IHtmlAttribute
  {

    IHtmlElement Element
    {
      get;
    }

    string Name
    {
      get;
    }

    string AttributeValue
    {
      get;
      set;
    }



    /// <summary>
    /// 移除此属性
    /// </summary>
    void Remove();

  }
}
