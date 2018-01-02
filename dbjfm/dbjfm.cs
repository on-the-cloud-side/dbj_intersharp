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

using System;
using System.Diagnostics;
using System.IO;

namespace dbj.fm
{
	#region interfaces to services this library provides

	/// <summary>Reflection services.
	/// </summary>
	/// <remarks>
	/// <p>Implementatio of this class encapsualtes the issues of getting the reflection
	/// information on the arbitrary .NET type.</p>
	/// <p>Instance of the implementation may me obtained only through core.make_service() method</p>
	/// <p>The information in the finished 'report' is not structured or user definable in any way.</p>
	/// <p>It is simply a string for end users. Use of this functionality is mainly limited to 
	/// testing and diagnostic tools.</p>
	/// <p>Making of this service through core.make_service() method requires no additional parameters.</p>
	/// </remarks>
	/// <example><code>
	/// dbj.fm.Itypeinfo ti_ = dbj.fm.core.make_service( 
	///                                      dbj.fm.ServiceID.Reflection
	///                                      ) ;
	/// </code></example>
	public abstract class Itypeinfo
	{
		/// <summary>
		/// Enum of flags used by callers to describe, what the caller wants to be described
		/// </summary>
		[Flags]
		public enum WHAT // to describe
		{ 
			/// <summary>
			/// show fields only
			/// </summary>
			FIELDS = 1, 
			/// <summary>
			/// show methods only
			/// </summary>
			METHODS = 2, 
			/// <summary>
			/// show properties only
			/// </summary>
			PROPERTIES = 4, 
			/// <summary>
			/// show interfaces only
			/// </summary>
			INTERFACES = 8, 
			/// <summary>
			/// show the parent class only
			/// </summary>
			PARENT = 16, 
			/// <summary>
			/// show the result of Type.Attributes
			/// </summary>
			ATTRIBUTES = 32 ,
			/// <summary>
			/// display also methods on interfaces
			/// </summary>
			INTERFACE_METHODS = 64 ,
			/// <summary>
			/// show everything available
			/// </summary>
			ALL = FIELDS | METHODS | PROPERTIES | INTERFACES | PARENT
		}

		/// <summary>
		/// The only methods on this interface for getting the description.
		/// </summary>
		/// <param name="specimen">instance to be described</param>
		/// <param name="what_part">or-ed enum values teling us what caller wants described</param>
		/// <returns>report in a string</returns>
		abstract public string describe ( object specimen, WHAT what_part ) ;
	}


	/// <summary>
	/// Data services.
	/// </summary>
	/// <remarks>
	/// Implementatio of this class encapsualtes the issues of getting the data
	/// from the database in a simple and un-cluttered manner. Manner in which more than 90%
	/// of code uses the databases.
	/// <p>Instance of the implementation may me obtained only through core.make_service() method</p>
	/// <p>To obtain data library specific functionality users may instantiate their implementations
	/// directly, as available in the fm.data namespace.</p>
	/// <p>Making of this service through core.make_service() method requires two additional parameterers</p>
	/// <p>First is a flag telling us which data library caller want to use</p>
	/// <p>Second is an connection string which is applicable to (usable by) implementation required.</p>
	/// </remarks>
	/// <example><code>
	///     fm.Idata db_ = fm.core.make_service( 
	///                                      fm.ServiceID.Data,
	///                                      fm.Idata.SQLSRV ,
	///                                      connection_string // made elsewhere
	///                                      ) ;
	/// </code></example>
	public abstract class  Idata
	{
		/// <summary>
		/// values of this enum define implementations provided.
		/// NOTE: ADO indeed covers both Oracle and SQLServer
		/// but is slower and provides legacy objects whic take
		/// more memory and give less functionality, vs .NET System.Data
		/// </summary>
		public enum Store { 
			/// <summary>
			/// Use ADO 
			/// </summary>
			ADO, 
			/// <summary>
			/// Use native SQLSRV library
			/// </summary>
			SQLSRV, 
			/// <summary>
			/// Use ORACLE library. Reserved. Not implemented yet.
			/// </summary>
			ORACLE }

		/// <summary>
		/// Values of this enum define types of tables like table ,view etc
		/// </summary>
		public enum TableType { 
			/// <summary>
			/// Use table
			/// </summary>
			TABLE, 
			/// <summary>
			/// Use view
			/// </summary>
			VIEW }

		/// <summary>
		/// Return all tables names from the underlying database.
		/// </summary>
		/// <param name="ttype">Idata.TableType</param>
		/// <returns>an array consisting of table types</returns>
		public abstract object[] get_table_names(Idata.TableType ttype ) ;
		/// <summary>
		/// execute the SQL INSERT statement 
		/// </summary>
		public abstract void insert(string statement)  ; 
		
