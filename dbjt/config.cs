#region Copyright
//
//        DBJ*EPT(tm) The End Point Tester
//
//        Copyright (c)  2005-2006 by DBJ*Solutions Ltd. All Rights Reserved
//
//        THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF DBJ*Solutions Ltd..
//
//        The copyright notice above does not evidence any
//        actual or intended publication of such source code.
//
//  $Author: dusan $
//  $Date: 30.06.06 14:20 $
//  $Revision: 1 $
//
#endregion

namespace dbjept
{
	using System;

	internal sealed class CFG 
	{
		#region single EP description from a config file
		internal sealed class EP
		{
			System.Xml.XmlNode xn_ = null ;
			public EP ( System.Xml.XmlNode endpoint_setting ) { xn_ = endpoint_setting ; }

			public string description { get { return dbj.fm.util.cast( xn_, "description" , "CFG ERROR: description not found" ) ;}  }
			public string progid { get { return dbj.fm.util.cast( xn_, "progid" , "CFG ERROR: progid not found" ) ;}  }
			public string request { get { return dbj.fm.util.cast( xn_, "request" , "CFG ERROR: request not found" ) ;}  }
			public string reply { get { return dbj.fm.util.cast( xn_, "reply" , "CFG ERROR: reply not found" ) ;}  }
			public bool measure { get { return dbj.fm.util.cast( xn_, "measure" , "CFG ERROR: measure not found" ) ;}  }
			public int wait { get { return dbj.fm.util.cast( xn_, "wait" , "CFG ERROR: wait not found" ) ;}  }
			public int count { get { return dbj.fm.util.cast( xn_, "count" , "CFG ERROR: count not found" ) ;}  }
		} 
		#endregion
		/// <summary>
		/// Names required to be in the config file
		/// </summary>
		public enum NAME { xsltype, xslfile, result_file_editor, show_every_result }

		public static string name ( CFG.NAME enum_name ) { return enum_name.ToString(); }

		private dbj.fm.Iconfiguration coreconf_ = null ;
		public dbj.fm.Iconfiguration coreconf { get { return this.coreconf_ ; } }

		#region  singleton implementation

		private CFG ()
		{
			this.coreconf_ = dbj.fm.core.make_service ( 
				dbj.fm.ServiceID.Configuration , System.Reflection.Assembly.GetCallingAssembly() 
				);
		}

		private static object padlock = new object() ;
		private static CFG singleton_ = null ;
		public  static CFG singleton ()
		{
			lock ( padlock ){
				if ( CFG.singleton_ == null ) CFG.singleton_ = new CFG() ;
			}
			return CFG.singleton_ ;
		}

		~CFG ()
		{
			this.coreconf_ = null ;
		}

		#endregion

		#region application settings handling

		public string result_file_editor () {
			return this.coreconf.get_value( CFG.NAME.result_file_editor.ToString() ) ;
		}

		public bool show_every_result () {
			try 
			{
				return
					this.coreconf.get_value( CFG.NAME.show_every_result.ToString()) ;
			} 
			catch ( System.Exception )
			{
				return false ;
			}
		}

		#endregion

		#region endpoint configuration handling

		/// <summary>
		/// this delegate is used to denote method pointers that 
		/// may be called from foreach_endpoint_setting_do
		/// </summary>
		public delegate void endpoint_setting_user ( System.Xml.XmlNode endpoint_setting ) ;
		/// <summary>
		/// call the delegate for each available endpoint settings 
		/// </summary>
		public void foreach_endpoint_setting_do(endpoint_setting_user eps_user )
		{
		// get only the ones where name='endpoints'
		System.Xml.XmlNodeList confsections = coreconf.get_asxml("endpoints");

		// now use endpoint settings
			foreach( System.Xml.XmlNode node in confsections )
			{
				System.Xml.XmlNodeList endpoints = node.SelectNodes("./endpoint") ;

				foreach( System.Xml.XmlNode endpoint_setting in endpoints )
				{
						eps_user( endpoint_setting ) ;
				}
			}

	} // eof fun

		public System.Xml.XmlNode find_setting_by_progid ( string progid_to_found )
		{
			// get only the ones where name='endpoints'
			System.Xml.XmlNodeList confsections = coreconf.get_asxml("endpoints");

			// now use endpoint settings
			foreach( System.Xml.XmlNode node in confsections )
			{
				System.Xml.XmlNodeList endpoints = node.SelectNodes("./endpoint") ;

				foreach( System.Xml.XmlNode endpoint_setting in endpoints )
				{
					string  progid = dbj.fm.util.cast( endpoint_setting, "progid") ;

					if ( progid_to_found.ToLower() == progid.ToLower() ) return endpoint_setting ;
				}
			}
					return null ; // not found
		}
#endregion

	} // eof class
}
