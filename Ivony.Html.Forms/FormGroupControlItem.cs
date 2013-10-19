using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  public abstract class FormGroupControlItem
  {

    protected FormGroupControlItem( FormGroupControlBase group )
    {
      GroupControl = group;
    }


    public FormGroupControlBase GroupControl
    {
      get;
      private set;
    }


    public abstract string Value
    {
      get;
    }

    public abstract bool Selected
    {
      get;
      set;
    }



  }
}
