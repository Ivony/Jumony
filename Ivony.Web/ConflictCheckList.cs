using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web
{
  /// <summary>
  /// 简单路由表冲突检查表
  /// </summary>
  public sealed class ConflictCheckList
  {

    private Dictionary<string, SimpleRouteRule> virtualPathList = new Dictionary<string, SimpleRouteRule>( StringComparer.OrdinalIgnoreCase );
    private Dictionary<string, SimpleRouteRule> routeValuesList = new Dictionary<string, SimpleRouteRule>( StringComparer.OrdinalIgnoreCase );


    private object _sync = new object();

    /// <summary>
    /// 在冲突检测表中添加一条记录，并检测与现有规则是否冲突。
    /// </summary>
    /// <param name="rule">要添加的规则</param>
    /// <param name="conflictRule">与之相冲突的规则，如果有的话</param>
    /// <returns>是否成功</returns>
    public bool AddRuleAndCheckConflict( SimpleRouteRule rule, out SimpleRouteRule conflictRule )
    {

      var virtualPath = rule.GetVirtualPathDescriptor();
      var routeValues = rule.GetRouteValuesDescriptor();

      lock ( _sync )
      {

        if ( virtualPathList.TryGetValue( virtualPath, out conflictRule ) )
          return false;

        if ( routeValuesList.TryGetValue( routeValues, out conflictRule ) )
          return false;


        conflictRule = null;
        virtualPathList.Add( virtualPath, rule );
        routeValuesList.Add( routeValues, rule );

        return true;
      }
    }
  }
}
