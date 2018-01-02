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

[assembly:System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.RequestMinimum, UnmanagedCode=true)]
[assembly:System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.RequestMinimum, Name = "FullTrust")]

namespace dbjept
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Configuration;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Xml ;

	internal sealed class Util 
	{
		static object locker = new object() ;

		public static void tooltip_to_control(System.Windows.Forms.Control control, string tooltip_text)
		{
			ToolTip toolTip1 = new ToolTip();
			// Set up the delays for the ToolTip.
			toolTip1.AutoPopDelay = 5000;
			toolTip1.InitialDelay = 1000;
			toolTip1.ReshowDelay = 500;
			// Force the ToolTip text to be displayed whether or not the form is active.
			toolTip1.ShowAlways = true;
			// Set up the ToolTip text for the Button and Checkbox.
			toolTip1.SetToolTip(control, tooltip_text);
		}
	}

	internal enum Build { D, R } ;
	/// <summary>
	/// Here we encapsulate what is 'behind' a Form.
	/// In onther words here is where the functionality is.
	/// </summary>
	internal sealed class Behind
	{
		//==================================================================
		static internal string build_letter 
		{
			get 
			{
#if DEBUG
				return Build.D.ToString() ;
#else
				return Build.R.ToString() ;
#endif
			}
		}
		
		Form1					  the_form_				  = null ;
		string					  tested_component_progid = string.Empty ;
		dbj.integration.IPoint	  tested_component_		  = null ;
		
		//========================================================================
		private static int  instance_id__			  = 0 ;
		public static int  instance { get { return ++ Behind.instance_id__  ; }}
		//========================================================================
		static dbj.fm.Itracer trace_ = null ;
		public static dbj.fm.Itracer trace 
		{
			get 
			{
				if ( Behind.trace_ == null )
					Behind.trace_ = dbj.fm.core.make_service( dbj.fm.ServiceID.Tracing ,
						System.Reflection.Assembly.GetExecutingAssembly()
						);
				return Behind.trace_ ;
			}
		}
		//========================================================================
		private Behind(){/* this ctor is forbidden */}

		//========================================================================
		/// <summary>
		/// the only way to construct this class
		/// </summary>
		/// <param name="parent">the form in the front</param>
		public Behind( Form1 parent ) 
		{
			the_form_         = parent ;
		}

		~Behind () {
						trace_ = null ;
						this.the_form_ = null ;
		}
		//========================================================================
		// how is new line of text added to the output text area
		// to the end or to the top of the text
		static public bool output_to_end () { return true; }
		//========================================================================
		delegate void write_method(string s);

		//========================================================================
		public void writeln( string s_ ) 
		{
			write_method form_write = new write_method(this.the_form_.writeln);
			this.the_form_.Invoke(form_write, new object [] {s_} );
		}
		// with format string
		public void writeln( string f_ , string s_ ) 
		{
			writeln( string.Format( f_ , s_ ) ) ;
		}
		//
		public void writeln( System.Exception x ) 
		{
			this.writeln( x.GetType().Name + " : " + x.Message ) ;
		}

		//========================================================================
		public void status( string s_ ) 
		{
			write_method form_write = new write_method(this.the_form_.status);
			this.the_form_.Invoke(form_write, new object [] {s_} );
		}

		//========================================================================
		static int was_here = 0 ;
		
		//========================================================================
		/// <summary>
		/// called when the front form is activated. From the forms event handler.
		/// </summary>
		public void on_form_activated(object sender, System.EventArgs e)
		{
			if ( was_here++ > 0 ) return ;
			//
			//this.test_cfg_handler();
			//
			this.tested_component_progid  = string.Empty ;
			//
			//----------------------------------------------------------------------------
			the_form_.Text = Application.ProductName + " " + Application.ProductVersion  + ":" + build_letter  ;
			status(this.tested_component_progid);	
			//----------------------------------------------------------------------------
			construct_tree_from_cfg( this.the_form_.treeView1 );
		}
		/// <summary>
		/// called when the front form is activated. From the forms event handler.
		/// to construct treeview.
		/// </summary>
		/// <param name="treeview1"></param>
		public void construct_tree_from_cfg(TreeView treeview1)
		{	
			try
			{				
				CFG cfg = CFG.singleton() ; 
				treeview1.Nodes.Clear() ;  // clear the TV on the form
				cfg.foreach_endpoint_setting_do( 
					new dbjept.CFG.endpoint_setting_user( 
							node_settings_user_for_treeview_making ) 
					) ;
			}
			catch(Exception ex)
			{
				writeln(ex.StackTrace.ToString());  
				writeln("----------------------------------------------------");
			}
		}		

		/// <summary>
		/// Transform each node into the Tree node.
		/// </summary>
		/// <example>
		/// One xml node to be transformed looks like this:
		/// <code escaped="true" >
		///		<endpoint>
		///			<measure>true</measure>
		///			<count>1</count>
		///			<wait>1</wait>
		///			<progid>TzxMsg.wsep</progid>
		///			<request><![CDATA[<ROOT><SERVICE_ID>1111</SERVICE_ID></ROOT>]]></request>
		///			<reply></reply>
		///		</endpoint>
		/// </code></example>
		/// <param name="endpoint_setting">the endpoint xml node from a config file</param>
		void node_settings_user_for_treeview_making (  System.Xml.XmlNode endpoint_setting )
		{
			System.Windows.Forms.TreeView tv = this.the_form_.treeView1  ;

			// anti-measure for jokers having empty nodes in the config files
			if ( endpoint_setting.InnerText == "" ) 
			{
				writeln("\n\rCFG ERROR: endpoint node empty.") ;
				return ;
			}
			try 
			{	
				CFG.EP ep = new CFG.EP( endpoint_setting ) ;
				// creating root node.			
				TreeNode trone=new TreeNode(ep.description);			
				tv.Nodes.Add(trone);   			
				// adding child nodes to the root node.
				trone.Tag = ep ;
				trone.Nodes.Add(new TreeNode("progid:" + ep.progid) );  
				// the request is not to be displayed because it can be long and ugly
				// trone.Nodes.Add(new TreeNode("request:" +  request) );  
				trone.Nodes.Add(new TreeNode("measure:" +  ep.measure.ToString() ));  
				trone.Nodes.Add(new TreeNode("count:" + ep.count.ToString() )) ;  
				trone.Nodes.Add(new TreeNode("delay:" + ep.wait.ToString() )) ;  
			} 
			catch ( System.Exception x ) {
						writeln( x.GetType().Name + " : " + x.Message ) ;
			}
		}

		//========================================================================
		void setup_the_tested_component_proxy( string the_prog_id )
		{
				object iunknownPTR     = null ;
				dbj.fm.Itypeinfo typeinfo = null ; 
			try 
			{
				typeinfo        = dbj.fm.core.make_service( dbj.fm.ServiceID.Reflection) ;   	
#if USE_COM_TO_MAKE_IXML_SERVICE
				iunknownPTR     = dbj.fm.util.make_new_com_object(this.tested_component_progid) ;
				this.tested_component_ = (tzxPlugInInterop.IXMLService)iunknownPTR;
#else
				Type tzx_xml_svc_type = Type.GetTypeFromProgID( the_prog_id ) ;
				iunknownPTR = Activator.CreateInstance( tzx_xml_svc_type ) ;
#endif				 
				this.tested_component_ = (dbj.integration.IPoint)iunknownPTR ;
				//status(describer.describe((MarshalByRefObject)iunknownPTR)) ;			
				writeln( typeinfo.describe( this.tested_component_  ,
						dbj.fm.Itypeinfo.WHAT.ALL 
					| dbj.fm.Itypeinfo.WHAT.ATTRIBUTES )
					);    
			} 
			catch ( InvalidCastException cx )
			{
				this.tested_component_ = null ;
				writeln("CASTING ERROR");
				writeln( cx.Message );
				writeln("----------------------------------------------------");
				if ( iunknownPTR != null )
				{
					writeln("Type report : " );
					writeln( typeinfo.describe( iunknownPTR ,
						dbj.fm.Itypeinfo.WHAT.ALL 
						| dbj.fm.Itypeinfo.WHAT.ATTRIBUTES )
						);    
					writeln("----------------------------------------------------");
				}
			}
			catch ( Exception x )
			{
				this.tested_component_ = null ;
				writeln("ERROR");
				writeln( x.ToString() );
				writeln("----------------------------------------------------");			
			}
			finally {
				iunknownPTR     = null ;
				typeinfo = null ; 
			}
		}

		//========================================================================
		string execute_the_ep ( string db_guid, string xml_request )
		{
			dbj.fm.Itypeinfo typeinfo = 
					dbj.fm.core.make_service( dbj.fm.ServiceID.Reflection) ;   	
			string xml_reply = string.Empty ;
			object retval = this.tested_component_.call( xml_request, out xml_reply );
			//
			if ( retval == null )
			{
				writeln("Tested component has returned null object.") ;
			} 
			else
			{
				writeln("Tested component has returned this object:") ;
				writeln( typeinfo.describe( retval  ,
					dbj.fm.Itypeinfo.WHAT.ALL | dbj.fm.Itypeinfo.WHAT.ATTRIBUTES )
					);    
			}
			//
			return xml_reply ;
		}

		//========================================================================
		void show_user_and_principal ()
		{
			//Get the current identity and put it into an identity object.
			System.Security.Principal.WindowsIdentity MyIdentity =
				System.Security.Principal.WindowsIdentity.GetCurrent();

			//Put the previous identity into a principal object.
			System.Security.Principal.WindowsPrincipal MyPrincipal = 
				new System.Security.Principal.WindowsPrincipal(MyIdentity);

			//Principal values.
			string Name = MyPrincipal.Identity.Name;
			string Type = MyPrincipal.Identity.AuthenticationType;
			string Auth = MyPrincipal.Identity.IsAuthenticated.ToString();

			//Identity values.
			string IdentName = MyIdentity.Name;
			string IdentType = MyIdentity.AuthenticationType;
			string IdentIsAuth = MyIdentity.IsAuthenticated.ToString();
			string ISAnon = MyIdentity.IsAnonymous.ToString();
			string IsG = MyIdentity.IsGuest.ToString();
			string IsSys = MyIdentity.IsSystem.ToString();
			string Token = MyIdentity.Token.ToString();

			//Print the values.
			this.writeln("SECURITY Values for the current thread:{0}", System.Environment.NewLine );
			this.writeln("Principal Name: {0}", Name);
			this.writeln("Principal Type: {0}", Type);
			this.writeln("Principal IsAuthenticated: {0}", Auth);
			this.writeln("") ;
			this.writeln("Identity Values for current thread:");
			this.writeln("Identity Name: {0}", IdentName);
			this.writeln("Identity Type: {0}", IdentType);
			this.writeln("Identity IsAuthenticated: {0}", IdentIsAuth);
			this.writeln("Identity IsAnonymous: {0}", ISAnon);
			this.writeln("Identity IsGuest: {0}", IsG);
			this.writeln("Identity IsSystem: {0}", IsSys);
			this.writeln("Identity Token: {0}", Token);
			this.writeln("") ;
		}
		//========================================================================
		void setup_the_correct_identity ()
		{
			show_user_and_principal() ;
#if PIGS
			System.Security.Principal.WindowsIdentity current =
			System.Security.Principal.WindowsIdentity.GetCurrent() ;

			string current_name  = current.Name  ;
			
			System.Security.Principal.WindowsImpersonationContext 
				previous =	current.Impersonate() ;
#endif
		}
		//========================================================================
		public void on_test_requested( string last_selected_progid_ ,System.Windows.Forms.TreeNode selectedNode )
		{
			try 
			{
				setup_the_correct_identity() ;

				this.status("");
				writeln("------------------------------------");				
				writeln("Testing " + last_selected_progid_ );				
				writeln("------------------------------------");				

				if ( this.tested_component_ != null ) this.release_the_complus_resources() ;
				this.tested_component_progid = last_selected_progid_ ;
				this.setup_the_tested_component_proxy( last_selected_progid_ ) ;
			} 
			catch (Exception x)
			{
				this.writeln( "\nERROR\n" + x.ToString() + "\n\n" ) ;
				return ;
			}
			
			if ( this.tested_component_ == null )
			{
				writeln("------------------------------------");				
				writeln( last_selected_progid_ + ", NOT instantiated.");				
				writeln("------------------------------------");				
				return ;
			}

			try 
			{
				if( selectedNode == null ) throw new Exception("setting by progid [" + last_selected_progid_ + "],not found?") ;
				CFG.EP endpoint_setting = (CFG.EP)selectedNode.Tag ;
				bool measure =  endpoint_setting.measure  ;
				int  max_of_calls =  endpoint_setting.count ;
				int  delay_seconds =  endpoint_setting.wait ;
				string progid =  endpoint_setting.progid ;
				string xml_request = endpoint_setting.request ;

				// CFG tzxcfg = new CFG() ;
				int	num_of_calls = 0 ;
				// Char[]	splitval		=	new Char [] {':'}  ;
				// int		delay_seconds		= int.Parse(selectedNode.Nodes[3].Text.Split(splitval).GetValue(1).ToString() )     ;
						if ( delay_seconds < 1 ) delay_seconds = 0 ;
				// int		max_of_calls		= int.Parse(selectedNode.Nodes[2].Text.Split(splitval).GetValue(1).ToString() );
				// string	xml_request			= selectedNode.Nodes[0].Text.Replace("request:","").ToString().Trim(); //   .Split(splitval).GetValue(1).ToString()  ;
				string	dbguid				= System.Guid.NewGuid().ToString() ; // will come from CONFIG!
				string	retval_				= string.Empty ;		
				DateTime total_begin		= DateTime.Now ;
				DateTime call_begin			= DateTime.MinValue ;
				DateTime call_end			= DateTime.MinValue ;

				// creating instance of ResultFile
				ResultFile result_file = new ResultFile( 
					last_selected_progid_ ,
					xml_request,
					max_of_calls.ToString() ,
					delay_seconds.ToString() ,
					total_begin
					); 
			
				while(num_of_calls++ < max_of_calls)
				{
					retval_ = string.Empty ;
					call_begin = DateTime.Now ;
					try 
					{								
						retval_ = this.execute_the_ep(dbguid,xml_request) ;
						writeln( string.Format( "{0}------------------------------------{1}Call :{2}",
							Environment.NewLine , Environment.NewLine, num_of_calls )
							);				
						writeln("OK");				
					} 
					catch ( System.Exception x )
					{
						retval_ = "*ERROR*\n\n" + x.ToString() ;
					}
					call_end = DateTime.Now ;
					result_file.add_call_element(num_of_calls.ToString(), retval_ , format_time( call_end.Ticks - call_begin.Ticks, "HH:mm:ss:ff") ) ;

					// wait required number of seconds 
					if ( delay_seconds > 0 )
						System.Threading.Thread.Sleep( delay_seconds * 1000 ) ;
					else
						// allow other threads to do their job
						System.Threading.Thread.Sleep(1) ;
				}


				DateTime total_end = DateTime.Now ;
				result_file.close_test( format_time(total_end.Ticks - total_begin.Ticks, "HH:mm:ss:ff"),total_begin ) ;

				if ( CFG.singleton().show_every_result() )
						open_xml_file( result_file.file_path ) ;
				/*
				 System.Threading.TimerCallback tc = new System.Threading.TimerCallback( open_xml_file ) ;
				System.Threading.Timer timer = new System.Threading.Timer(
					new System.Threading.TimerCallback( open_xml_file ) ,
					result_file.file_path ,
					0,
					-1 
					) ;
				*/
				// status( describer.describe((MarshalByRefObject)this.tested_component_)) ;
				this.release_the_complus_resources() ;
				writeln("------------------------------------");
				writeln("Executed : " + max_of_calls + (max_of_calls == 1 ? ", call" : ", calls") ) ;
				writeln("------------------------------------");
				
				// tzxcfg = null ;
				num_of_calls = 0 ;
			}
			catch (Exception x) 
			{
				this.writeln( x ) ;
				x = null ;
			}
		}

		//---------------------------------------------------------------------------------
		void release_the_complus_resources() 
		{
			try 
			{
				if ( System.Runtime.InteropServices.Marshal.IsComObject( this.tested_component_ ) )
				System.Runtime.InteropServices.Marshal.ReleaseComObject( this.tested_component_ ) ;
				this.tested_component_ = null ;
			} 
			catch ( Exception x ) 
			{
				this.writeln( x ) ;
			}
			finally {
				GC.Collect() ; // for a good measure
			}
		}
		
		/// <summary>
		/// format a timespan given
		/// </summary>
		/// <param name="format">format string</param>
		/// <returns>formated time string</returns>
		static public string format_time ( long ticks, string format )
		{
			DateTime dt = new DateTime( ticks ) ;
			return dt.ToString( format ) ;
		}
		
		//========================================================================
		public void restart(object sender, System.EventArgs e)
		{
		//	construct_tree_from_cfg( this.the_form_.treeView1 );
			this.the_form_.output_dblclick(sender,e);
			was_here = 0;
			this.on_form_activated(sender,e);	
		}

		//========================================================================
		public void copy_output(object sender, System.EventArgs e)
		{
			this.the_form_.output_copy();
		}

		//========================================================================
		public void open_xml_file(object state)
		{
			try 
			{
				string path = (string)state ;
				// dbj.fm.corelib.Run( "IExplore.exe", path, 0 ); 
				// System.Diagnostics.Process.Start("IExplore.exe",path); 
				if ( path != string.Empty )
				{
					string  editor = CFG.singleton().result_file_editor() ;
					dbj.fm.util.Run( editor , path , 0 ) ; 
				}
			} 
			catch ( Exception x ) 
			{
				this.writeln("\n--------------------------------------------------------------------");
				this.writeln ("\nERROR:\n");
				this.writeln( x.ToString() ) ;		
			}
		}

		//========================================================================
		public bool open_config_file () {
			System.Windows.Forms.DialogResult user_decision =
			System.Windows.Forms.MessageBox.Show( 
				this.the_form_ ,
				"Open the configuration file for editing,\n\r" + 
				"and close the application in the same time?", 
				this.the_form_.Text , System.Windows.Forms.MessageBoxButtons.YesNo ) ;

			if ( user_decision != System.Windows.Forms.DialogResult.Yes ) 
					return  false ;
				dbj.fm.util.Run( "notepad.exe" , dbj.fm.util.config_file , 0 ) ; 
			return true ;
		}
		//========================================================================
		public void open_result_file () {
			string fname = string.Empty ;
			OpenFileDialog ofd = new OpenFileDialog();

			ofd.InitialDirectory = ResultFile.folder_path() ;
			ofd.Filter = "txt files (*.xml)|*.xml|All files (*.*)|*.*" ;
			ofd.FilterIndex = 1 ;
			ofd.CheckPathExists = true ;
			ofd.RestoreDirectory = false ;

			if( ofd.ShowDialog() == DialogResult.OK )
			{
				if(( fname = ofd.FileName )!= string.Empty )
				{
					string  editor = CFG.singleton().result_file_editor() ;
					dbj.fm.util.Run( editor , fname , 0 ) ; 
				}
			}
		}
	}
}