		/// <summary>
		/// execute the SQL UPDATE statement 
		/// </summary>
		public abstract void update(string statement)  ; 
		
		/// <summary>
		/// execute the SQL DELETE statement 
		/// </summary>
		public abstract void erase(string statement)   ; 
		
		/// <summary>
		/// execute the SQL EXEC statement 
		/// </summary>
		public abstract int exec(string statement) ;

		/// <summary>
		/// init must be implemented because of COM clients
		/// </summary>
		public abstract void init(string init_str)     ; 
		
		/// <summary>
		/// invoke a visitor argument for each row of the result of the SQL select statement given.
		/// <seealso cref="dbj.fm.data.visitor" />
		/// </summary>
		/// <example><code>
		///     fm.Idata dta = core.get_service( dbj.fm.ServiceID.Data , dbj.fm.Idata.Store.SQLSRV , connection_string ) ;
		/// 	const string statement = "SELECT * FROM customers" ;
		///		Collector collector = new Collector() ;
		///		// Collector has to implement public void collect ( object the_row ), method
		///		dbj.fm.data.visitor db_visitor = new dbj.fm.data.visitor( collector.collect ) ;
		///		dta.for_each_row( db_visitor, statement ) ;
		/// </code></example>
		public abstract void for_each_row( fm.data.visitor vinvoke, string select_statement)  ; 

	}

#if INCLUDE_DEPRECATED
	/// <summary>
	/// make Tranzax 'friendly' error node from the 'foreign' xml nodes
	/// or exceptions or whatever is used.
	/// </summary>
	internal abstract class  Ierror_handler 
	{
		/// <summary>
		/// return xml representing error as required or understood by tranzax 
		/// </summary>
		/// <param name="foreign_xml_error">xml presentation of the error from an external system</param>
		/// <returns>xml in a string</returns>
		public abstract string make_tranzax_error_node ( string foreign_xml_error ) ;

		/// <summary>
		/// process the system error by logging it to the event log or whatever is necessary
		/// </summary>
		/// <param name="x">the exception instance</param>
		/// <returns>int value representing BAD or GOOD, in the context of the system for which the implementation is made.</returns>
		public abstract int tranzax_system_error ( System.Exception x ) ;
	}
#endif

	/// <summary>
	/// Configuration service.
	/// </summary>
	/// <remarks>
	/// <p>An extension to the .NET configuration mechanism.
	/// Opposite of the default configuration available in .NET this service makes per assembly 
	/// configuration files possible. </p>
	/// <p>Assembly configuration file name is made by following the same pattern as application configuration files. 
	/// If assembly output is called : </p>
	/// <p><pre>myAssembly.dll</pre></p>
	/// <p>Its configuration must be stored in a file named:</p>
	/// <p><pre>myAssembly.dll.config</pre></p>
	/// <p>
	/// Dbj configuration files are defined per-assembly. 
	/// Each of them can have one or more dbj_config nodes.
	/// Each of them must have a name attribute. Only one of them can be
	/// without name attribute and is treated specially.
	/// Configuration nodes used by this mechanism (and this library) MUST be called :</p>
	/// <p><b><pre>dbj_config</pre></b></p>
	/// <p>XML schema of the unnamed dbj_config node is the same as for normal .NET config files, settings node.</p>
	/// <p>See the example configuration file bellow.</p>
	/// </remarks>
	/// <example><code>
	/// 
	///         // this is how dbj configuration service is instantiated
	///         // in your code
	/// 		dbj.fm.Iconfiguration cfg = core.get_service( 
	///					dbj.fm.ServiceID.Configuration , 
	///					// Always use GetCallingAssembly() only !
	///					System.Reflection.Assembly.GetCallingAssembly()
	///				) ;
	///				
	/// </code>
	/// <code escaped='true'>
	/// 
	/// <!-- 
	///      Please use this sample configuration file when observing 
	///      particular configuration methods description.
	/// -->
	/// <configuration>
	/// <!-- there can be only ONE unnamed dbj_config node -->
	/// <dbj_config>
	/// <!-- This keys are MANDATORY for the dbj.fm tracer mechanism -->
	/// <add key="trace_level" value="4" />
	/// <add key="trace_style_type" value="text/xsl" />
	/// <add key="trace_style_file" value="../dbjtraceshow.xsl" />
	/// </dbj_config>
	/// <!-- example of a named dbj_config node -->
	///		<dbj_config name="endpoints" >
	/// 		 <endpoint>
	///				<progid>dbj.ep5001</progid>
	///			</endpoint>
	///		</dbj_config>
	/// </configuration>
	/// 
	/// </code></example>
	public abstract class Iconfiguration
	{
		/// <summary>
		/// name of the config file implmentation wants.
		/// it depends on the assembly making it.
		/// </summary>
		public abstract string file_name { get ; }

