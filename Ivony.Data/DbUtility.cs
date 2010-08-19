using System;
using System.Data;
using System.Data.Common;
using Ivony.Data.SqlDom;
using System.Collections.Generic;
using System.Configuration;
using System.Collections;



namespace Ivony.Data
{
  public abstract class DbUtility
  {


    #region 抽象方法

    /// <summary>
    /// 由派生类实现，创建IDbCommand对象
    /// </summary>
    /// <param name="newConnection">指定是否同时创建新的数据库连接</param>
    protected abstract IDbCommand CreateCommand( bool newConnection );

    /// <summary>
    /// 由派生类实现，创建IDataAdapter对象
    /// </summary>
    /// <param name="selectCommand">查询命令</param>
    protected abstract IDataAdapter CreateDataAdapter( IDbCommand selectCommand );

    /// <summary>
    /// 由派生类实现，创建IDataParameter对象
    /// </summary>
    /// <param name="name">参数名</param>
    /// <param name="value">参数值</param>
    protected abstract IDataParameter CreateParameter( string name, object value );

    #endregion


    private bool _alwaysCreateConnection = true;

    /// <summary>
    /// 指示是否总是创建新的连接
    /// </summary>
    public virtual bool AlwaysCreateConnection
    {
      get { return _alwaysCreateConnection; }
      set { _alwaysCreateConnection = value; }
    }


    private bool _hideConnection;
    /// <summary>
    /// 指示是否已经将数据库连接隐藏，即Connection等属性不可用。
    /// </summary>
    public bool HideConnection
    {
      get { return _hideConnection; }
      protected set { _hideConnection = value; }
    }

    private bool _autoConvertDBNull = true;

    /// <summary>
    /// 指示在创建Parameter的时候是否将空引用(null)自动转换为DBNull值。
    /// </summary>
    public bool AutoConvertNull
    {
      get { return _autoConvertDBNull; }
      set { _autoConvertDBNull = value; }
    }



    private CommandParserOptions _parserOptions;
    /// <summary>
    /// 设定命令解释器的参数
    /// </summary>
    public CommandParserOptions ParserOptions
    {
      get { return _parserOptions; }
      set { _parserOptions = value; }
    }


    /// <summary>
    /// 获取DbExpressionParser实例，用于分析Sql表达式
    /// </summary>
    /// <returns>DbExpressionParser实例</returns>
    protected abstract DbExpressionPaser GetExpressionParser();


    /// <summary>
    /// 填充DataSet，并将填充的最后一个表返回
    /// </summary>
    /// <param name="commandText">查询字符串模板</param>
    /// <param name="parameters">查询字符串参数</param>
    /// <returns></returns>
    public virtual DataTable Data( string commandText, params object[] parameters )
    {
      return Data( null, null, Sql.Template( commandText, parameters ) );
    }

    public virtual DataTable ExecuteData( SqlExpression expression )
    {
      return Data( null, null, expression );
    }

    /// <summary>
    /// 填充DataSet，并将填充的最后一个表返回
    /// </summary>
    /// <param name="dataSet">需要被填充的数据集</param>
    /// <param name="tableName">将最后一个表设置为什么名字</param>
    /// <param name="commandText">查询字符串模板</param>
    /// <param name="parameters">查询字符串参数</param>
    /// <returns></returns>
    public virtual DataTable Data( DataSet dataSet, string tableName, string commandText, params object[] parameters )
    {
      return Data( dataSet, tableName, Sql.Template( commandText, parameters ) );

    }

    public virtual DataTable Data( DataSet dataSet, string tableName, SqlExpression query )
    {

      IDbCommand command = null;
      try
      {
        return ExecuteData( CreateDataAdapter( command = CreateCommand( query ) ), dataSet, tableName );
      }
      catch ( DbException e )
      {
        OnError( this, e, command );
        throw;
      }
    }

    /// <summary>
    /// 执行无结果的查询
    /// </summary>
    /// <param name="commandText">查询字符串模板</param>
    /// <param name="parameters">查询字符串参数</param>
    /// <returns></returns>
    public virtual int NonQuery( string commandText, params object[] parameters )
    {
      return NonQuery( Sql.Template( commandText, parameters ) );
    }

    public virtual int NonQuery( SqlExpression query )
    {
      IDbCommand command = null;
      try
      {
        return ExecuteNonQuery( command = CreateCommand( query ) );
      }
      catch ( DbException e )
      {
        OnError( this, e, command );
        throw;
      }
    }

    /// <summary>
    /// 执行查询，并返回首行首列
    /// </summary>
    /// <param name="commandText">查询字符串模板</param>
    /// <param name="parameters">查询字符串参数</param>
    /// <returns></returns>
    public virtual object Scalar( string commandText, params object[] parameters )
    {
      return Scalar( Sql.Template( commandText, parameters ) );
    }

