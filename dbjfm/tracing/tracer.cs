using System;

namespace dbj.fm.tracing
{
	/// <summary>
	/// Summary description for tracer.
	/// </summary>
	internal class tracer : fm.Itracer 
	{
		private traceservice_implementation imp = null ;
		private System.Reflection.Assembly asm_ = null ;

		private tracer() /* no-no! */	{		}
		/// <summary>
		/// Constructor of tracer class
		/// </summary>
		/// <param name="asm">Reference to the caller assembly</param>
		public tracer(System.Reflection.Assembly asm)
		{
			lock(this)
			{
				this.asm_ = asm ;
				this.imp = new traceservice_implementation(asm, string.Empty ) ;
			}
		}

		~tracer ()
		{
			if ( this.imp != null ) this.imp.Dispose() ;
			this.imp = null ;
			this.asm_ = null ;
		}
		
		/// <summary>
		/// if subclass property is set, the tracefile will be made
		/// in the sub-folder by the subclass name
		/// otherwise it will be in the folder which is named same
		/// as assembly which has made the tracer instance
		/// subclass name string can contain only digits and letters
		/// to prevent sub-sub classing ideas.
		/// </summary>
		public override string subclass 
		{ 
			set{ 
				lock(this)
				{
					string sc = value ;
					// Replace invalid characters,except dot and hyphen, with empty strings.
					sc = System.Text.RegularExpressions.Regex.Replace(sc, @"[^\w\.-]", "");
					if ( sc.Length > 0 )
					this.imp = new traceservice_implementation( this.asm_ , sc ) ;
					else
						throw new fm.Error(value + ", is not good as a subclass name for tracer.") ;
				}
			} 
			get{
				return imp.sub_folder   ;
			} 
		}

		/// <summary>
		/// leave tracing info if level is set to ERROR (1), or higher
		/// Also send the same message to the local evlog.
		/// </summary>
		/// <param name="x">exception instance</param>
		public override void error( Exception x ) 
		{    
			lock(this)
			{
				if ( imp.trace_level < System.Diagnostics.TraceLevel.Error )
					return;

				imp.writeln(x);

				if ( x.GetType().IsSubclassOf( typeof(fm.Error) ))
					return ; // already gone to evlog!

				// CLUDGE begin
				string name = x.Source == null ? string.Empty : x.Source ;
				if ( name == string.Empty ) name = x.TargetSite  == null ? string.Empty :  x.TargetSite.Name ; ;
				if ( name == string.Empty ) name = "Unknown" ;
				evlog.service logger = new evlog.service( name ) ;
				logger.error(x);
				// CLUDGE end
			}
		}

		/// <summary>
		/// leave tracing info if level is set to ERROR (1), or higher
		/// </summary>		
		/// <param name="fmt">fromat string and arguments, or just a string</param> 
		/// <param name="args"></param> 
		public override void error( string fmt, params object [] args )  {
			lock(this)
			{
				if ( imp.trace_level < System.Diagnostics.TraceLevel.Error )
					return;
				if ( args.Length < 1 )
					imp.writeln( fmt ) ;
				else
					imp.writeln(fmt,args) ;
			}
		}
		
		/// <summary>
		/// leave tracing info if level is set to WARNING (2), or higher
		/// </summary>
		/// <param name="fmt">fromat string and arguments, or just a string</param>
		/// <param name="args">object array</param>
		public override void warning( string fmt, params object [] args )  {      
			lock(this)
			{
				if ( imp.trace_level < System.Diagnostics.TraceLevel.Warning )
					return;
				if ( args.Length < 1 )
					imp.writeln( fmt ) ;
				else
					imp.writeln(fmt,args) ;
			}
		}

		/// <summary>
		/// leave tracing info if level is set to INFO (3), or higher
		/// </summary>
		/// <param name="fmt">fromat string and arguments, or just a string</param>
		/// <param name="args" >Object array</param>
		public override void info( string fmt, params object [] args )  {      
			lock(this)
			{
				if ( imp.trace_level < System.Diagnostics.TraceLevel.Info  )
					return;
				if ( args.Length < 1 )
					imp.writeln( fmt ) ;
				else
					imp.writeln(fmt,args) ;
			}
		}
	
		/// <summary>
		/// leave tracing info if level is set to VERBOSE (4)
		/// </summary>
		/// <param name="fmt">fromat string and arguments, or just a string</param>
		/// <param name="args">Object array</param> 
		public override void verbose( string fmt, params object [] args )  {      
			lock(this)
			{
				if ( imp.trace_level < System.Diagnostics.TraceLevel.Verbose )
					return;
				if ( args.Length < 1 )
					imp.writeln( fmt ) ;
				else
					imp.writeln(fmt,args) ;
			}
		}

	} // class tracer
}
