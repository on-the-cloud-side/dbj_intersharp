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

#if LEGACY_ADO

namespace dbj.fm.data
{
using System;
using System.Text ;
using System.Data ;
using System.Runtime.InteropServices ;
using ADODB ;
using System.Data.OleDb ;
			
	internal sealed class ado : dbjsql
	{
		ConnectionClass connection_ ;
		OleDbCommand command_  ;

		private ado() {}

		//default constructor for ado implementation
		public ado(string conn_string_) : base(conn_string_)
		{
			try 
			{
				this.connection_string = conn_string_ ;
				connection_ = new ConnectionClass() ;
				command_ = new OleDbCommand() ;
				//			parameter_ = new ParameterClass() ;
			}
			catch(	System.Exception  e )
			{
				if (e != null) throw new Exception(e.Message) ;
			}
		}

		public override object[] get_table_names(Idata.TableType ttype ) 
		{
			throw new fm.Error.NotImplemented() ;
		}
		
		/// <summary>
		/// for COM clients
		/// </summary>
		public override void init(string new_cs)
		{
			this.connection_string = new_cs ;
		}
		// execute the SQL INSERT statement
		public override void insert(string statement)
		{
			this.check() ;

			statement = statement.ToLower() ;
			if(statement.IndexOf("insert") == -1)
				throw new Exception("Invalid SQL query");
			System.Object res = new System.Object() ;
			try
			{
				this.connection_.Open(this.connection_string,null,null,0);
				this.connection_.Execute(statement,out res,0) ;
			}
			catch(	System.Exception  e )
			{
				if (e != null) throw new Exception(e.Message) ;
			}
			finally
			{
				this.close_connection() ;
			}
		}
		// execute the SQL UPDATE statement
		public override void update(string statement)
		{
			this.check() ;

			statement = statement.ToLower() ;
			if(statement.IndexOf("update") == -1)
				throw new Exception("Invalid SQL query");
			System.Object  res = new System.Object() ;
			try
			{
				this.connection_.Open(this.connection_string,null,null,0);
				this.connection_.Execute(statement,out res,0) ;
			}
			catch(System.Exception e)
			{
				if (e != null) throw new Exception(e.Message) ;
			}
			finally
			{
				this.close_connection() ;
			}
		}
		// execute the SQL DELETE statement
		public override void erase(string statement)
		{
			this.check() ;

			statement = statement.ToLower() ;
			if(statement.IndexOf("delete") == -1)
				throw new Exception("Invalid SQL query");
			System.Object  res = new System.Object() ;
			try
			{
				this.connection_.Open(this.connection_string,null,null,0);
				this.connection_.Execute(statement,out res,0) ;
			}
			catch(	System.Exception  e )
			{
				if (e != null) throw new Exception(e.Message) ;
			}
			finally
			{
				this.close_connection() ;
			}
		}
		// execute the SQL EXEC statement
		public override int exec(string statement)
		{
			this.check() ;

			statement = statement.ToLower() ;
			if(statement.IndexOf("exec") == -1)
				throw new Exception("Invalid SQL query");
			int result = 0;
			System.Object  res = new System.Object() ;
			try
			{
				this.connection_.Open(this.connection_string,null,null,0);
				this.connection_.Execute(statement,out res,0) ;
				result = System.Convert.ToInt32(res) ;
			}
			catch(	System.Exception  e )
			{
				if (e != null) throw new Exception(e.Message) ;
			}
			finally
			{
				this.close_connection() ;
			}
			return result;
		}
		// invokes a visitor argument for each row of the current result
		public override void for_each_row(visitor theVisitor, string select_statement)
		{
			this.check() ;

			try 
			{
				DataTable newTable = new DataTable();
				System.Data.OleDb.OleDbDataAdapter adapter = new System.Data.OleDb.OleDbDataAdapter();
				_Recordset result = this.rs_open( select_statement,true ) ;
				
				adapter.Fill(newTable,result) ;
				DataRowCollection rows_ = newTable.Rows ;

				for ( int j = 0 ; j < rows_.Count ; j++ )
				{
					theVisitor( rows_[j] ) ;
				}
			} 
			catch(	System.Exception e )
			{
				if(e != null) throw new Exception(e.Message) ;
			}
		}
		// execute the SQL SELECT statement
		public _Recordset select(string statement,bool disconnect)
		{
			statement = statement.ToLower() ;
			if(statement.IndexOf("select") == -1)
				throw new Exception("Invalid SQL query") ;

			_Recordset result = this.rs_open(statement,disconnect) ;
			return result ;
		}

		public void close_recordset(_Recordset result)
		{
			result.Close() ;
		}

		public void close_connection()
		{
			if(this.connection_.State != (int)ObjectStateEnum.adStateClosed)
				this.connection_.Close() ;
		}