		/// <summary>
		/// the name of the config node which is going to be processed by this service.
		/// Other nodes are ignored.
		/// </summary>
		public abstract string config_node_name ( ) ;

		/// <summary>
		/// get value assoicated with a key :
		/// </summary>
		/// <param name="key">the key name</param>
		/// <returns>fm.Implicitor object that allows for transparent casting</returns>
		/// <example><code>
		/// ...
		/// <!-- the config file -->
		/// <assemblySettings>
		/// <add key="tracing" value="true" />
		/// </assemblySettings>
		/// ...
		/// bool tracing_switch = get_value("tracing") ;
		/// // get_value() returns fm.Implicitor
		/// // tracing_switch resolves transparently to boolean true
		/// </code></example>
		public abstract fm.Implicitor get_value ( string key ) ;

		/// <summary>
		/// get every config node as xmlnode regardles of its kind
		/// </summary>		
		/// <returns>list of XML nodes</returns>
		public abstract System.Xml.XmlNodeList get_asxml( );
	
		/// <summary>
		/// get every config node where attrib name eq givne name as xmlnode regardles of its kind
		/// </summary>		
		/// <param name="name">attribute value in tranzax_settings node for which 
		/// xmlnodelist has to be retrived</param> 
		/// <returns>list of XML nodes</returns>
		public abstract System.Xml.XmlNodeList get_asxml( string name );

	}


	/// <summary>
	/// xml transformations service interface.
	/// </summary>
	public abstract class Itransformer
	{
		/// <summary>
		/// apply xsl on the xml request when sending out of the system
		/// </summary>
		/// <param name="xml_request">xml in a string</param>
		/// <returns>transformed xml in a string</returns>
		public abstract string transform_for_request ( string xml_request );

		/// <summary>
		/// apply xsl on the xml request when replying the hosting system
		/// </summary>
		/// <param name="xml_reply">xml in a string</param>
		/// <returns>transformed xml in a string</returns>
		public abstract string transform_for_reply   ( string xml_reply ) ;
	}

	
	/// <summary>
	/// Tracing service interface.
	/// </summary>
	/// <remarks>
	/// <p>The tracing information is saved in xml files. The tracing is independent for each 
	/// calling assembly using it.</p>
	/// <p>If folder using this service is called myAssembly then the trace files are 
	/// saved in the sub follder named 'trace_myAssembly'</p>
	/// <p>Each trace xml file name will be made like this:</p>
	/// <pre>
	/// myAssembly_20050225172000_0.xml
	/// </pre>
	/// <p>Assembly name + ISO time stamp + "_" + trace sequence number.</p>
	/// <p>Please see the example bellow for the internals of a sample trace file.
	/// The dbjtraceshow.xsl, is provided with a library as a default xsl file, that may be used.
	/// </p><p>Please feel free to write your own xsl to display trace files in a browser of your choice.</p>
	/// <p>The behaviour of the service depends on configuration file settings.</p>
	/// <p>There are three keys which are mandatory for each config file. 
	/// They define the dbj tracing service behaviour.
	/// They must be inside the anonymous (unnamed) dbj_config node</p>
	/// <pre>
	/// trace_level                 see the possible values bellow<br />
	/// trace_style_type"           always: "text/xsl"<br />
	/// key="trace_style_file"		the xsl file name<br />
	/// </pre>
	/// <p>'trace_level' legal values are 0..4</p>
	/// <pre>
	/// 0	OFF<br/>
	/// 1	ERROR<br/>
	/// 2	WARNING<br/>
	/// 3	INFO<br/>
	/// 4	VERBOSE<br/>
	/// </pre>
	/// <p>If this key is missing tracing is OFF. See the remarks on Itracer methods 
	/// for further explanations.</p>
	/// <p>If 'trace_style_type' and 'trace_style_file' are not set, tracing 
	/// file made will not contain an 'xml-stylesheet' processing instruction.</p>
	/// </remarks>
	/// <example>
	/// <code escaped='true'>
	/// <!-- 
	///		Example config file showing mandatory keys for the tracing mechanism.
	/// -->
	/// <configuration>
	/// <!-- there can be only ONE unnamed dbj_config node -->
	/// <dbj_config>
	/// <!-- This keys are MANDATORY for the dbj.fm tracer mechanism -->
	/// <add key="trace_level" value="4" />
	/// <add key="trace_style_type" value="text/xsl" />
	/// <add key="trace_style_file" value="../dbjtraceshow.xsl" />
	/// </dbj_config>
	/// <!-- the rest of file -->
	/// </configuration>
	/// </code></example>
	/// <example><code escaped="true">
	/// <!--
	///     Example of a trace file source. Assembly which used the tracing service
	///     is called 'dbj_ept'
	/// -->
	/// <?xml-stylesheet type='text/xsl' href='../dbjtraceshow.xsl' ?>
	/// <trace timestamp="20050225172432" assembly="dbj_ept, Version=2.0.1882.27464, Culture=neutral, PublicKeyToken=7385268609f9297c">
	/// <switch displayname="trace_level" description="dbj_ept Assembly trace switch" level="Verbose" />
	/// <line timestamp="20050225172432"><![CDATA[
	/// dbj_ept, Version=2.0.1882.27464, Culture=neutral, PublicKeyToken=7385268609f9297c
	/// Corelib Version : 5.0.0.0(RELEASE)
	/// ]]></line>
	/// <line timestamp="20050225172433"><![CDATA[Testing TzxMsg.wsep]]></line>
	/// <line timestamp="20050225172433"><![CDATA[ERROR]]></line>
	/// <line timestamp="20050225172433"><![CDATA[System.Runtime.InteropServices.COMException (0x800401F3): Invalid class string
	/// at System.Runtime.InteropServices.Marshal.MkParseDisplayName(UCOMIBindCtx pbc, String szUserName, UInt32& pchEaten, UCOMIMoniker& ppmk)
	/// at System.Runtime.InteropServices.Marshal.BindToMoniker(String monikerName)
	/// at dbj.fm.util.make_new_com_object(String progid)
	/// at dbjept.Behind.setup_the_tested_component_proxy(String the_prog_id)]]></line>
	/// <!-- Tracing closed at : 20050225172435 -->
	/// </trace>
	/// </code></example>
	public abstract class Itracer
	{

