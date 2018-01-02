#region source_file_header
//
// (c) 2003-2005 by DBJ*Solutions Ltd.
//
// $Revision: 1 $
// Last edited by $Author: dusan $
// $JustDate: 30.06.06 $
//
#endregion


namespace dbj.fm.tracing
{
	using System;
	using System.Diagnostics ;
	using System.Xml ;

	#region trace switch that uses AssemblySettings 

	internal sealed class AssemblyTraceSwitch 
	{
		private string id_ = string.Empty ;
		private string description_ = string.Empty ;
		private System.Diagnostics.TraceLevel level_ ;

		static string trace_style_type_key = "trace_style_type" ;
		static string trace_style_file_key  = "trace_style_file" ;
		private string trace_style_type_val = string.Empty ;
		private string trace_style_file_val  = string.Empty ;


		AssemblyTraceSwitch( ){}
		
		/// <summary>
		/// Constructor of AssemblyTraceSwitch class
		/// </summary>
		/// <param name="asm">calling assembly</param>
		/// <param name="id">trace level</param>
		/// <param name="description">description</param>
		/// <exception cref=" System.Exception">On error throws System.Exception</exception>	
		public AssemblyTraceSwitch( System.Reflection.Assembly asm,  string id, string description ) 
		{
			try 
			{
				this.id_  = id ; 
				this.description_ = description ;

				fm.Iconfiguration cfg = fm.core.make_service( ServiceID.Configuration , asm ) ;

				trace_style_type_val = cfg.get_value( trace_style_type_key ) ;
				trace_style_file_val = cfg.get_value( trace_style_file_key ) ;

				int trace_level = cfg.get_value( id ) ;
				/*
				Trace Level Configuration File Value 
				Off			0 
				Error		1 
				Warning		2 
				Info		3 
				Verbose		4 
				*/
				switch ( trace_level ) 
				{
					case 0:
						this.Level = System.Diagnostics.TraceLevel.Off ;
						break ;
					case 1:
						this.Level = System.Diagnostics.TraceLevel.Error ;
						break ;
					case 2:
						this.Level = System.Diagnostics.TraceLevel.Warning  ;
						break ;
					case 3:
						this.Level = System.Diagnostics.TraceLevel.Info  ;
						break ;
					case 4:
						this.Level = System.Diagnostics.TraceLevel.Verbose  ;
						break ;
					default :
						// if anything goes wrong switch of the tracing
						this.Level = System.Diagnostics.TraceLevel.Off ;
						break;
				}
				cfg = null ;
			} 
			catch ( Exception )
			{
				// if anything goes wrong switch of the tracing
				this.Level = System.Diagnostics.TraceLevel.Off ;
			}

		}
		/// <summary>
		/// returns trace level
		/// </summary>
		public string DisplayName { get { return this.id_ ; } }
		/// <summary>
		/// returns description
		/// </summary>
		public string Description { get { return this.description_ ; } }

		/// <summary>
		/// sets or gets trace level
		/// </summary>
		public System.Diagnostics.TraceLevel Level
		{
			get { return this.level_ ; }
			set { this.level_ = value ; }
		}

		/*
		string PItext="type='text/css' href='trace.css'";
		xmlwriter.WriteProcessingInstruction("xml-stylesheet", PItext);
		*/
		/// <summary>
		/// returns a formated string 
		/// </summary>
		/// <param name="href_prefix"></param>
		/// <returns>string</returns>
		public string style_instruction_pitext ( string href_prefix )
		{
			if ( this.trace_style_file_val != string.Empty ) 
			if ( this.trace_style_type_val != string.Empty )
			{
				return string.Format(" type='{0}' href='{1}' ",  this.trace_style_type_val , href_prefix + this.trace_style_file_val ) ;
			}
				return string.Empty ;
		}
	}


	#endregion

	#region implementation of the trace service
	/// <summary>
	/// Summary description for traceservice.
	/// </summary>
	internal sealed class traceservice_implementation : IDisposable
	{
		//------------------------------------------------------------------------------
		// a TraceSwitch to use in the entire application.
		AssemblyTraceSwitch theSwitch = null ;
		/// <summary>
		/// returns trace level
		/// </summary>
		public System.Diagnostics.TraceLevel trace_level { 
			get { return this.theSwitch.Level ; } 
		}

