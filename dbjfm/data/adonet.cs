#region Copyright © 2003-2005 DBJ*Solutions Ltd. All Rights Reserved
// This file and its contents are protected by United States and 
// International copyright laws. Unauthorized reproduction and/or 
// distribution of all or any portion of the code contained herein 
// is strictly prohibited and will result in severe civil and criminal 
// penalties. Any violations of this copyright will be prosecuted 
// to the fullest extent possible under law. 
// 
// UNDER NO CIRCUMSTANCES MAY ANY PORTION OF THE SOURCE
// CODE BE DISTRIBUTED, DISCLOSED OR OTHERWISE MADE AVAILABLE TO ANY 
// THIRD PARTY WITHOUT THE EXPRESS WRITTEN CONSENT.
// 
// THE END USER LICENSE AGREEMENT (EULA) ACCOMPANYING THE PRODUCT 
// PERMITS THE REGISTERED DEVELOPER TO REDISTRIBUTE THE PRODUCT IN 
// EXECUTABLE FORM ONLY IN SUPPORT OF APPLICATIONS WRITTEN USING 
// THE PRODUCT. IT DOES NOT PROVIDE ANY RIGHTS REGARDING THE 
// SOURCE CODE CONTAINED HEREIN. 
// 
// THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE. 
#endregion

//--------------------------------------------------------------------
//
//  The whole data part of this library was designed and developed 2002-2003
//  by :
//  
//  Mr. Dusan B. Jovanovic ( dbj@dbj.org )
//
//  Author donates this code to Clear-Technology Inc. (Denver,USA) 
//  Author reserves intelectual copyrights on this code.
//
//--------------------------------------------------------------------

namespace dbj.fm.data
{
	using System;
	using System.Text ;
	using System.Data ;
	using System.Data.OleDb ;
	// using System.Runtime.InteropServices ;
	/// <summary>
	/// Summary of AdoNet class. Defines method and properties which helps in making various
	/// calls like select, insert, update etc to the database using OleDbConnection.
	/// </summary>
	public sealed class AdoNet : dbjsql
	{
		OleDbConnection  connection_ ;
		OleDbDataAdapter adapter_  ;
		OleDbTransaction  transaction_ ;
		OleDbCommand command_ ;


		//----------------------------------------------------------------------	
		// this constructor requires a connection string
		/// <summary>
		/// Constructor of AdoNet class.
		/// </summary>
		/// <param name="conn_string">connection string</param>
		public AdoNet(string conn_string) : base(conn_string)
		{
			this.construct() ;
		}

		
		/// <summary>
		/// makes the string containing the full exception report from the given OleDbException.
		/// </summary>
		/// <param name="e">OleDbException</param>
		/// <returns>string representing exception report</returns>
		public static string exception_report(OleDbException e) 
		{
			StringBuilder result = new StringBuilder("SQLSRV2000 ERROR: ") ;
			try 
			{
				OleDbErrorCollection myErrors = e.Errors ;
				result.AppendFormat("\nHRESULT: {1}", e.ErrorCode );
				result.AppendFormat("\nError #{1}: {2} ", e.ErrorCode , e.Message );
				result.AppendFormat("\nError reported by {1}\nFrom method: {2}", e.Source , e.TargetSite  );
				result.Append("\nNOTE: In case of updates neither record was written to database.");
				result.Append("\nErrors collection contains:");

				foreach (OleDbError err in e.Errors ) 
				{
					result.AppendFormat("\nNative Error: {1}", err.NativeError  );
					result.AppendFormat("\nError {0}\nFrom provider {1}\nANSI SQL State{2}", err.Message  , err.Source, err.SQLState   ) ;
				}
			}
			catch ( Exception x )
			{
				result.AppendFormat("\nerror in exception_report(): {0}", x.Message ) ;
			}
			return result.ToString() ;			
		}
		
		/// <summary>
		/// Returns names of tables or views.
		/// </summary>		
		/// <param name="ttype">must be 'BASE TABLE' or 'VIEW'</param> 
		/// <returns>array of names</returns>
		public override object[] get_table_names(Idata.TableType ttype )
		{
#if DEBUG
			if ( ( ttype != TableType.TABLE  )  && ( ttype != TableType.VIEW  )
				)
				throw new Error( "table_type is wrong argument, must be 'BASE TABLE' or 'VIEW' " ) ;
#endif
			System.Collections.ArrayList retval = null ;
			IDataReader reader = null ;
			string table_type ;
			try 
			{
				table_type =   ttype == Idata.TableType.TABLE ? "BASE TABLE" : "VIEW" ;
				reader =  this.getReaderFor(
					"SELECT DISTINCT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES " +
					"WHERE TABLE_TYPE = '"+ 
					table_type +"'") ;
				retval = new System.Collections.ArrayList() ;
				while( reader.Read() )
					retval.Add(reader.GetValue(0).ToString()) ;
				return retval.ToArray() ;
			} 
			catch ( Exception x) 
			{ 
				throw new Error("AdoNet.get_table_names(" + ttype +")",x); 
			}
			finally 
			{
				if ( reader != null ) 
					if ( ! reader.IsClosed ) reader.Close() ;
			}
		}