		/// <summary>
		/// if subclass property is set, the tracefile will be made
		/// in the sub-folder by the subclass name
		/// otherwise it will be in the folder which is named same
		/// as assembly which has made the tracer instance
		/// subclass name string can contain only digits and letters
		/// to prevent sub-sub classing ideas.
		/// </summary>
		public abstract string subclass { set; get; }

		/// <summary>
		/// leave tracing info if level is set to ERROR (1), or higher
		/// Also send the same message to the local evlog.
		/// </summary>
		/// <param name="x">exception instance</param>
		public abstract void error( Exception x );

		/// <summary>
		/// leave tracing info if level is set to ERROR (1), or higher
		/// </summary>
		/// <param name="fmt">fromat string and arguments, or just a string</param>
		/// <param name="args">optional list of arguments</param> 
		public abstract void error( string fmt, params object [] args ) ;

		/// <summary>
		/// leave tracing info if level is set to WARNING (2), or higher
		/// </summary>
		/// <param name="fmt">fromat string and arguments, or just a string</param>
		/// <param name="args">optional list of arguments</param> 
		public abstract void warning( string fmt, params object [] args ) ;

		/// <summary>
		/// leave tracing info if level is set to INFO (3), or higher
		/// </summary>
		/// <param name="fmt">fromat string and arguments, or just a string</param>
		/// <param name="args">optional list of arguments</param> 
		public abstract void info( string fmt, params object [] args ) ;
	
		/// <summary>
		/// leave tracing info if level is set to VERBOSE (4)
		/// </summary>
		/// <param name="fmt">fromat string and arguments, or just a string</param>
		/// <param name="args">optional list of arguments</param> 
		public abstract void verbose( string fmt, params object [] args ) ;

	}


	/// <summary>
	/// Event logging services
	/// </summary>
	/// <remarks>
	/// Logging can be done for different sources but all will be 
	/// under the same logfile as defined by DBJ_EVLOG_LOGNAME
	/// <h3>Security Note</h3>
	/// <p>This service may not be used from the code which is behind ASP.NET pages,
	/// or any other code running as a part of a solution which runs in the web server context.</p>
	/// <p>This is because .NET does not allow access to the registry from a code without special credentials. And .NET 
	/// implementation used by this service implementation requires registry access.</p>
	/// <p>Please refer to the .NET security to learn about the solutions.</p>
	/// <p>For ASP.NET applicatin one way is to ad the 'impersonation' line in the web.config file. Giving 
	/// the login name and password of the user who has enough credentials to access the registry.</p>
	/// </remarks>
	public abstract class Ievent_logger
	{
		/// <summary>
		/// Logging can be done for different sources but all will be 
		/// under the same logfile as defined by DBJ_EVLOG_LOGNAME
		/// </summary>
		public abstract string source_name { get ; }
		/// <summary>
		/// log error message
		/// </summary>
		 public abstract void error ( string message );

