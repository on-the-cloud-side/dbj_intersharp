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

namespace dbj.fm
{
	using System;
	using System.Runtime.InteropServices ;
	using System.Text ;

		
		/// <summary>
		/// The exception class made to be used in this library. 
		/// </summary>
		/// <remarks>This exception may be thrown, by code from this library.
		/// It also automatically sends its message to the event log.
		/// Users of this library can differentiate between other exceptions
		/// and exceptions thrown from inside this library. </remarks>
		/// <example><code>
		/// 
		/// try {
		///			dt.execute("sp_settle");
		///	} catch ( fm.Error e )
		///	{
		///	     ... corelib specific error ...
		///	}		
		///	catch ( Exception x ) 
		///	{
		///	    ... all the other errors ...
		///	}
		/// </code></example>
		public class Error : System.ApplicationException 
		{
			static StringBuilder prefix_ = new StringBuilder("DBJ_CORELIB*ERROR: ") ;

			/// <summary>
			/// Appends given string to prefix defined in here
			/// </summary>
			/// <param name="msg">message to be apppended to prefix</param>
			/// <returns>string</returns>
			static string prefix( string msg ) { return prefix_.Append(msg).ToString(); }

			bool logged_ = false ;
			
			/// <summary>
			/// True if error was logged
			/// </summary>
			public bool isLogged { get { return logged_; }}

			/// <summary>
			/// Default constructor of the Error class
			/// </summary>				
			public Error () : base ( prefix_ + " Unknown" )
			{
				this.HelpLink = "http://fm.dbjsolutions.com/" ;
				if ( this.Source == null )
					this.Source = System.Reflection.Assembly.GetExecutingAssembly().FullName ;
				if ( ! this.isLogged ) logged_ = log(this) ; 
			}
			/// <summary>
			/// Construct the Error class with an arbitrary message.
			/// </summary>
			/// <param name="msg">message </param>
			public  Error(string msg): base(prefix_ + msg) 
			{	
				this.HelpLink = "http://fm.dbjsolutions.com/" ;
				if ( this.Source == null ) this.Source = string.Empty ;
					if ( this.Source == string.Empty )
					this.Source = System.Reflection.Assembly.GetExecutingAssembly().FullName ;
				if ( ! this.isLogged ) logged_ = log(this) ; 
			}
			/// <summary>
			/// Construct the Error class with an arbitray message and an System.Exception instance.
			/// </summary>
			/// <param name="msg">string containing messsage</param>
			/// <param name="the_cause">Exception</param>
			public Error(string msg, Exception the_cause ) :
				base( prefix_ + msg , the_cause) 
			{
				this.HelpLink = "http://fm.dbjsolutions.com/" ;
				if ( this.Source == null ) this.Source = string.Empty ;
				if ( this.Source == string.Empty )
					this.Source = System.Reflection.Assembly.GetExecutingAssembly().FullName ;
				
				if ( the_cause is Error ) this.logged_ = true ; // avoid multiple logging

				if ( ! this.isLogged ) logged_ = log(this) ; 
			}

			/// <summary>
			/// Predefined Error number for DBJ*Corelib errors.
			/// </summary>
			public static int ERR_NUM { get { return 0xFF ; }}

			/// <summary>
			/// logs an error into the event log
			/// </summary>
			/// <param name="x">Error</param>
			/// <returns>boolean value indicating log is successfull or not</returns>
			/// <exception cref=" Error"></exception>
			protected static bool log( Error x )
			{
				lock( typeof( Error ))
				{
					try 
					{
						if ( ! x.isLogged )
							fm.evlog.internal_log.error(x) ;
						x = null ;
						return true ;
					} 
					catch (  Error ) // avoid recursion  and stack overflow !
					{
						return false ;
					}
				}
			}

			//-------------------------------------------------------------------
			/// <summary>
			/// Formats the given exception into the html fragment.
			/// </summary>
			/// <param name="x">Exception to be formated</param>
			/// <returns>string representing exception in the form of html</returns>
			/// <remarks>
			/// This method may be used when Exception ha to be displayed to users on HTML page.
			/// </remarks>
			public static string html_format ( Exception x )
			{
				lock( typeof( Error ))
				{
					try {
					string msg = string.Empty ;
						msg = "<pre>" ;
						msg += "\nMessage: " + x.Message  ;
						msg += "\nSource: " + ( x.Source == null ? Environment.MachineName : x.Source ) ;
						msg += "\nFrom: : " + ( x.TargetSite == null ? " unknown method" : x.TargetSite.Name ) ; 
						msg += "\nHelp Link: " + x.HelpLink ;
						if ( x.InnerException != null )
							msg += " Cause: " + x.InnerException.ToString() ;
						msg += "</pre>" ;
					return msg ;
				} 
				catch (  Error x2 ) // avoid recursion  and stack overflow !
				{
					return string.Format("<pre>{0}</pre>",x2.ToString()) ;
				}
			} // eof lock
			}
			
			/// <summary>
			/// Thrown from methods which are not implemented. Used mostly when developing them.
			/// </summary>
			/// <example><code>
			/// class myClass {
			/// 	void method () {
			///			throw new dbj.fm.NotImplemented() ;
			///		}
			/// }
			/// </code></example>
			public class NotImplemented : Error
			{
				/// <summary>
				/// default conctructor.
				/// </summary>
				public NotImplemented () : base("Not implemented") {}
			}

		} // eof class Error
	}