		//--------------------------------------------------------------
		// Execute any non-query through here...
		/// <summary>
		/// Executes a SQL statement against the Connection and returns the number of rows affected.
		/// </summary>
		/// <param name="statement">sql statement</param>
		/// <returns>Number of rows affected</returns>
		/// <exception cref=" System.Exception">on error throws System.Exception</exception>
		int ExecuteNonQuery( string statement ) 
		{
			TRANSACTION_STATE tx_state = TRANSACTION_STATE.COMMIT ;
			System.Exception  ex = null ;
			int result = 0;

			try
			{
				if(this.connection_.State == ConnectionState.Open)
					this.close_connection(TRANSACTION_STATE.COMMIT) ;
				
				this.open_connection(true, OPERATION_KIND.CHANGING_DATA );
				OleDbCommand sqlcmd = new OleDbCommand(statement,this.connection_) ;
				
				sqlcmd.Transaction =  this.transaction_  ;
				result = sqlcmd.ExecuteNonQuery() ;
			}
				/*
				catch ( OleDbException sex )
				{
					return sex.Number ;
				}
				*/
			catch ( System.Exception se ) 
			{
				tx_state = TRANSACTION_STATE.ROLLBACK ;
				if ( se != null ) ex = se ;
			}
			finally	
			{
				this.close_connection(tx_state); 
				if ( ex != null ) 
				{
					ex.Source = "AdoNet.executeNonQuery()" ;
					throw ex ;
				}
			}
			return result ;
		}

		/// <summary>
		/// this is used for signaling what is the last transaction outcome
		/// </summary>
		internal enum TRANSACTION_STATE : int { COMMIT, ROLLBACK } ;

		/// <summary>
		/// signal what kind of operation will be performed on the data
		/// </summary>
		internal enum OPERATION_KIND : int { USING_DATA , CHANGING_DATA } ;

		/// <summary>
		/// open the connection taking care of the transactional mode
		/// it transaction is required
		/// </summary>
		/// <param name="WITH_TRANSACTION">true if transaction is required</param>
		/// <param name="op_kind">kind of operation</param> 
		private void open_connection(bool WITH_TRANSACTION, OPERATION_KIND op_kind )
		{
			/*
				.NET isolation levels and their meanning
				----------------------------------------
				Chaos			The pending changes from more highly isolated transactions cannot be overwritten. 
				ReadCommitted	Shared locks are held while the data is being read to avoid dirty reads, but the data can be changed before the end of the transaction, resulting in non-repeatable reads or phantom data. 
				ReadUncommitted A dirty read is possible, meaning that no shared locks are issued and no exclusive locks are honored. 
				RepeatableRead	Locks are placed on all data that is used in a query, preventing other users from updating the data. Prevents non-repeatable reads but phantom rows are still possible. 
				Serializable	A range lock is palced on the DataSet, preventing other users from updating or inserting rows into the dataset until the transaction is complete. 
				Unspecified		A different isolation level than the one specified is being used, but the level cannot be determined. 
			 */
			IsolationLevel iso_level = IsolationLevel.Unspecified ;
			// here we encapsulate the level of locking/isolation we use
			// for each operation kind
			switch ( op_kind )
			{
				case OPERATION_KIND.CHANGING_DATA : iso_level = IsolationLevel.Serializable ;
					break ;
				case OPERATION_KIND.USING_DATA : iso_level = IsolationLevel.ReadCommitted ;
					break ;
				default :
					throw new Error("Unknown OPERATION_KIND ?") ;
			} 

			if(this.connection_.State != ConnectionState.Open )
			{
				this.connection_.Open() ;
				if ( WITH_TRANSACTION )
					this.transaction_ = this.connection_.BeginTransaction(iso_level) ;
			}
		}

