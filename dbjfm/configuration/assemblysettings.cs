#region Copyright � 2003-2005 DBJ*Solutions Ltd. All Rights Reserved
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
//
// Idea : Mike Woodring
// http://staff.develop.com/woodring
//
namespace dbj.fm.config 
{
	using System;
	using System.Reflection;
	using System.Collections;
	using System.Xml;
	using System.Configuration;

	//	AssemblySettings usage:
	//	
	//	If you know the keys you're after, the following is probably
	//	the most convenient:
	//	
	//	AssemblySettings settings = new AssemblySettings();
	//	string someSetting1 = settings["someKey1"];
	//	string someSetting2 = settings["someKey2"];
	//	
	//	If you want to enumerate over the settings (or just as an
	//	alternative approach), you can do this too:
	//	
	//	IDictionary settings = AssemblySettings.GetConfig();
	//	
	//	foreach( DictionaryEntry entry in settings )
	//{
	//	// Use entry.Key or entry.Value as desired...
	//}
	//	
	//	In either of the above two scenarios, the calling assembly
	//	(the one that called the constructor or GetConfig) is used
	//	to determine what file to parse and what the name of the
	//	settings collection element is.  For example, if the calling
	//	assembly is c:\foo\bar\TestLib.dll, then the configuration file
	//	that's parsed is c:\foo\bar\TestLib.dll.config, and the
	
	//	configuration section that's parsed must be named <assemblySettings>.
	//	
	//	To retrieve the configuration information for an arbitrary assembly,
	//	use the overloaded constructor or GetConfig method that takes an
	//	Assembly reference as input.
	//	
	//	If your assembly is being automatically downloaded from a web
	//	site by an "href-exe" (an application that's run directly from a link
	//	on a web page), then the enclosed web.config shows the mechanism
	//	for allowing the AssemblySettings library to download the
	//	configuration files you're using for your assemblies (while not
	//	allowing web.config itself to be downloaded).
	//	
	//	If the assembly you are trying to use this with is installed in, and loaded
	//	from, the GAC then you'll need to place the config file in the GAC directory where
	//	the assembly is installed.  On the first release of the CLR, this directory is
	//	<windir>\assembly\gac\libName\verNum__pubKeyToken]]>.  For example,
	//	the assembly "SomeLib, Version=1.2.3.4, Culture=neutral, PublicKeyToken=abcd1234"
	//	would be installed to the c:\winnt\assembly\gac\SomeLib\1.2.3.4__abcd1234 diretory
	//	(assuming the OS is installed in c:\winnt).  For future versions of the CLR, this
	//	directory scheme may change, so you'll need to check the <code>CodeBase</code> property
	//	of a GAC-loaded assembly in the debugger to determine the correct directory location.

	/// <summary>
	/// Defines methods and properties for accessing the configuration file of caller		
	/// </summary>

	internal class AssemblySettings
	{
		private IDictionary settings;

