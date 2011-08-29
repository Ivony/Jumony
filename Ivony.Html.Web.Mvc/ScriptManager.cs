using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Mvc
{
  
  /// <summary>
  /// 注册和管理脚本引用
  /// </summary>
  public class ScriptManager
  {

    public void RegisterScriptLibrary( string name )
    { 
    
    }
  }




  public abstract class ScriptLibrary
  {

    public abstract string Name
    {
      get;
    }
  
  }

  public class ScriptLibraryCollection : SynchronizedKeyedCollection<string, ScriptLibrary>
  {

    protected override string GetKeyForItem( ScriptLibrary item )
    {
      return item.Name;
    }
  }
}