		/// <summary>
		/// returns recordset for stored procedure executed
		/// </summary>
		public _Recordset exec_for_recordset(string statement,bool disconnect)
		{
			statement = statement.ToLower() ;
			if(statement.IndexOf("exec") == -1)
				throw new Exception("Invalid SQL query") ;
			
			_Recordset result = this.rs_open(statement,disconnect) ;
			return result;
		}
		//makes and returns ado recordset
		_Recordset rs_open(string statement,bool disconnect)
		{
			this.check() ;

			statement = statement.ToLower() ;
			RecordsetClass result = new RecordsetClass() ;
			try
			{	
				result.CursorLocation = CursorLocationEnum.adUseClient  ;
				this.connection_.Open(this.connection_string,null,null,0);
				this.connection_.BeginTrans() ;
				result.Open(
					statement,
					this.connection_string,
					CursorTypeEnum.adOpenStatic, 
					LockTypeEnum.adLockBatchOptimistic,
					(int)CommandTypeEnum.adCmdText 
					) ;
				//if true, disconnect the recordset
				if(disconnect)
				{
					System.Object notcon = null ;
					result.ActiveConnection = notcon ;
				}			
			}
			catch(	System.Exception  e )
			{
				if (e != null)
				{
					this.connection_.RollbackTrans() ;
					this.connection_.Close() ;
					throw new Exception(e.Message) ;
				}
			}
			finally
			{
				this.connection_.CommitTrans() ;
				this.connection_.Close() ;
			}
			return result ;
		}

		/// <summary>
		/// Returns names of tables or views.
		/// </summary>
		/// <param name="db_obj_type">must be 'TABLE' or 'VIEW'</param>
		/// <returns>array of names</returns>
		public object[] get_table_names(string table_type)
		{
			System.Collections.ArrayList retval = null ;
			table_type = table_type.ToUpper() ;
			try 
			{
#if DEBUG
				if ( ( table_type != "BASE TABLE" )  && ( table_type != "VIEW" )
					)
					throw new fm.Error( table_type + " is wrong argument, must be 'BASE TABLE' or 'VIEW' " ) ;
#endif
				System.Data.OleDb.OleDbDataAdapter schemaDA = null ;
				DataTable schemaTable = null ;
				
				schemaDA = new System.Data.OleDb.OleDbDataAdapter("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES " +
					"WHERE TABLE_TYPE = '"+ table_type +"'" , 
					this.connection_string);
				schemaTable = new DataTable();
				schemaDA.Fill(schemaTable);

				retval = new System.Collections.ArrayList() ;
				foreach(DataRow r in schemaTable.Rows)
					retval.Add(r[0]) ;
				return retval.ToArray() ;
			} 
			catch ( Exception x) 
			{ throw new fm.Error("ado.get_table_names(" + table_type +")",x); 
			}
		}
	
	

		/// <summary>
		/// executes whatever sql query passed as an argument
		/// </summary>
		public void execute(string statement)
		{
			this.check() ;

			System.Object  res = new System.Object() ;
			try
			{
				this.connection_.Open(this.connection_string,null,null,0);
				this.connection_.Execute(statement,out res,0) ;
				
			}
			catch(	System.Exception  e )
			{
				if (e != null) throw new Exception(e.Message) ;
			}
			finally
			{
				this.close_connection() ;
			}	
		}

		/// <summary>
		/// returns Parameter object 
		/// </summary>
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
				throw new fm.Error("Cannot add parameter to command object") ;
			}
		}
		
		/// <summary>
		/// Adds parameter of varchar type to command object
		/// </summary>
		public void add_param_VARCHAR(string param_name, object param_value,int param_size,ParameterDirection param_dir)
		{
			OleDbType param_type = OleDbType.VarChar ;
			this.add_parameter( param_name, param_type, param_dir, param_value, param_size );		
		}

		/// <summary>
		/// Adds parameter of integer type to command object
		/// </summary>
		public void add_param_INT(string param_name, object param_value,ParameterDirection param_dir)
		{
			OleDbType param_type = OleDbType.Integer ;
			this.add_parameter( param_name, param_type, param_dir, param_value, 0 );
		}
		
		/// <summary>
		/// Adds parameter of datetime type to command object
		/// </summary>
		public void add_param_DATETIME(string param_name, object param_value,ParameterDirection param_dir)
		{
			OleDbType param_type = OleDbType.DBTimeStamp ;
			this.add_parameter( param_name, param_type, param_dir, param_value, 0 );
		}
		
		/// <summary>
		/// Adds parameter of float type to command object
		/// </summary>
		public void add_param_FLOAT(string param_name, object param_value,ParameterDirection param_dir)
		{
			OleDbType param_type = OleDbType.Double ;
			this.add_parameter( param_name, param_type, param_dir, param_value, 0 );
		}
		
		/// <summary>
		/// Adds parameter of bool type to command object
		/// </summary>
		public void add_param_BOOL(string param_name, object param_value,ParameterDirection param_dir)
		{
			OleDbType param_type = OleDbType.Boolean ;
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
		/// executes command object and returns value from stored procedure
		/// </summary>
		public int exec_sp_from_command(string sp_name)
		{
			OleDbConnection newconnection = new OleDbConnection(this.connection_string);
			OleDbTransaction newtrans  = null;
			try
			{
				this.command_.CommandText = sp_name ;
				this.command_.CommandType = CommandType.StoredProcedure ;
				this.command_.Connection = newconnection ;
				this.command_.Connection.Open() ;
				//begin transaction
				newtrans = this.command_.Connection.BeginTransaction() ;
				this.command_.Transaction = newtrans ;
				this.command_.ExecuteNonQuery() ;						
				newtrans.Commit() ;
			}
			catch(OleDbException x)
			{
				newtrans.Rollback() ;
				throw new fm.Error(x.Message) ;
			}
			catch(Exception err)
			{
				newtrans.Rollback() ;
				throw new fm.Error(err.Message) ;
			}
			finally
			{	
				newconnection.Close();
			}
			return (int)this.command_.Parameters[0].Value ;
		}	
	}
}

#endif 