		/// <summary>
		/// log the exception
		/// </summary>
		 public abstract void error ( Exception x );

		/// <summary>
		/// log warning message
		/// </summary>
		 public abstract void warn ( string message );

		/// <summary>
		/// log info message
		/// </summary>
		 public abstract void info ( string message );
	}
	
	#endregion

	/// <summary>
	/// Facade of the library.
	/// </summary>
	/// <remarks>
	/// Through this class all the services of the library are delivered.
	/// Designad after a 'facade' design pattern.
	/// </remarks>
	public sealed class core
	{
		/// <summary>
		/// Used for locking. 
		/// </summary>
		private static object locker = new object() ;
		/// <summary>
		/// returns corelib source name in the event log
		/// </summary>
		public static string DBJ_EVLOG_CORELIB_SOURCENAME { get { return "DBJ_CORELIB"; }}
		/// <summary>
		/// returns event log name
		/// </summary>
		public static string DBJ_EVLOG_LOGNAME { get { return "DBJ_EVLOG"; }}

		/// <summary>
		/// Name of the XML node, used for dbj configuration files.
		/// <seealso cref="dbj.fm.Iconfiguration" />
		/// </summary>
		public static string dbj_config_node_name { get { return "dbj_config" ;} }


		private core()
		{
			// can not make instances of this class
		}

		/// <summary>
		/// Obtain the implementation of the service required.
		/// <seealso cref="dbj.fm.ServiceID"/>
		/// </summary>
		/// <param name="which">service id</param>
		/// <param name="args">optional list of argument that has to match service implementation requirements</param>
		/// <returns>service wrap-up that allows for implicit casting</returns>
		/// <remarks>
		/// This is the 'factory method' implementation. All the key services implementation issues are encapsulated 
		/// inside the library. This method also encapsulates HOW are they instances made. Are they singletons, are they 
		/// made on heap or on the stack, are they pooled, etc.
		/// Each services interface is publicly available as an abstract class.
		/// To make caller able to require exact service, first argument has to be a element from 
		/// the dbj.fm.ServiceID enum.
		/// Please read each services interface documentation to understand its behavior and parameters
		/// required in order to instantiate each of them.
		/// To have a single method like this, make and return possibly diverse types, is made possible by
		/// returning object of type fm.FactoryImplicitor. Which use is completely transparent thanks
		/// to its implicit conversion operators.
		/// While there is quite a lot of activity going on behind the scenes , one can see 
		/// in the example bellow, how simple the code using all of this really looks.
		/// </remarks>
		/// <example><code>
		/// 
		/// 		// Please remember what type is returned from make_service()
		/// 		// the casting to the fm.Iconfiguration is transparently executed
		///  		dbj.fm.Iconfiguration	cfg_ = dbj.fm.core.get_service( 
		///				dbj.fm.ServiceID.Configuration ,               // enum element 
		///				System.Reflection.Assembly.GetExecutingAssembly()   // reference to the calling assembly
		///				) ;
		///	
        ///
		/// </code></example>
		/// <threadsafety static="true" instance="false"/>
		public static fm.FactoryImplicitor make_service ( fm.ServiceID which, params object[] args )
		{
				lock( core.locker )
				{
					try 
					{
							return fm.service_factory.get_service(which,args);
					} 
					catch ( Exception x )
					{
						fm.evlog.internal_log.error(x);
						throw x ;
					}
				}
		}

		} // eof corelib class

	/// <summary>
	/// Collection of various utility methods.
	/// Instances of this class can not be made. 
	/// All public methods are static.
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
	public sealed class util 
	{

		/// <summary>
		/// Used for locking. 
		/// </summary>
		private static object locker = new object() ;

		private util () { /* this is stoping users making instances of this class */ }
		#region utilities