		/// <summary>
		/// close the current connection, taking care of the possible transaction
		/// and its outcome
		/// </summary>
		/// <param name="TxSTATE">commit or rollback, enum value</param>
		private void close_connection(TRANSACTION_STATE TxSTATE)
		{
			if(this.connection_.State != ConnectionState.Closed)
			{
				if ( this.transaction_ != null ) 
					if ( this.transaction_.Connection != null ) 
					{
						if ( TxSTATE == AdoNet.TRANSACTION_STATE.COMMIT )	
							this.transaction_.Commit() ;
						else
							this.transaction_.Rollback() ;

						this.transaction_ = null ;
					}
				this.connection_.Close() ;
			}
		}

		//--------------------------------------------------------------
		// helper
		/// <summary>
		/// Makes connection string
		/// </summary>
		/// <param name="data_source_">Source of data</param>
		/// <param name="user_id_">User Id</param>
		/// <param name="user_password_">User Password</param>
		/// <param name="initial_catalog_">database name</param>
		/// <returns>string representing connection string</returns>
		static string make_conn_str (string data_source_ , string user_id_ ,  string user_password_ , string initial_catalog_ )
		{
			System.Object[] args = {data_source_, initial_catalog_, user_id_, user_password_} ;
			return String.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};", args ) ;
		}
		
		//new ctor
		/// <summary>
		/// Constructor of AdoNet Class
		/// </summary>
		/// <param name="data_source_">Source of data</param>
		/// <param name="user_id_">User Id</param>
		/// <param name="user_password">User Password</param>
		/// <param name="initial_catalog_">database name</param>
		public AdoNet(string data_source_, string user_id_, string user_password, string initial_catalog_ ):
			base(make_conn_str(data_source_,user_id_,user_password, initial_catalog_))
		{
			this.construct() ;
		}
		
		/// <summary>
		/// init function for COM clients
		/// </summary>
		/// <param name="new_cs">connection string</param>
		public override void init(string new_cs)
		{
			this.connection_string = new_cs ;
			this.construct() ;
		}
		
		
		/// <summary>
		/// private called from constructors
		/// </summary>
		/// <exception cref=" OleDbException">While making connection to the database if oledb 
		/// error occurs then OleDbException is thrown</exception>
		/// <exception cref=" Exception ">If other than OleDb error occurs Exception is thrown</exception>
		private void construct()
		{
			this.check() ;
			
			try 
			{
				this.connection_ = new OleDbConnection( connection_string ) ;
				this.adapter_ = new OleDbDataAdapter() ;
				this.command_ = new OleDbCommand() ;
			}
			catch ( OleDbException sex )
			{
				throw new Error( AdoNet.exception_report(sex) ) ;
			}
			catch ( Exception se ) // anything else
			{
				throw new Error( "AdoNet.construct()", se ) ;
			}	
		}


		 
		/// <summary>
		/// execute the SQL SELECT statement and return the data set.
		/// Fills the resulting data set and creates a DataTable named "Table".
		/// </summary>
		/// <param name="statement">Sql statement</param>
		/// <returns>Dataset containing the result of sql statement</returns>
		/// <exception cref="System.Exception">On error system.Exception is thrown</exception>
		public DataSet select(string statement)
		{
			statement = statement.ToLower() ;
			if((statement.IndexOf("select") == -1)&& (statement.IndexOf("exec") == -1))
				throw new Error("Wrong SQL query") ;

			DataSet result = new DataSet() ;
			TRANSACTION_STATE tx_state = TRANSACTION_STATE.COMMIT ;
			
			try
			{
				this.open_connection(true, OPERATION_KIND.USING_DATA ) ; // in transactional mode
				OleDbCommand sqlcmd = this.select_command( statement ) ;

				this.adapter_.SelectCommand = sqlcmd  ;
				// fills the data set and creates a DataTable named "Table".
				this.adapter_.Fill( result ) ;
			}
			catch ( System.Exception se ) // anything else
			{
				se.Source = "AdoNet.select()" ;
				tx_state = TRANSACTION_STATE.ROLLBACK  ;
				throw new Error( "select()", se ) ;
			}
			finally 
			{
				this.close_connection( tx_state ) ;
			}
			
			return result ;
		}
		
		
		/// <summary>
		/// execute the SQL INSERT statement
		/// </summary>
		/// <param name="statement">Sql Statement</param>
		public override void insert(string statement)
		{
			string lowText = statement.ToLower() ;
			if(lowText.IndexOf("insert") == -1) 
				throw new Error("Wrong SQL query") ;
			this.ExecuteNonQuery( statement ) ;
		}
		
		
		/// <summary>
		/// execute the SQL UPDATE statement
		/// </summary>
		/// <param name="statement">Sql Statement</param>
		public override void update(string statement)
		{
			string lowText = statement.ToLower() ;
			if(lowText.IndexOf("update") == -1) 
				throw new Error("Wrong SQL query") ;
			this.ExecuteNonQuery( statement ) ;
		}
		
		
		/// <summary>
		/// execute the SQL DELETE statement
		/// </summary>
		/// <param name="statement">sql statement</param>
		public override void erase(string statement)
		{
			string lowText = statement.ToLower() ;
			if(lowText.IndexOf("delete") == -1) 
				throw new Error("Wrong SQL query") ;
			this.ExecuteNonQuery( statement ) ;
		}
		
		
		/// <summary>
		/// execute the SQL EXEC statement
		/// </summary>
		/// <param name="statement">sql statement </param>
		/// <returns>number of row affetected</returns>
		public override int exec(string statement)
		{
			string lowText = statement.ToLower() ;
			if(lowText.IndexOf("exec") == -1)
				throw new Error("Wrong stored procedure call") ;
			return this.ExecuteNonQuery( statement ) ;
		}
		/// <summary>
		/// Executes the query, and returns the first column of the first row 
		/// in the result set returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="scalar_query">scalar query</param>
		/// <returns>Object</returns>
		public object scalar(string scalar_query ) 
		{
			object retval = null ;
			OleDbCommand command = new OleDbCommand(scalar_query);
			command.Connection = this.connection_ ;
			command.Connection.Open();
			retval = command.ExecuteScalar();
			command.Connection.Close() ;
			return retval ;
		}


		
		/// <summary>
		/// Make OleDbCommand to be assigned to OleDbDataAdapter.SelectCommand
		/// </summary>
		/// <param name="statement">sql statement</param>
		/// <returns>OleDbCommand</returns>
		public OleDbCommand select_command(string statement)
		{
			OleDbCommand command = new OleDbCommand( statement ) ;
			command.Connection = this.connection_  ;
			command.CommandType = CommandType.Text ;
			if ( this.transaction_ != null ) command.Transaction = this.transaction_  ;
			return command ;
		}
		