    public virtual object Scalar( SqlExpression query )
    {
      IDbCommand command = null;
      try
      {
        return ExecuteScalar( command = CreateCommand( query ) );
      }
      catch ( DbException e )
      {
        OnError( this, e, command );
        throw;
      }
    }



    /// <summary>
    /// 协助执行无结果的查询
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <returns>受影响的行数。</returns>
    public static int ExecuteNonQuery( IDbCommand command )
    {
      if ( command.Connection == null )
        throw new InvalidOperationException( "数据库连接尚未初始化" );

      if ( command.Connection.State == ConnectionState.Closed )
      {
        try
        {
          command.Connection.Open();
          return command.ExecuteNonQuery();
        }
        finally
        {
          command.Connection.Close();
        }
      }
      else
        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// 协助执行查询，并返回首行首列
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <returns></returns>
    public static object ExecuteScalar( IDbCommand command )
    {
      if ( command.Connection == null )
        throw new InvalidOperationException( "数据库连接尚未初始化" );

      if ( command.Connection.State == ConnectionState.Closed )
      {
        try
        {
          command.Connection.Open();
          return command.ExecuteScalar();
        }
        finally
        {
          command.Connection.Close();
        }
      }
      else
        return command.ExecuteScalar();
    }


    /// <summary>
    /// 执行查询，并返回首行
    /// </summary>
    /// <param name="commandText">查询字符串模板</param>
    /// <param name="parameters">查询字符串参数</param>
    /// <returns></returns>
    public virtual DataRow SingleRow( string commandText, params object[] parameters )
    {
      return SingleRow( Sql.Template( commandText, parameters ) );
    }

    /// <summary>
    /// 执行查询，并返回首行
    /// </summary>
    /// <param name="query">查询表达式</param>
    /// <returns></returns>
    public virtual DataRow SingleRow( SqlExpression query )
    {

      IDbCommand command = null;

      try
      {
        return ExecuteSingleRow( command = CreateCommand( query ) );
      }
      catch ( DbException e )
      {
        OnError( this, e, command );
        throw;
      }
    }


    /// <summary>
    /// 协助执行查询，并返回首行
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <returns></returns>
    public static DataRow ExecuteSingleRow( IDbCommand command )
    {
      if ( command.Connection == null )
        throw new InvalidOperationException( "数据库连接尚未初始化" );

      if ( command.Connection.State == ConnectionState.Closed )
      {
        try
        {
          command.Connection.Open();
          return ExecuteSingleRowPrivate( command );
        }
        finally
        {
          command.Connection.Close();
        }
      }
      else
      {
        return ExecuteSingleRowPrivate( command );
      }
    }

    private static DataRow ExecuteSingleRowPrivate( IDbCommand command )
    {
      using ( IDataReader reader = command.ExecuteReader( CommandBehavior.SingleRow | CommandBehavior.SingleResult ) )
      {
        DataTable table = new DataTable();

        table.Load( reader );

        if ( table.Rows.Count < 1 )
          return null;
        else
          return table.Rows[0];
      }
    }


    /// <summary>
    /// 执行命令，并返回结果集的第一列
    /// </summary>
    /// <param name="commandTemplate">命令模板</param>
    /// <param name="parameters">命令参数</param>
    /// <returns></returns>
    public object[] ExecuteSingleColumn( string commandTemplate, params object[] parameters )
    {

      return ExecuteSingleColumn<object>( Sql.Template( commandTemplate, parameters ) );

    }

    /// <summary>
    /// 执行命令，并返回结果集的第一列
    /// </summary>
    /// <typeparam name="T">第一列的数据类型</typeparam>
    /// <param name="commandTemplate">命令模板</param>
    /// <param name="parameters">命令参数</param>
    /// <returns></returns>
    public T[] ExecuteSingleColumn<T>( string commandTemplate, params object[] parameters )
    {

      return ExecuteSingleColumn<T>( Sql.Template( commandTemplate, parameters ) );

    }

    /// <summary>
    /// 执行命令，并返回结果集的第一列
    /// </summary>
    /// <typeparam name="T">第一列的数据类型</typeparam>
    /// <param name="query">查询表达式</param>
    /// <returns></returns>
    public T[] ExecuteSingleColumn<T>( SqlExpression query )
    {
      DataTable data = ExecuteData( query );
      List<T> list = new List<T>();

      foreach ( DataRow row in data.Rows )
        list.Add( (T) row[0] );

      return list.ToArray();
    }


    public T[] ExecuteEntity<T>( SqlExpression query )
    {
      throw new NotSupportedException();
    }







    /// <summary>
    /// 协助执行查询，并返回DataReader对象
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <returns></returns>
    public static IDataReader ExecuteReader( IDbCommand command )
    {
      if ( command.Connection == null )
        throw new InvalidOperationException( "数据库连接尚未初始化" );

      if ( command.Connection.State == ConnectionState.Closed )
      {
        command.Connection.Open();

        return command.ExecuteReader( CommandBehavior.CloseConnection );
      }
      else
        return command.ExecuteReader();
    }

    /// <summary>
    /// 协助填充DataSet，并将填充的最后一个表返回
    /// </summary>
    /// <param name="adapter">用来填充数据的适配器</param>
    /// <param name="dataSet">需要被填充的数据集</param>
    /// <param name="tableName">将最后一个表设置为什么名字</param>
    /// <returns></returns>
    public static DataTable ExecuteData( IDataAdapter adapter, DataSet dataSet, string tableName )
    {
      if ( dataSet == null )
        dataSet = new DataSet();

      adapter.Fill( dataSet );

      DataTable dataTable = dataSet.Tables[dataSet.Tables.Count - 1];


      if ( tableName != null )
        dataTable.TableName = tableName;

      return dataTable;
    }


    /// <summary>
    /// 解析SQL命令模版和填充参数生成Command对象
    /// </summary>
    /// <param name="commandText">SQL命令模版</param>
    /// <param name="parameters">参数</param>
    /// <returns></returns>
    protected IDbCommand CreateCommand( string commandText, params object[] parameters )
    {

      return CreateCommand( Sql.Template( commandText, parameters ) );

    }


    /// <summary>
    /// 根据SQL表达式来创建一个DbCommand对象
    /// </summary>
    /// <param name="query">SQL表达式</param>
    /// <returns>用于执行表达式的DbCommand对象</returns>
    protected virtual IDbCommand CreateCommand( SqlExpression query )
    {
      IDbCommand command = CreateCommand( _alwaysCreateConnection );

      IDictionary<string, object> parameters = new Dictionary<string, object>();

      command.CommandText = GetExpressionParser().Parse( query, out parameters );
      foreach ( KeyValuePair<string, object> p in parameters )
        command.Parameters.Add( CreateParameter( p.Key, p.Value ) );

      return command;
    }

    protected IDictionary<string, object> ParseStoredProcedureExpression( out string storedProcedureName, string template, params object[] parameters )
    {

      if ( parameters.Length > 0 )
        throw new NotImplementedException();
      else
      {
        storedProcedureName = template;
        return new Dictionary<string, object>();
      }
    }

    protected abstract StoredProcedureBase CreateStoredProcedureProtected( string name );


    /*
    public StoredProcedureBase CreateStoredProcedure( string template, params object[] parameters )
    {

      string[] parameters;
      string name = ParseStoredProcedureTemplate( template, out parameters );
      
      StoredProcedureBase storedProcedure = CreateStoredProcedureProtected( name );
    }

    private string ParseStoredProcedureTemplate( string template, out string[] parameters )
    {
      throw new NotImplementedException();
    }
    */






    /*
        public static DbUtility this[string name]
        {
          get
          {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[name];

            if ( setting == null )
              return null;

            lock ( _utilities.SyncRoot )
            {
              if ( _utilities.Contains( name ) )
                return (DbUtility) _utilities[name];



              DbUtility utility = CreateDbUtility( setting.ProviderName, setting.ConnectionString );
              _utilities[name] = utility;

              return utility;

            }
          }
        }
    */

    private static Hashtable _utilities = new Hashtable();


    private static DbUtility CreateDbUtility( string provider, string connectionString )
    {

      switch ( provider )
      {
        case "MSSQL":
          return new SqlDbUtility( connectionString );
        default:
          throw new NotSupportedException( "未知的数据库提供程序类型" );
      }

    }



    /// <summary>
    /// 当发生错误时
    /// </summary>
    public event DbErrorEventHandler Error;

    protected void OnError( object sender, DbException exception, IDbCommand command )
    {
      DbErrorEventHandler handler = Error;
      if ( handler != null )
      {
        if ( HideConnection )
          command = null;

        handler( sender, new DbErrorEventArgs( exception, command ) );


      }
    }

    public delegate void DbErrorEventHandler( object sender, DbErrorEventArgs e );

    public class DbErrorEventArgs
    {
      private DbException _exception;
      private IDbCommand _command;

      public DbErrorEventArgs( DbException exception, IDbCommand command )
      {
        _exception = exception;
        _command = command;
      }

      public Exception Exception
      {
        get { return _exception; }
      }

      public IDbCommand Command
      {
        get
        {
          return _command;
        }
      }
    }


    public virtual ITransactionUtility CreateTransaction()
    {
      throw new NotSupportedException();
    }
  }

  public class DbUtilitySettings
  {
    public bool AlwaysNewConnection { get; set; }
    public bool ConvertNullToDbNull { get; set; }
    public bool ShadowConnection { get; set; }
  }

}