		/// <summary>
		/// Return the name of the configuration file, as defined by current AppDomain private data.
		/// </summary>
		public static string config_file { get { return (string)AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE"); } }

		/// <summary>
		/// Returns path of the base directory of the calling assembly, as defined by current AppDomain
		/// </summary>
		public static  string codebase { get { return AppDomain.CurrentDomain.BaseDirectory ; } }
		
		/// <summary>
		/// User domain name
		/// </summary>
		public static  string domain_name	() 
		{
			return System.Environment.UserDomainName ;
		}
		/// <summary>
		/// Machine name, on which this assembly is running.
		/// </summary>
		public static  string  computer_name	() 
		{
			return System.Environment.MachineName ;
		}

		/// <summary>
		/// Return just a user name, never return a domain\uname format.
		/// </summary>
		public static  string user_name() 
		{
			lock ( util.locker )
			{
				System.Security.Principal.WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent() ;
				int domain_delmiter_pos = wi.Name.IndexOf("\\") ;
				if ( domain_delmiter_pos > -1 ) // '<domain>\<uname>' format
				{
					return wi.Name.Substring( 1 + domain_delmiter_pos ) ;
				}
				else 
				{
					return wi.Name ;
				}
			}
		}

		/// <summary>
		/// Encapsulation of the ArayList.Adapter() usage. 
		/// </summary>
		/// <param name="source_">any kind of a array</param>
		/// <returns>object</returns>
		public	static object array2objarray ( Array source_ )
		{
			return System.Collections.ArrayList.Adapter( source_).ToArray() ;
		}

		/// <summary>
		/// Return true if two strings are identical. 
		/// Case sensitive if anythung as an third argument is supplied.
		/// </summary>
		/// <example>
		/// <code>
		/// 
		/// compare( "dbj", "DBJ", true ) ; // returns false
		/// compare( "dbj", "DBJ", 1 )    ; // returns false
		/// compare( "dbj", "DBJ", "")    ; // returns false
		/// compare( "dbj", "DBJ" )       ; // returns true
		/// 
		/// </code>
		/// </example>
		public static bool compare( string  s1, string s2, params bool [] sensitive )
		{
			lock ( util.locker ) 
			{
				System.Collections.IComparer comparer = null ;
				if ( sensitive.Length > 0 )
				{
					// case sensitive comparison 
					comparer = System.Collections.Comparer.Default ;
				} 
				else 
				{
					// NOT case sensitive comparison is default
					comparer = System.Collections.CaseInsensitiveComparer.Default ;
				}
				return 0 == comparer.Compare( s1, s2 ) ;
			}
		}

		/// <summary>
		/// Throw exception if argument resolves to false
		/// Non existent in release builds
		/// </summary>
		/// <param name="obj" >boolean value</param>
		public static void ASSERT ( bool obj )
		{
#if DEBUG				
			lock ( util.locker )
			{
#line 177 "DBJ CORELIB Assembly"
				if(obj == false)
					throw new Exception("ASSERT failure") ;
			}
#endif
		}
		//---------------------------------------------------------------------
		/// <summary>Return true if string is found in an array</summary>
		/// <param name="array">array on which search needs to be performed</param>
		/// <param name="val" >value to be searched in the array</param>
		public static bool stringValueExistsInArray(object [] array, object val)
		{
			lock ( util.locker )
			{
				int index = Array.BinarySearch(array,0,array.Length,val,System.Collections.CaseInsensitiveComparer.Default ) ;
				if ( index < 0 )
					return false ;
				else
					return true ;
			}
		}

		/// <summary>
		/// Return new array made from values of the existing array, only if they begin with given string.
		/// 
		/// Example :
		/// 
		/// { "merry","mother","daddy" } , "m"
		/// 
		/// gives
		/// 
		/// { "merry", "mother" }
		/// 
		/// </summary>
		/// <param name="arr" >array to search</param>
		/// <param name="begins_with" >start string to be searched</param>
		public static object [] make_if_begins_with ( object []  arr, string begins_with )
		{
			lock( util.locker )
			{
				System.Collections.ArrayList retval =  new System.Collections.ArrayList() ;
				foreach ( string to_compare in arr )
					if ( to_compare.StartsWith(begins_with) )
					{
						retval.Add(to_compare ) ;
					}
				return retval.ToArray() ;
			}
		}
			
		/// <summary>
		/// start the external process
		/// </summary>
		/// <param name="argsLine" >arguments</param>
		/// <param name="exeName" >name of the executable</param>
		/// <param name="timeoutSeconds" >time out in seconds</param>
		/// <returns>the output</returns>
		/// <remarks>If timeout argument is 0 caling process is not waiting for the new process.
		/// Otherwise wait for given number of seconds happens, and possible standard output result 
		/// is returned in a single string.</remarks>
		public static string Run(string exeName, string argsLine, int timeoutSeconds)
		{
			StreamReader	outputStream	= StreamReader.Null;
			string			output			= "";
			bool			success			= false; 
			Process			newProcess		= new Process();

			try
			{
				newProcess.StartInfo.FileName = exeName;
				newProcess.StartInfo.Arguments = argsLine;
				newProcess.StartInfo.UseShellExecute = false ;
				newProcess.StartInfo.CreateNoWindow = true;
				newProcess.StartInfo.RedirectStandardOutput = true;
				newProcess.Start(); 

				if (0 == timeoutSeconds)
				{
					// outputStream = newProcess.StandardOutput;
					output = "" ; // outputStream.ReadToEnd();
					// newProcess.WaitForExit();
				}
				else
				{
					success = newProcess.WaitForExit(timeoutSeconds * 1000);

					if (success)
					{
						outputStream = newProcess.StandardOutput;
						output = outputStream.ReadToEnd();
					}
					else
					{
						output = "Timed out at " + timeoutSeconds + " seconds waiting for " + exeName + " to exit.";
					}
				}
			}
			catch(Exception e)
			{
				throw e ;
			}
			finally
			{
				if ( outputStream != StreamReader.Null ) outputStream.Close();
				newProcess.Close() ;
			} 
			// Return the output to the calling method: 
			return "\t" + output; 
		}

		/// <summary>
		/// Shell execute the process with argLine 
		/// </summary>
		/// <param name="argsLine" >arguments to be passed</param>
		/// <param name="exeName" >name of the executable</param>
		/// <param name="timeoutSeconds" >time out in seconds</param>
		/// <returns >string</returns>
		public static string execute(string exeName, string argsLine, int timeoutSeconds)
		{
			string output = "";
			bool success = false; 

			try
			{
				Process newProcess = new Process();
				newProcess.StartInfo.FileName = exeName;
				newProcess.StartInfo.Arguments = argsLine;
				newProcess.StartInfo.UseShellExecute = true ;
				newProcess.StartInfo.CreateNoWindow = true;
				newProcess.StartInfo.RedirectStandardOutput = false ;
				newProcess.Start(); 

				if (0 == timeoutSeconds)
				{
					newProcess.WaitForExit();
				}
				else
				{
					success = newProcess.WaitForExit(timeoutSeconds * 1000);
					if (! success)
					{
						output = "Timed out at " + timeoutSeconds + " seconds waiting for " + exeName + " to exit.";
					}
				}
			}
			catch(Exception e)
			{
				output = e.ToString() ;

			}
			finally
			{
			} 
			// Return the output to the calling method: 
			return "\t" + output; 
		}

		// version of this assembly
		/// <summary>
		/// Finds the version of the executing assembly
		/// </summary>
		/// <returns>string representing version of the executing assembly</returns>
		public static string version ( )
		{
			return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()  + "(" + util.build_label() + ")";
		}

		/// <summary>
		/// Returns a string denoting wether debug or release version of the assembly is in use.
		/// </summary>
		/// <returns>"DEBUG" if debug build is used.</returns>
		/// <returns>"RELEASE" if release build is used.</returns>
		public static string build_label ( )
		{
#if DEBUG
			return "DEBUG" ;
#else
			return "RELEASE" ;
#endif
		}

		/// <summary>
		/// Get the value from the xml node, and cast it implicitly.
		/// </summary>
		/// <remarks>This in effect converts xml string values into the type of variable receiving the result
		/// of this method.
		/// <p>Through the magic of the Implicitor, which is the actual return type.</p></remarks>
		/// <example><code>
		/// // These are both legal calls
		/// // find a first node called 'wait' under the endpoint_setting
		/// // return it's string value and cast it into the 'int' type
		/// // this 'magic' can be done for any type that Implicitor supports.
		/// int wait =  dbj.fm.util.cast( endpoint_setting, "wait" ) ;
		/// string progid =  dbj.fm.util.cast( endpoint_setting, "progid" ) ;
		/// 
		/// // what actually happens is this
		/// fm.Implicitor temporary =  dbj.fm.util.cast( endpoint_setting, "wait" ) ;
		/// int wait = temporary ;
		/// </code>
		/// <code>
		/// // Fallback value may be optionaly given. 
		/// // Exception is NOT thrown in this case
		/// // UNLESS fallout type is different than the required result type!
		/// string progid =  dbj.fm.util.cast( endpoint_setting, "progid", "DEFAULT.PROGID" ) ;
		/// // Following line throws the exception from the Implicitor
		/// int wait =  dbj.fm.util.cast( endpoint_setting, "wait", "Has to be 'int'" ) ;
		/// </code>
		/// </example>
		/// <param name="xmlnode">the root node</param>
		/// <param name="name">tag name of the required child node</param>
		/// <param name="fallback">Optionaly a fallback value is given if node by given name is not found.</param>
		/// <returns>instance of the Implicitor</returns>
		public static fm.Implicitor cast( System.Xml.XmlNode xmlnode , string name, params object [] fallback )
		{
			lock ( util.locker )
			{
				try 
				{
					return new fm.Implicitor( xmlnode[name].InnerText )  ;
				} 
				catch ( System.Exception x ) {
					if ( fallback.Length < 1 )
						throw new Error("Casting the xml node by name :" + name + ", failed.", x) ;
					else
						return new fm.Implicitor( fallback[0].ToString() )  ;
				}
			}
		}

		
		/// <summary>
		/// Generate the timestamp by the ISO standard. Format used is: yyyyMMddHHmmss
		/// </summary>
		/// <returns>the timestamp string</returns>
		public static string timestamp ()
		{
			lock ( util.locker )
			{
				return DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmss");
			}
		}

		/// <summary>
		/// Create an arbitrary com object from progid given.
		/// <seealso cref="dbj.fm.util.make_new_com_object" />
		/// </summary>
		/// <param name="progid">the prog id</param>
		/// <returns>object, which has to be casted to one of the interfaces that is implemented.</returns>
		/// <remarks>
		/// Although this method returns System.Object, 
		/// the 'real' type returned is Remoting.__TransparentProxy , an internal .NET class.
		/// Although you can call methods of this interface through reflection, the best and
		/// safest usage pattern is to cast the result to the interface and then use it.
		/// This method obtains instances only from the local host.
		/// </remarks>
		/// <example>
		/// <code>
		/// // IDcoument may be imported from the IDL or TLB file.
		/// public Interface IDocument {
		///			string title () ;
		/// }
		/// // make new instance of the document COM(+) object
		/// // cast ti to interface 
		/// IDocument doc = (IDocument)dbj.fm.util.maken_new_com_object("MY.DOCUMENT");
		/// // use it
		/// string title = doc.title() ;
		/// </code>
		/// </example>
		static public object make_new_com_object(string progid)
		{
			lock ( util.locker )
			{
				// this returns Remoting.__TransparentProxy
				return System.Runtime.InteropServices.Marshal.BindToMoniker("new:" + progid);
			}
		}

		/// <summary>
		/// Make com(+) instance on the required machine.
		/// </summary>
		/// <param name="progid">prog id</param>
		/// <param name="hostname">hostname, use 'localhost' for local machine</param>
		/// <returns>reference to remote object proxy</returns>
		/// <remarks>
		/// COM setup required for this method to work is explained in the MSDN as
		/// related to the Type.GetTypeFromProgID() method.
		/// If this method can not create the instance you need from the remote machine, please be sure you have read about 
		/// the setup required.
		/// </remarks> 
		public static object make_new_com_object ( string progid, string hostname ) 
		{
			lock( util.locker )
			{
				Type the_type = Type.GetTypeFromProgID(progid,hostname,true);
				if ( ! the_type.IsCOMObject ) 
					throw new ApplicationException(progid + ", is not a COM object") ;
				return Activator.CreateInstance( the_type ) ;
			}
		}

		/// <summary>
		/// Check permission helper.Checks the single permision.
		/// </summary>
		/// <param name="perm">Required permision instance</param>
		/// <returns>True if demand for permission required is succeeded.</returns>
		/// <example>
		/// <code>
		/// void SaveHighScore(string name, int score) 
		///	{
		///		IPermission perm =	new FileDialogPermission(FileDialogPermissionAccess.Save);
		///		if( ! dbj.fm.util.have_permission(perm) ) 
		///		{
		///			MessageBox.Show("Doh!");
		///			return;
		///		}
		///}
		/// </code></example>
		public static bool have_permission( System.Security.IPermission perm ) 
		{
			lock ( util.locker )
			{
				try { perm.Demand(); }
				catch( System.Security.SecurityException ) { return false; }
				return true;
			}
		}

		/// <summary>
		/// Check permisions on the whole permision set at once.
		/// </summary>
		/// <param name="pset" >An PermissionSet instance</param>
		/// <returns>True if demand for permission required is succeeded.</returns>
		public static bool have_permission( System.Security.PermissionSet pset ) 
		{
			lock( util.locker )
			{
				try { pset.Demand(); }
				catch( System.Security.SecurityException ) { return false; }
				return true;
			}
		}

		#endregion
	}
}