		/// <summary>
		/// Make OleDbCommand to be assigned to OleDbDataAdapter.InsertCommand
		/// </summary>
		/// <param name="statement">Sql statement</param>
		/// <returns>OleDbCommand</returns>
		public OleDbCommand insert_command(string statement)
		{
			OleDbCommand command = new OleDbCommand( statement ) ;
			command.Connection = this.connection_  ;
			command.CommandType = CommandType.Text ;
			if ( this.transaction_ != null ) command.Transaction = this.transaction_  ;
			return command ;
		}
		
		/// <summary>
		/// Make OleDbCommand to be assigned to OleDbDataAdapter.UpdateCommand
		/// </summary>
		/// <param name="statement">Sql Statement</param>
		/// <returns>OleDbCommand</returns>
		public OleDbCommand update_command(string statement)
		{
			OleDbCommand command = new OleDbCommand( statement ) ;
			command.Connection = this.connection_  ;
			command.CommandType = CommandType.Text ;
			if ( this.transaction_ != null ) command.Transaction = this.transaction_  ;
			return command ;
		}
		
		/// <summary>
		/// Make OleDbCommand to be assigned to OleDbDataAdapter.DeleteCommand
		/// </summary>
		/// <param name="statement">sql statement</param>
		/// <returns>OleDbCommand</returns>
		public OleDbCommand delete_command(string statement)
		{
			OleDbCommand command = new OleDbCommand( statement ) ;
			command.Connection = this.connection_   ;
			command.CommandType = CommandType.Text  ;
			if ( this.transaction_ != null ) command.Transaction = this.transaction_  ;
			return command ;	
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="theVisitor">fm.data.visitor delegate</param>
		/// <param name="select_statement">sql statement</param>
		public override void for_each_row(visitor theVisitor, string select_statement)
		{
			DataSet newset = this.select( select_statement ) ;
			DataTable result = newset.Tables["Table"] ;
			DataRowCollection rows_ = result.Rows ;

			for ( int j = 0 ; j < rows_.Count ; j++ )
			{
				theVisitor( rows_[j] ) ;
			}
		}

		/// <summary>
		/// returns DataReader
		/// </summary>
		/// <param name="statement" >sql statement</param>
		/// <exception cref="System.Exception">on error throws System.Exception</exception>
		private IDataReader getReaderFor(string statement)
		{
			IDataReader result = null ;
			try
			{
#if DEBUG
				string check_query = statement.ToLower() ;
				if((check_query.IndexOf("select") == -1) && (check_query.IndexOf("exec") == -1)) 
					throw new Error("Wrong SQL query") ;
#endif
				this.open_connection(false, OPERATION_KIND.USING_DATA ) ; // NOT in transactional mode
				
				if(String.Compare(this.check_statement(statement).ToLower(),("select")) == 0)
				{
					OleDbCommand sqlcmd = this.select_command( statement ) ;
					result = sqlcmd.ExecuteReader(CommandBehavior.CloseConnection) ;
				}
				if(String.Compare(this.check_statement(statement).ToLower(),("exec")) == 0)
				{
					OleDbCommand sqlcmd = new OleDbCommand(statement,this.connection_) ;
					result = sqlcmd.ExecuteReader(CommandBehavior.CloseConnection) ;
				}
			}
			catch ( System.Exception se ) // anything else
			{
				throw new Error( "getReaderFor() failed" , se ) ;
			}
			finally 
			{
			}
			return result ;
		}
		
		/// <summary>
		/// returns Parameter object 
		/// </summary>
		/// <param name="param_direction" >value indicating whether the parameter is input-only, output-only, bidirectional, 
		/// or a stored procedure return value parameter.</param>
		/// <param name="param_name" >name of the parameter</param>
		/// <param name="param_size" >the maximum size, in bytes, of the data within the column</param>
		/// <param name="param_type" >the DbType of the parameter.</param>
		/// <param name="param_value" >value of the parameter</param>
		/// <exception cref="Exception">on error exception is thrown</exception>
		private void add_parameter(string param_name, OleDbType param_type, ParameterDirection param_direction, object param_value, int param_size)
		{
			try
			{
				OleDbParameter param = this.command_.CreateParameter() ;
				param.ParameterName = param_name ;
				param.OleDbType = param_type ;
				param.Direction = param_direction ;
				
				if ( param_size != 0 ) 
					param.Size = param_size ;		
						
				if ( param_direction == ParameterDirection.InputOutput || param_direction == ParameterDirection.Input) // if it is an input parameter...
					param.Value = param_value ;	
				
				this.command_.Parameters.Add(param) ;
			}
			catch(Exception )
			{
				throw new Error("Cannot add parameter to command object") ;
			}
		}
		
		/// <summary>
		/// Adds parameter of varchar type to command object
		/// </summary>
		/// <param name="param_name">must be the same as the name in stored procedure</param>
		/// <param name="param_dir" >value indicating whether the parameter is input-only, output-only, bidirectional, 
		/// or a stored procedure return value parameter.</param>
		/// <param name="param_size" >the maximum size, in bytes, of the data within the column</param>
		/// <param name="param_value" >value of the parameter</param>		
		public void add_param_VARCHAR(string param_name, object param_value,int param_size,ParameterDirection param_dir)
		{
			OleDbType param_type = OleDbType.VarChar ;
			if( param_value == null) param_value = Convert.DBNull ;
			this.add_parameter( param_name, param_type, param_dir, param_value, param_size );		
		}

		/// <summary>
		/// Adds parameter of integer type to command object
		/// </summary>
		/// <param name="param_name">must be the same as the name in stored procedure</param>
		/// <param name="param_dir" >Parameter Direction</param>
		/// <param name="param_value" >value of the parameter</param>
		public void add_param_INT(string param_name, object param_value,ParameterDirection param_dir)
		{
			OleDbType param_type = OleDbType.Integer  ;
			if( param_value == null) param_value = Convert.DBNull ;
			this.add_parameter( param_name, param_type, param_dir, param_value, 0 );
		}
		
		/// <summary>
		/// Adds parameter of datetime type to command object
		/// </summary>
		/// <param name="param_name">must be the same as the name in stored procedure</param> 
		/// <param name="param_dir" >Parameter Direction</param>
		/// <param name="param_value" >value of the parameter</param>
		public void add_param_DATETIME(string param_name, object param_value,ParameterDirection param_dir)
		{
			OleDbType param_type = OleDbType.Date  ;
			if( param_value == null) param_value = Convert.DBNull ;
			this.add_parameter( param_name, param_type, param_dir, param_value, 0 );
		}
		
		/// <summary>
		/// Adds parameter of float type to command object
		/// </summary>
		/// <param name="param_name">must be the same as the name in stored procedure</param>
		/// <param name="param_dir" >Parameter Direction</param>
		/// <param name="param_value" >value of the parameter</param>
		public void add_param_FLOAT(string param_name, object param_value,ParameterDirection param_dir)
		{
			OleDbType param_type = OleDbType.Decimal  ;
			if( param_value == null) param_value = Convert.DBNull ;
			this.add_parameter( param_name, param_type, param_dir, param_value, 0 );
		}
		
		/// <summary>
		/// Adds parameter of bool type to command object
		/// </summary>
		/// <param name="param_name">must be the same as the name in stored procedure</param>
		/// <param name="param_dir" >Parameter Direction</param>
		/// <param name="param_value" >value of the parameter</param>
		public void add_param_BOOL(string param_name, object param_value,ParameterDirection param_dir)
		{
			OleDbType param_type = OleDbType.Boolean  ;
			if( param_value == null) param_value = Convert.DBNull ;
			this.add_parameter( param_name, param_type, param_dir, param_value, 0 );
		}

		/// <summary>
		/// deletes Parameters collection from command object
		/// </summary>
		public void delete_parameters()
		{
			this.command_.Parameters.Clear() ;
		}

		/// <summary>
		/// Executes command object and returns single int value from stored procedure
		/// example : 
		/// exec_sp_for_single_retval('sp_x')
		/// </summary>
		/// <param name="sp_name">the name of stored procedure</param>
		/// <returns>
		/// return singleint value, obtained through first parameter of the stored procedure
		/// </returns>
		public int exec_sp_for_single_retval(string sp_name)
		{		
			int the_number_of_rows_affected = 0 ;
			// we ignore this value in this method!
			try
			{
				this.command_.CommandText = sp_name ;
				this.command_.CommandType = CommandType.StoredProcedure ;
				this.command_.Connection = this.connection_ ;
				this.open_connection(true, OPERATION_KIND.CHANGING_DATA ) ; //with transaction
				this.command_.Transaction = this.transaction_ ;
				the_number_of_rows_affected = this.command_.ExecuteNonQuery() ;						
				this.close_connection(TRANSACTION_STATE.COMMIT) ;
			}
				/* catch( OleDbException sex )
				{
					throw sex ;
				} */
			catch(Exception err)
			{
				this.close_connection(TRANSACTION_STATE.ROLLBACK) ;
				err.Source = "AdoNet.exec_sp_for_single_retval()" ;
				throw err ;
			}
			return (int)this.command_.Parameters[0].Value ;
		}

		/// <summary>
		/// execute SP and its parameters stored in a command
		/// </summary>
		/// <param name="command_" >OleDbCommand to be used</param>
		public int exec_sp_command(OleDbCommand command_ )
		{		
			int the_number_of_rows_affected = 0 ;
			// we ignore this value in this method!
			try	
			{
				command_.CommandType = CommandType.StoredProcedure ;
				command_.Connection = this.connection_ ;
				this.open_connection(true, OPERATION_KIND.CHANGING_DATA ) ; //with transaction
				command_.Transaction = this.transaction_ ;
				the_number_of_rows_affected = command_.ExecuteNonQuery() ;						
				this.close_connection(TRANSACTION_STATE.COMMIT) ;
			}
				/* catch( OleDbException sex )
				{
					this.close_connection(TRANSACTION_STATE.ROLLBACK) ;
					throw sex ;
				} */
			catch(Exception err)
			{
				this.close_connection(TRANSACTION_STATE.ROLLBACK) ;
				err.Source = "AdoNet.exec_sp_command()" ;
				throw err ;
			}
			return (int)command_.Parameters[0].Value ;
		}

		
		/// <summary>
		/// fill_dataset
		/// </summary>
		/// <param name="ds">dataset</param>
		/// <param name="ad">OleDbAdapter</param>
		/// <param name="statement">sql statement</param>
		/// <param name="tbl_name">table name</param>
		/// <exception cref="System.Exception">On error throws System.Exception</exception>
		public void fill_dataset(DataSet ds, OleDbDataAdapter ad,string statement,string tbl_name)
		{
			try
			{
				if(this.connection_.State == ConnectionState.Closed)
					this.open_connection(true, OPERATION_KIND.USING_DATA ) ;

				ad.SelectCommand = this.select_command(statement) ;
				ds.Tables.Add(tbl_name) ;
				ad.Fill(ds,tbl_name) ;
			}
			catch ( System.Exception se ) // anything else
			{
				this.close_connection(TRANSACTION_STATE.ROLLBACK) ;
				if ( se != null ) throw new Error( se.Message ) ;
			}
		} 
		/// <summary>
		/// trims sql statement
		/// </summary>
		/// <param name="statement">sql statement</param>
		/// <returns></returns>
		string check_statement(string statement)
		{
			string [] retval = statement.Split(' ') ;
			return retval[0].Trim() ;
		}
		
		//---------------------------------------------------------
		/// <summary>
		/// Makes SPimplementation with the list of parameters supplied
		/// </summary>
		/// <param name="sp_name">name of the sp</param>
		/// <param name="list">optional list of parameters</param>
		/// <returns>SP</returns>
		public SP new_sp_call ( string sp_name, params PARAM [] list )
		{
			SP sp = new SPimplementation(this,sp_name ) ;

			foreach( PARAM param in list )
			{
				sp.add_p( param ) ;
			}

			return sp ;
		}
		//---------------------------------------------------------

		/// <summary>Abstraction of one parameter for the stored procedure.</summary>
		public abstract class PARAM 
		{
			/// <summary>
			/// ParameterDirection
			/// </summary>
			protected ParameterDirection	direction_ ;
			/// <summary>
			/// OleDbType
			/// </summary>
			protected OleDbType				type_ ;
			/// <summary>
			/// name of the parameter
			/// </summary>
			protected string				name_ ;
			/// <summary>
			/// value of the parameter
			/// </summary>
			protected object				value_ ;
			/// <summary>
			/// size of the parameter
			/// </summary>
			protected int					size_ ;
			/// <summary>
			/// Parameter direction enumerator
			/// </summary>
			public enum DIRECTION { 
				/// <summary>
				/// In parameter 
				/// </summary>
				IN, 
				/// <summary>
				/// Out parameter
				/// </summary>
				OUT, 
				/// <summary>
				/// Inout parameter
				/// </summary>
				INOUT, 
				/// <summary>
				/// Value Return parameter
				/// </summary>
				RETVAL } ;
			/// <summary>
			/// Returns appropriate parameter dir based on the given direction
			/// </summary>
			/// <param name="dir">direction of parameter any of the value in 
			/// Direction Enumeration defined in this class</param>
			/// <returns>ParameterDirection</returns>
			static public ParameterDirection the_direction ( PARAM.DIRECTION dir )
			{
				switch(dir)
				{
					case PARAM.DIRECTION.IN:		return ParameterDirection.Input ;
					case PARAM.DIRECTION.OUT:		return ParameterDirection.Output  ;
					case PARAM.DIRECTION.INOUT:		return ParameterDirection.InputOutput  ;
					case PARAM.DIRECTION.RETVAL:	return ParameterDirection.ReturnValue  ;
					default: throw new Error("Unknown parameter direction") ;
				}
			}
			/// <summary>
			/// Constructor of PARAM class
			/// </summary>
			/// <param name="dir">direction of paramater</param>
			/// <param name="typ">OleDbType</param>
			/// <param name="name">name of the paramter</param>
			/// <param name="val">value of the parameter</param>
			/// <param name="siz">size of parameter</param>
			public PARAM ( ParameterDirection dir, OleDbType typ, string name, object val, int siz)
			{
				direction_ = dir ;
				type_ = typ ;
				name_ = name ;
				value_ = (val == null ?  Convert.DBNull : val) ;
				size_ = siz ; 
			}
			/// <summary>
			/// make Parameter and add it to the command object given
			/// </summary>
			/// <param name="command_" >OleDbCommand</param>
			/// <exception cref="Exception">On error throws an exception</exception>
			public void to_command( ref OleDbCommand command_ )
			{
				try
				{
					OleDbParameter param = command_.CreateParameter() ;
					param.ParameterName = this.name_ ;
					param.OleDbType = this.type_ ;
					param.Direction = this.direction_ ;
				
					if ( this.size_ != 0 ) param.Size = this.size_  ;		
						
					if ( this.direction_  == ParameterDirection.InputOutput || this.direction_ == ParameterDirection.Input) // if it is an input parameter...
						param.Value = this.value_ ;	
				
					command_.Parameters.Add(param) ;
				}
				catch(Exception )
				{
					throw new Error("Cannot add parameter to command object") ;
				}
			}

		}
		/// <summary>
		/// Makes a parameter of BOOL Type
		/// </summary>
		public sealed class PARAM_BOOL : PARAM	
		{
			/// <summary>
			/// constructor of PARAM_BOOL class
			/// </summary>
			/// <param name="direnum">Direction of the parameter</param>
			/// <param name="nam">name of the parameter</param>
			/// <param name="val">value of the parameter</param>
			public PARAM_BOOL ( PARAM.DIRECTION direnum, string nam, object val )
				: base(	PARAM.the_direction( direnum ) , OleDbType.Boolean , nam, val, 0 )
			{ }
		}
		/// <summary>
		/// Makes a parameter of FLOAT Type
		/// </summary>
		public sealed class PARAM_FLOAT : PARAM	
		{
			/// <summary>
			/// constructor of PARAM_FLOAT class
			/// </summary>
			/// <param name="direnum">Direction of the parameter</param>
			/// <param name="nam">name of the parameter</param>
			/// <param name="val">value of the parameter</param>
			public PARAM_FLOAT ( PARAM.DIRECTION direnum, string nam, object val )
				: base(	PARAM.the_direction( direnum ) , OleDbType.Decimal  , nam, val, 0 )
			{ }
		}
		/// <summary>
		/// Makes a parameter of DATETIME Type
		/// </summary>
		public sealed class PARAM_DATETIME : PARAM	
		{
			/// <summary>
			/// constructor of PARAM_DATETIME class
			/// </summary>
			/// <param name="direnum">Direction of the parameter</param>
			/// <param name="nam">name of the parameter</param>
			/// <param name="val">value of the parameter</param>
			public PARAM_DATETIME ( PARAM.DIRECTION direnum, string nam, object val )
				: base(	PARAM.the_direction( direnum ) , OleDbType.Date , nam, val, 0 )
			{ }
		}
		/// <summary>
		/// Makes a parameter of INT Type
		/// </summary>
		public sealed class PARAM_INT : PARAM	
		{
			/// <summary>
			/// constructor of PARAM_INT class
			/// </summary>
			/// <param name="direnum">Direction of the parameter</param>
			/// <param name="nam">name of the parameter</param>
			/// <param name="val">value of the parameter</param>
			public PARAM_INT ( PARAM.DIRECTION direnum, string nam, object val )
				: base(	PARAM.the_direction( direnum ) , OleDbType.Integer , nam, val, 0 )
			{ }
		}
		/// <summary>
		/// Makes a parameter of VARCHAR Type
		/// </summary>
		public sealed class PARAM_VARCHAR : PARAM	
		{
			/// <summary>
			/// constructor of PARAM_VARCHAR class
			/// </summary>
			/// <param name="direnum">Direction of the parameter</param>
			/// <param name="nam">name of the parameter</param>
			/// <param name="val">value of the parameter</param>
			/// <param name="siz" >size of the parameter</param>
			public PARAM_VARCHAR ( PARAM.DIRECTION direnum, string nam, object val, int siz )
				: base(	PARAM.the_direction( direnum ) , OleDbType.VarChar , nam, val, siz )
			{ }
		}


		/// <summary>
		/// Encapsulates an parameterized call to the stored procedure
		/// </summary>
		public interface SP 
		{
			/// <summary>
			/// add the parameter for SP call
			/// </summary>
			/// <param name="the_parameter">instance of the PARAM</param>
			void add_p ( PARAM the_parameter ) ;

			/// <summary>execute the stored proc. and return the single return value.
			/// NOTE: does not return 'rows affected'</summary>
			/// <returns>return value obtained through Param[0].Value</returns>
			int exec_for_retval() ;

			/// <summary>
			/// get or set the value of the named SP property
			/// </summary>			
			object this[string index] 
			{
				set ; get ;
			}
		} // eof interface SP

		/// <summary>
		/// SP implementation
		/// </summary>
		internal sealed class SPimplementation : SP 
		{
			string		sp_name_ ;
			AdoNet	host_ ;
			OleDbCommand	command_ ;
			/// <summary>
			/// Constructor of SPimplementation class
			/// </summary>
			/// <param name="host">instance of AdoNet Class</param>
			/// <param name="name">name of the sp</param>
			public SPimplementation ( AdoNet host, string name ) 
			{ 
				sp_name_ = name ; 
				host_ = host ;
				this.command_ = new OleDbCommand() ;
			}
			/// <summary>
			/// adds the parameter to the command object
			/// </summary>
			/// <param name="the_parameter"></param>
			public void add_p ( PARAM the_parameter ) 
			{
				the_parameter.to_command( ref this.command_ ) ;
			}
			/// <summary>
			/// Executes the sp
			/// </summary>
			/// <returns>the number of rows affected</returns>
			public int exec_for_retval() 
			{
				this.command_.CommandText = this.sp_name_ ;
				return host_.exec_sp_command( this.command_ ) ;
			}
			/// <summary>
			/// Indexer for this class helps in setting and retriving parameter values for the command
			/// </summary>
			public object this[string index] 
			{
				set 
				{
					this.command_.Parameters[index].Value = value ;
				}
				get 
				{
					return this.command_.Parameters[index].Value ;
				}
			}

		} // eof class SP ////////////////////////////////////////////
	} // eof class AdoNet ////////////////////////////////////////
} // eof namespace dbjsrvlib //////////////////////////////////////

