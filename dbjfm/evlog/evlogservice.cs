
namespace dbj.fm.evlog
{
	using System;
	using System.Diagnostics ;
	/// <summary>
	/// Summary description for evlogservice.
	/// </summary>
	internal sealed class service : fm.Ievent_logger
	{
		//-------------------------------------------------------------------------------------------------
		/// <summary>
		/// Contains methods for various types of Messages to be written into event log
		/// </summary>
		internal sealed class LOG
		{
			
			/// <summary>
			/// Determines whether specified event source is registered on a specified computer , 
			/// if does not exists creates event source
			/// </summary>
			/// <param name="src">Event Source Name</param>
			static void check( string src )
			{
				if ( ! EventLog.SourceExists( src , Environment.MachineName ) )
					EventLog.CreateEventSource( src  , core.DBJ_EVLOG_LOGNAME ) ;
			}
			/// <summary>
			/// Writes an information entry with the given message text to the event log, 
			/// using the specified registered event source.
			/// </summary>
			/// <param name="src">Event Source Name</param>
			/// <param name="msg">Message to be written to the event log</param>
			public static void info ( string src, string msg )
			{
				lock ( typeof( internal_log ))
				{
					LOG.check(src) ;
					EventLog.WriteEntry( src, msg, EventLogEntryType.Information ) ;
				}
			}
			/// <summary>
			/// Writes an warning entry with the given message text to the event log, 
			/// using the specified registered event source.
			/// </summary>
			/// <param name="src">Event Source Name</param>
			/// <param name="msg">Message to be written to the event log</param>
			public static void warn ( string src, string msg )
			{
				lock ( typeof( internal_log ))
				{
					LOG.check(src) ;
					EventLog.WriteEntry( src, msg, EventLogEntryType.Warning ) ;
				}
			}
			/// <summary>
			/// Writes an erorr entry with the given exception text to the event log, 
			/// using the specified registered event source.
			/// </summary>
			/// <param name="src">Event Source Name</param>
			/// <param name="x">Excpetion string</param>
			public static void error ( string src, Exception x )
			{
				lock ( typeof( internal_log ))
				{
					LOG.check(src) ;
					EventLog.WriteEntry( src, x.ToString() , EventLogEntryType.Error ) ;
				}
			}
			/// <summary>
			/// Writes an erorr entry with the given message text to the event log, 
			/// using the specified registered event source.
			/// </summary>
			/// <param name="src">Event Source Name</param>
			/// <param name="msg">Message to be written to the event log</param>
			public static void error ( string src, string msg )
			{
				lock ( typeof( internal_log ))
				{
					LOG.check(src) ;
					EventLog.WriteEntry( src, msg , EventLogEntryType.Error ) ;
				}
			}
		}
		//-------------------------------------------------------------------------------------------------
		private service(  )	{ /* no-no ! */	}
		/// <summary>
		/// constructor of service class
		/// </summary>
		/// <param name="s_name">Name of event source</param>
		public service( string s_name )
		{
			source_name_ = s_name ;
		}

		private string source_name_ = string.Empty ;
		/// <summary>
		/// logging can be done for different sources but all will be 
		/// under the same logfile as defined by DBJ_EVLOG_LOGNAME
		/// </summary>
		public override string source_name { get { return source_name_; } }
		/// <summary>
		/// log error message
		/// </summary>
		public override void error ( string message ) { LOG.error( source_name, message); }

		/// <summary>
		/// log the exception
		/// </summary>
		public override void error ( Exception x )  { LOG.error( source_name, x ); }

		/// <summary>
		/// log warning message
		/// </summary>
		public override void warn ( string message ) { LOG.warn( source_name, message); }

		/// <summary>
		/// log info message
		/// </summary>
		public override void info ( string message ) { LOG.info( source_name, message); }
	}
}
