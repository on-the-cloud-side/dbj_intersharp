
namespace dbj.fm.evlog
{
	using System;
	using System.Diagnostics ;
	/// <summary>
	/// Summary description for corelibprivaterrlog.
	/// </summary>
	internal sealed class internal_log
	{
		/// <summary>
		/// Determines whether specified event source is registered on a specified computer , 
		/// if does not exists creates event source
		/// </summary>
		public static void check()
		{
			if ( ! EventLog.SourceExists( core.DBJ_EVLOG_CORELIB_SOURCENAME , Environment.MachineName ) )
				EventLog.CreateEventSource( core.DBJ_EVLOG_CORELIB_SOURCENAME  , core.DBJ_EVLOG_LOGNAME ) ;
		}
		/// <summary>
		/// Writes an iformation entry with the given message text to the event log, 
		/// using the specified registered event source.
		/// </summary>
		/// <param name="msg">Message to be written into event log</param>
		public static void info ( string msg )
		{
			lock ( typeof( internal_log ))
			{
				internal_log.check() ;
				EventLog.WriteEntry( core.DBJ_EVLOG_CORELIB_SOURCENAME, msg, EventLogEntryType.Information ) ;
			}
		}
		/// <summary>
		/// Writes an warning audit entry with the given message text to the event log, 
		/// using the specified registered event source.
		/// </summary>
		/// <param name="msg">Message to be written into event log</param>
		public static void warn ( string msg )
		{
			lock ( typeof( internal_log ))
			{
				internal_log.check() ;
				EventLog.WriteEntry( core.DBJ_EVLOG_CORELIB_SOURCENAME, msg, EventLogEntryType.Warning ) ;
			}
		}
		/// <summary>
		/// Writes an error audit entry with the given message text to the event log, 
		/// using the specified registered event source.
		/// </summary>
		/// <param name="x">Message to be written into event log</param>
		public static void error ( Exception x )
		{
			lock ( typeof( internal_log ))
			{
				internal_log.check() ;
				EventLog.WriteEntry( core.DBJ_EVLOG_CORELIB_SOURCENAME, x.ToString() , EventLogEntryType.Error ) ;
			}
		}
	}
}
