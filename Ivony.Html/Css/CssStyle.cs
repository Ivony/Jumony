using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// CSS3 样式设置的实现
  /// </summary>
  public class CssStyle : CssStyleBase
  {


    private Dictionary<string, CssStyleProperty> _settings = new Dictionary<string, CssStyleProperty>();

    private object _sync = new object();

    public override object SyncRoot { get { return _sync; } }


    /// <summary>
    /// 设置样式设置
    /// </summary>
    /// <param name="setting">样式设置</param>
    protected override void SetStyleProperty( CssStyleProperty setting )
    {
      lock ( SyncRoot )
      {
        var name = setting.Name;
        var s = GetStyleProperty( name );
        if ( s == null || setting.Important || !s.Important )
          _settings[name] = setting;
      }
    }

    /// <summary>
    /// 获取样式设置
    /// </summary>
    /// <param name="name">样式名</param>
    /// <returns>样式设置</returns>
    protected override CssStyleProperty GetStyleProperty( string name )
    {
      lock ( SyncRoot )
      {
        CssStyleProperty setting;
        if ( _settings.TryGetValue( name, out setting ) )
          return setting;

        else
          return null;
      }
    }


    protected override CssStyleProperty[] GetAllStyleProperties()
    {
      return _settings.Values.ToArray();
    }
  }
}