		//------------------------------------------------------------------------------
		/// <summary>
		/// returns timestamp of the format yyyyMMddHHmmss
		/// </summary>
		/// <returns></returns>
		public static string timestamp ()
		{
			return util.timestamp() ; // ISO standard timestamp
									  // "yyyyMMddHHmmss"
		}
		//------------------------------------------------------------------------------
		/// <summary>
		/// name of trace file is made using the assembly name and subfolder
		/// </summary>
		/// <param name="asm">reference to calling assembly</param>
		/// <param name="sub_folder_name">name of the sub folder</param>
		/// <returns>string representing path to the log file</returns>
		public static string log_file_full_path ( System.Reflection.Assembly asm, string sub_folder_name )
		{
			string tstamp = timestamp() ;
			string asmname = asm.GetName().Name ;
			string trace_folder_path =  System.IO.Path.GetDirectoryName( asm.CodeBase ) ;
			
			Uri log_folder_uri = null ;
			
			if ( sub_folder_name != string.Empty ) 
			log_folder_uri =new Uri( 
				trace_folder_path + @"\trace_" + asmname + @"\" + sub_folder_name
				) ;
			else
				log_folder_uri =new Uri( 
					trace_folder_path + @"\trace_" + asmname 
					) ;

			trace_folder_path =  log_folder_uri.LocalPath ;
			
			System.IO.Directory.CreateDirectory( trace_folder_path ) ;

			//obtains file count 
			int file_count = checkFileExits ( trace_folder_path , tstamp ) ;

			Uri log_uri = new Uri( trace_folder_path + @"\" + asmname + "_" + tstamp + "_" + file_count +  ".xml" ) ;
			return log_uri.LocalPath ;
		}
		//------------------------------------------------------------------------------
		/// <summary>
		/// Checks for file existence and returns the count of files exists
		/// </summary>
		/// <param name="path">path of the folder to search for the files of specified format</param>
		/// <param name="tstamp">timestamp</param>
		/// <returns>int representing count of files with same time stamp</returns>
		public static int checkFileExits(string path,string tstamp )
		{
			int file_count = 0 ;
			string[] files = System.IO.Directory.GetFiles( path,"*"+tstamp+".xml") ;
			if(files != null)
				file_count = files.Length  ;			
			return file_count ;
		}
		//------------------------------------------------------------------------------
		System.Reflection.Assembly	asm			= null ;
		System.Xml.XmlTextWriter	xmlwriter	= null ;
		// System.IO.FileStream		fs			= null ;
		string						file_path	= string.Empty ;	

		//------------------------------------------------------------------------------
		private string sub_folder_name_ = string.Empty ;
		/// <summary>
		/// returns name of the sub folder
		/// </summary>
		public string sub_folder 
		{
			get {return this.sub_folder_name_ ; }
		}
		//------------------------------------------------------------------------------
		private traceservice_implementation () {} // no-no!

		//------------------------------------------------------------------------------
		/// <summary>
		/// Constructor of traceservice_implementation  class
		/// </summary>
		/// <param name="calling_assembly"> reference to calling assembly</param>
		/// <param name="sub_folder">name of the subfolder</param>
		public traceservice_implementation( 
			System.Reflection.Assembly calling_assembly, 
			string sub_folder )
		{
			System.Diagnostics.Debug.Assert( sub_folder != null ) ;
			this.asm = calling_assembly ;
			this.sub_folder_name_ = sub_folder ;
			this.file_path  = log_file_full_path(asm, sub_folder );
			this.theSwitch = new AssemblyTraceSwitch( asm, "trace_level",  asm.GetName().Name + " Assembly trace switch");
		}

		//------------------------------------------------------------------------------
		private bool opened_ = false ;
		/// <summary>
		/// creates logfile
		/// </summary>
		private void open ()
		{
			if ( this.opened_ ) return ;
			try 
			{
				if ( theSwitch.Level > TraceLevel.Off ) 
				{
					xmlwriter = new System.Xml.XmlTextWriter( this.file_path,null ) ;
					xmlwriter.Formatting = System.Xml.Formatting.Indented;
					xmlwriter.WriteStartDocument() ;
					//Write the ProcessingInstruction node.
					string PItext= theSwitch.style_instruction_pitext( string.Empty ) ;
					if ( PItext != string.Empty )
						xmlwriter.WriteProcessingInstruction("xml-stylesheet", PItext);

					xmlwriter.WriteStartElement("trace") ;
					xmlwriter.WriteAttributeString( "timestamp" , timestamp() ) ;
					xmlwriter.WriteAttributeString( "assembly" , this.asm.FullName ) ;
					xmlwriter.WriteStartElement("switch") ;
					// xmlwriter.WriteAttributeString( "displayname" , theSwitch.DisplayName) ;
					// xmlwriter.WriteAttributeString( "description" , theSwitch.Description ) ;
					xmlwriter.WriteAttributeString( "level" , theSwitch.Level.ToString() ) ;
					xmlwriter.WriteEndElement(); // close 'switch'
				}
					this.opened_ = true ;
			} 
			catch ( Exception x )
			{
				evlog.internal_log.error(x); x = null ;
			}
			finally 
			{
				if ( this.xmlwriter != null ) xmlwriter.Close(); this.xmlwriter = null ;
			}
		}
		//------------------------------------------------------------------------------
		/// <summary>
		/// destructor of traceservice_implementation class
		/// </summary>
		~traceservice_implementation ()
		{
			Dispose(false);
		}

		//------------------------------------------------------------------------------
		/// <summary>
		/// closes the trace
		/// </summary>
		void close ()
		{
			try 
			{
				if ( theSwitch.Level == TraceLevel.Off ) return ;
				//Trace.Unindent() ;
				//Trace.Flush();
				Trace.Close();
			}
			catch ( Exception x)
			{
				evlog.internal_log.error(x); x = null ;
			}
			finally 
			{
			}
		}

		//------------------------------------------------------------------------------
		// overload for exception output
		/// <summary>
		/// used for writting details of exception to trace file
		/// </summary>
		/// <param name="x">Exception</param>
		public void writeln( Exception x )
		{
			if ( theSwitch.Level == TraceLevel.Off ) return ;
			try 
			{
				this.open() ;
				XmlDocument xml_doc=new XmlDocument();
				xml_doc.Load(this.file_path);
				XmlElement ele_line=xml_doc.CreateElement("line"); 
				XmlAttribute att_time=xml_doc.CreateAttribute("timestamp"); 
				att_time.Value =timestamp() ;
				ele_line.SetAttributeNode(att_time);
				XmlElement ele_error=xml_doc.CreateElement("error"); 
				XmlAttribute att_help=xml_doc.CreateAttribute("help_link"); 
				att_help.Value = x.HelpLink == null ? "null" : x.HelpLink  ;
				ele_error.SetAttributeNode(att_help);
				XmlAttribute att_source = xml_doc.CreateAttribute("source"); 
				att_source.Value = x.Source == null ? "null" : x.Source  ;
				ele_error.SetAttributeNode(att_source);
				XmlAttribute att_target=xml_doc.CreateAttribute("target_site"); 
				att_target.Value = x.TargetSite == null ? "null" :  x.TargetSite.ToString()  ;
				ele_error.SetAttributeNode(att_target);
				XmlCDataSection cd_section=xml_doc.CreateCDataSection(x.ToString());
				ele_error.AppendChild(cd_section);
				ele_line.AppendChild(ele_error);
				xml_doc.DocumentElement.AppendChild(ele_line);
				xml_doc.Save(this.file_path);
				xml_doc = null ;
			} 
			catch ( Exception xx ) 
			{
				evlog.internal_log.error(xx) ; xx = null ;
			}
		}
		//------------------------------------------------------------------------------
		/// <summary>
		/// Writes given message to the trace file
		/// </summary>
		/// <param name="msg"></param>
		public void writeln( string msg )
		{
			if ( theSwitch.Level == TraceLevel.Off ) return ;
			try 
			{
				this.open() ;
				XmlDocument xml_doc=new XmlDocument();
				xml_doc.Load(this.file_path);
				XmlElement ele_line=xml_doc.CreateElement("line"); 
				XmlAttribute att_time=xml_doc.CreateAttribute("timestamp"); 
				att_time.Value =timestamp() ;
				ele_line.SetAttributeNode(att_time);

				// do this so that CDATA section may be created from embded CDATA section
				if ( msg.IndexOf("CDATA") < 0 )
				{
					XmlCDataSection cd_section=xml_doc.CreateCDataSection( msg );
					ele_line.AppendChild(cd_section);
				} 
				else 
				{
					XmlComment  cm_section = xml_doc.CreateComment( msg );
					ele_line.AppendChild(cm_section);
				}

				xml_doc.DocumentElement.AppendChild(ele_line); 
				xml_doc.Save(this.file_path);
				xml_doc = null ;
			} 
			catch ( Exception x ) {   evlog.internal_log.error(x); x = null ;
			}
		}

		//------------------------------------------------------------------------------
		/// <summary>
		/// Writes message to the trace file
		/// </summary>
		/// <param name="fmt">format of the string</param>
		/// <param name="args">object array</param>
		public void writeln( string fmt, params object [] args )
		{
			this.writeln( string.Format( fmt, args) ) ;
		}
		#region IDisposable Members

		bool disposed_ = false ;
		public void Dispose()
		{
			lock ( this )
			{
				Dispose(true);
				// GC.SuppressFinalize(this);
			}
		}

		public void Dispose( bool who_called )
		{
			try 
			{
				if ( ! this.opened_ ) return ;
				if ( disposed_ ) return ;
				if ( theSwitch.Level == TraceLevel.Off ) return ;

				XmlDocument xml_doc=new XmlDocument();
				xml_doc.Load(this.file_path);
				XmlComment xml_coment=xml_doc.CreateComment( "Tracing closed at : " + timestamp() ) ;
				xml_doc.DocumentElement.AppendChild(xml_coment);
				xml_doc.Save(this.file_path);
				xml_doc = null ;
				/*
				try 
				{
					if ( xmlwriter != null ) 
					{ // yes, this is possible
						if ( xmlwriter.WriteState != System.Xml.WriteState.Closed )
						{
							xmlwriter.WriteComment( "Tracing closed at : " + timestamp() ) ;
							xmlwriter.WriteEndElement();
							xmlwriter.WriteEndDocument() ;
						}
					}
				} 
				catch ( Exception x)
				{
					string debug = x.ToString() ;
				}
				*/
				close();
			}
			catch ( Exception x)
			{
				evlog.internal_log.error(x); x = null ;
			}
			finally 
			{
				this.disposed_ = true ;
			}
		}

		#endregion
	}

	#endregion
}





