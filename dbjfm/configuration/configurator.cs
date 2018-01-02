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

namespace dbj.fm.config
{
	/// <summary>
	/// Summary description for configurator. 
	/// Implements Iconfiguration interface which contains methods for 
	/// accessing the callers configuration file
	/// </summary>
	internal class configurator : Iconfiguration 
	{
		fm.config.AssemblySettings asmcfg = null ;
		/// <summary>
		/// Constructor of configurator class. Makes as instace of 
		/// fm.config.AssemblySettings class.
		/// </summary>
		/// <param name="asm">assembly reference of the caller</param>
		public configurator(System.Reflection.Assembly asm)
		{
			this.asmcfg = new fm.config.AssemblySettings(asm) ;
		}
		//-----------------------------------------------------------------------
		~configurator()
		{
			 this.asmcfg = null ;
		}
		//-----------------------------------------------------------------------
		/// <summary>
		/// returns name of calling assembly configuration file.
		/// </summary>
		public override string file_name 
		{
			get {
				return asmcfg.cfg_file() ;
			} 
		}
		//-----------------------------------------------------------------------
		/// <summary>
		/// the name of the config node which is going to be processed by this service.
		/// Other nodes are ignored.
		/// </summary>
		public override string config_node_name ( ) { return fm.config.AssemblySettings.nodeName ; }
		//-----------------------------------------------------------------------
		/// <summary>
		/// Retrieves value for the specified key from the caller configuration file
		/// </summary>
		/// <param name="key">name of key present in configuration file of the caller</param>
		/// <returns>fm.Implicitor</returns>
		public override fm.Implicitor get_value(string key)
		{
			return this.asmcfg[key] ;
		}
		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns an xmlNodeList containing config nodes of nodename defined in 
		/// Assemblysettings class from the caller configuration file
		/// </summary>
		/// <returns>System.Xml.XmlNodeList</returns>
		public override System.Xml.XmlNodeList get_asxml()
		{
			return this.asmcfg.config_asxml() ;
		}
		//-----------------------------------------------------------------------
		/// <summary>
		/// get every config node where attrib name eq givne name as xmlnode regardles of its kind
		/// </summary>		
		/// <param name="name">attribute value in tranzax_settings node for which 
		/// xmlnodelist has to be retrived</param> 
		/// <returns>list of XML nodes</returns>
		public override System.Xml.XmlNodeList get_asxml( string name )
		{
			return this.asmcfg.config_asxml( name ) ;
		}

	
	}
}
