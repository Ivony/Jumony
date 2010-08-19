
#pragma warning disable 1591
#warning 1591 disabled

using System;
using System.Collections.Generic;
using System.Text;

using System.Data;

namespace Ivony.Data
{

  public abstract class StoredProcedureBase
  {


    protected StoredProcedureBase( bool hideConnection )
    {
      _hideConnection = hideConnection;
      _autoConvertDBNull = false;
    }


    /// <summary>
    /// 由派生类实现，得到指定参数名的存储过程参数
    /// </summary>
    /// <param name="name">参数名</param>
    /// <returns></returns>
    protected abstract IDataParameter GetParameter( string name );

    /// <summary>
    /// 由派生类实现，利用指定参数名和值创建存储过程参数
    /// </summary>
    /// <param name="name">参数名</param>
    /// <param name="value">参数值</param>
    /// <returns></returns>
    protected abstract IDataParameter CreateParameter( string name, object value );

    public IDbCommand Command
    {
      get
      {
        if ( _hideConnection )
          throw new NotSupportedException();
        return InnerCommand;
      }
    }

    protected abstract IDbCommand InnerCommand
    {
      get;
    }



    private bool _hideConnection;

    /// <summary>
    /// 指示是否将数据库连接隐藏，即Command等属性不可用。
    /// </summary>
    public bool HideConnection
    {
      get { return _hideConnection; }
    }



    private bool _autoConvertDBNull;

    /// <summary>
    /// 是否将空引用自动转换为DBNull值，仅在赋值时有效。
    /// </summary>
    public bool AutoConvertNull
    {
      get { return _autoConvertDBNull; }
      set { _autoConvertDBNull = value; }
    }


    #region 参数设置

    /// <summary>
    /// 获取或设置存储过程指定参数的值
    /// </summary>
    public virtual object this[string name]
    {
      get { return GetValue( name ); }
      set { SetValue( name, value ); }
    }

    public virtual void SetValue( string name, object value )
    {
      if ( _autoConvertDBNull && value == null )
        GetParameter( name ).Value = DBNull.Value;
      else
        GetParameter( name ).Value = value;
    }

    public virtual object GetValue( string name )
    {
      return GetParameter( name ).Value;
    }

    /// <summary>
    /// 存储过程的返回值
    /// </summary>
    public abstract int ReturnValue
    {
      get;
    }

    #endregion

    #region 执行存储过程

    /// <summary>
    /// 以ExecuteNonQuery方式执行存储过程，并返回ReturnValue值
    /// </summary>
    /// <returns>ReturnValue的值</returns>
    public virtual int Execute()
    {
      DbUtility.ExecuteNonQuery( InnerCommand );

      return ReturnValue;
    }


    /// <summary>
    /// 以ExecuteNonQuery的方式执行存储过程
    /// </summary>
    /// <returns>ExecuteNonQuery的返回值</returns>
    public virtual int ExecuteNonQuery()
    {
      return DbUtility.ExecuteNonQuery( InnerCommand );
    }

    /// <summary>
    /// 以ExecuteScalar方式执行存储过程
    /// </summary>
    /// <returns>ExecuteScalar的返回值</returns>
    public virtual object ExecuteScalar()
    {
      return DbUtility.ExecuteScalar( InnerCommand );
    }

    /// <summary>
    /// 执行存储过程，并返回首行结果
    /// </summary>
    /// <returns>首行结果</returns>
    public virtual DataRow ExecuteSingleRow()
    {
      return DbUtility.ExecuteSingleRow( InnerCommand );
    }

    #endregion
  }
}
