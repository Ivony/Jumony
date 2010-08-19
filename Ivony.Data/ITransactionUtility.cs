using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  public interface ITransactionUtility : IDisposable
  {

    void Begin();

    /// <summary>
    /// 提交事务
    /// </summary>
    void Commit();

    /// <summary>
    /// 回滚事务
    /// </summary>
    void Rollback();

    /// <summary>
    /// 获取帮助执行SQL语句的DbUtility实例。
    /// </summary>
    DbUtility DbUtility
    {
      get;
    }
  }

  public interface ITransactionUtility<T> : ITransactionUtility where T : DbUtility
  {
    T DbUtility
    {
      get;
    }
  }


}