		private Assembly assembly_ = null ;
		/// <summary>
		/// returns nodename as tranzax_settings
		/// </summary>
		public static string nodeName { get { return core.dbj_config_node_name ; } }
		//-----------------------------------------------------------------------------
		private AssemblySettings()	{ /* forbidden */ }
		/// <summary>
		/// Constructor of AssemblySettings class 
		/// Makes a call to GetConfig(assembly) method and initializes settings (IDictionary)
		/// </summary>
		/// <param name="asm"></param>
		public AssemblySettings( Assembly asm )
		{
			this.assembly_ = asm ;
			settings = GetConfig(asm);
		}
		//-----------------------------------------------------------------------------
		~AssemblySettings() {
			if ( this.settings != null ) this.settings.Clear() ;
			this.settings = null ;
			this.assembly_ = null ;
		}
		//-----------------------------------------------------------------------------
		/// <summary>
		/// indexer returning instance of an implicitor made by value found
		/// </summary>
		public Implicitor this[ string key ]
		{
			get
			{
				object settingValue = null;

				if( settings != null )
				{
					settingValue = settings[key];
				}

				// settingValue = (settingValue == null ? "" : settingValue) ;

				return new Implicitor(settingValue);
			}
		}
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Retursn IDictionary containing the values of calling assembly config file.
		/// </summary>
		/// <returns>IDictionary</returns>
		public static IDictionary GetConfig()
		{
			return GetConfig(Assembly.GetCallingAssembly());
		}
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Open and parse configuration file for specified
		/// assembly, returning dictionary collection to caller for future
		/// use outside of this class.
		/// </summary>
		/// <param name="asm">the assembly</param>
		/// <returns>IDictinary representing a section found by nodeName as defined in here</returns>
		/// <exception cref="System.Exception">On error throws System.Exception</exception>
		public static IDictionary GetConfig( Assembly asm )
		{
			try 
			{
				Uri file_uri = new Uri( assembly_cfg_file(asm)) ;
				string cfgFile = file_uri.LocalPath ;

				XmlDocument doc = new XmlDocument();
				System.IO.TextReader txt_reader = System.IO.TextReader.Synchronized( new System.IO.StreamReader(cfgFile)) ;
				XmlTextReader  xtr = new XmlTextReader( cfgFile, txt_reader ) ;
				xtr.WhitespaceHandling = WhitespaceHandling.None ;

				doc.Load(xtr);
            
				// collect all the nodes whose tag name is AssemblySettings.nodeName
				XmlNodeList nodes = doc.GetElementsByTagName(AssemblySettings.nodeName);

				foreach( XmlNode node in nodes )
				{
					// try to find <add key='' value='' />  nodes
					XmlNodeList add_nodes = node.SelectNodes( "add" ) ;
					// if these nodes are not in this section don't bother to make a dictionary
					if( add_nodes.Count > 1 )
					{
						DictionarySectionHandler handler = new DictionarySectionHandler();
						return (IDictionary)handler.Create(null, null, node);
					}
				}
			} 
			catch ( Exception x )
			{
				fm.evlog.internal_log.error(x);
				throw x ;
			}

			return null ;
		}
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Makes call to AssemblySettings.GetXMLConfig(assembly reference) by passing
		///  reference of calling assembly 
		/// </summary>
		/// <returns>
		/// System.Xml.XmlNodeList representing a section found by a nodename 
		/// defined in this class
		/// </returns>
		public System.Xml.XmlNodeList config_asxml( ) 
		{
			return AssemblySettings.GetXMLConfig( this.assembly_ ) ;
		}
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Makes call to AssemblySettings.GetXMLConfig(assembly,sectionname) passing
		/// reference of calling assembly and section name 
		/// </summary>
		/// <param name="name">section name in the config file</param>
		/// <returns>System.Xml.XmlNodeList</returns>
		public System.Xml.XmlNodeList config_asxml( string name ) 
		{
			return AssemblySettings.GetXMLConfig( this.assembly_, new string [] { name } ) ;
		}
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Open configuration file for specified assembly, returning collection of xml nodes to caller for future use outside of this class.
		/// This allows users to have sections which are pure xml
		/// <code>
		/// <assemblySeetings>
		/// <root>
		/// <data>
		/// <f1>F1</f1>
		/// <f2>F2</f2>
		/// </data>
		/// </root>
		/// </assemblySeetings>
		/// </code>
		/// </summary>
		/// <param name="asm">the assembly for which toread the settings</param>
		/// <param name="names">optional arg ifwe are looking for the settings node by exact value of hte attribute 'name'</param>
		/// <returns>XmlNodeList representing all sections found by nodeName as defined in here</returns>
		static public XmlNodeList GetXMLConfig( Assembly asm, params string [] names )
		{
			XmlNodeList nodes = null ;
			try
			{
				string cfgFile = assembly_cfg_file(asm) ;

				XmlDocument doc = new XmlDocument();
				doc.Load(new XmlTextReader(cfgFile));
            
				if ( names.Length < 1 )
					nodes = doc.GetElementsByTagName(AssemblySettings.nodeName);
				else
				{
					string xpath = string.Format("/configuration/{0}[@name='{1}']", AssemblySettings.nodeName, names[0] ) ;
					nodes = doc.SelectNodes( xpath ) ;
				}

				return nodes ;
			}
			catch
			{
			}

			return null ;
		}
		//-----------------------------------------------------------------------------
	
		/// <summary>
		/// the name of the assembly configuration file
		/// </summary>		
		/// <returns>file name</returns>
		public string cfg_file ( )
		{
			return this.assembly_.CodeBase + ".config";
		}
		//-----------------------------------------------------------------------------

		/// <summary>
		/// the name of the assembly configuration file
		/// </summary>
		/// <param name="asm">the assembly</param>
		/// <returns>file name</returns>
		public static string assembly_cfg_file ( Assembly asm )
		{
			return asm.CodeBase + ".config";
		}
	}

} // eof namespace dbj.fm 
