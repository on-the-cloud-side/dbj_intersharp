//*****************************************************************************/
//
//        DBJ*EPT(tm) The End Point Tester
//
//        Copyright (c)  2005 by DBJ*Solutions Ltd. All Rights Reserved
//
//        The copyright notice above does not evidence any
//        actual or intended publication of such source code.
//
//  $Author: dusan $
//  $Date: 30.06.06 15:20 $
//  $Revision: 2 $
//*****************************************************************************/
namespace dbjept
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.Configuration;
	using System.ComponentModel;
	using System.Windows.Forms;

	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	internal class Form1 : System.Windows.Forms.Form
	{
		//==================================================================
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			try 
			{
				AppDomain.CurrentDomain.UnhandledException +=
					new UnhandledExceptionEventHandler(unhandledExceptionEventHandler) ;

				AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles = "true" ;

				Application.EnableVisualStyles() ;
				Application.Run(new Form1());
			}
			catch ( System.Exception x )
			{
				System.Windows.Forms.MessageBox.Show( "*ERROR*\n\n" + x.ToString(),"UNHANDLED ERROR " + Form1.title ) ;
			}
			finally {
				AppDomain.CurrentDomain.UnhandledException -=
					new UnhandledExceptionEventHandler(unhandledExceptionEventHandler) ;
			}
		}

		//==================================================================
		static void unhandledExceptionEventHandler( object sender,UnhandledExceptionEventArgs e	)
		{
			Exception x = (Exception) e.ExceptionObject;			
			System.Windows.Forms.MessageBox.Show( "*ERROR*\n\n" + x.ToString(),"UNHANDLED ERROR " + Form1.title ) ;
			x = null ;
			sender = null ;
			e = null ;
		}
		//==================================================================
		static public readonly string NL	= Environment.NewLine ;
		static public readonly string title = Application.ProductName ;

		dbjept.Behind						behind_ = null ;
		private System.ComponentModel.Container	components = null;
		private System.Windows.Forms.TextBox	output;
		private System.Windows.Forms.StatusBar	statusBar;
		private System.Windows.Forms.ToolBar	toolBar;
		public  System.Windows.Forms.TreeView	treeView1;

		private System.Windows.Forms.Panel		panel1;
		private System.Windows.Forms.Panel		panel2;
		private System.Windows.Forms.Panel		panel3;
		private System.Windows.Forms.Panel		panel4;

		//private System.Windows.Forms.Splitter	splitter1;
		private System.Windows.Forms.Splitter	splitter2;
		//private System.Windows.Forms.Splitter	splitter3;
		//private System.Windows.Forms.Splitter	splitter4;

		//
		// This CAN NOT BE USED with our config file format ...
		// internal System.Configuration.AppSettingsReader configurationAppSettings = null ; 

		//==================================================================
		public Form1()
		{
			//
			// This CAN NOT BE USED with our config file format ...
			// configurationAppSettings = new System.Configuration.AppSettingsReader() ; 
			//
			InitializeComponent();
			this.output.Text = "" ;
		}

		//==================================================================
		private void on_loaded(object sender, System.EventArgs e)
		{
			/**/
			var /*Rectangle*/ sr	= Screen.GetWorkingArea(this) ;
			this.Width		= Convert.ToInt32( sr.Width * 0.8 ) ;
			this.Height		= Convert.ToInt32( sr.Height * 0.8 ) ;
            int left = 1; // (sr.Width/2) - (this.Width/2);
            int top = 1; // (sr.Height/2) - (this.Height/2);
			// Center the Form on the user's screen
			this.SetBounds(
				left ,top, this.Width, this.Height, BoundsSpecified.All 
				);  
			this.Location = new Point(left,top) ; 
			/**/
		
			// Add buttons to the tool bar
			ToolBarButton button1 = new ToolBarButton();
			ToolBarButton button2 = new ToolBarButton();
			ToolBarButton button3 = new ToolBarButton();
			ToolBarButton button4 = new ToolBarButton();
			ToolBarButton button5 = new ToolBarButton();
			ToolBarButton button6 = new ToolBarButton();

			ToolBarButton separator = new ToolBarButton();
 
			// Set the Text properties of the button controls.
			button1.Text = "&CAll";
			button2.Text = "Con&figure";
			button3.Text = "&Copy Output";
			button4.Text = "&Results";
			button5.Text = "&Exit";
			button6.Text = "&About" ;

			separator.Style = ToolBarButtonStyle.Separator ;

			// Add the button controls to the ToolBar.
			toolBar.Buttons.Add(separator);
			toolBar.Buttons.Add(button1);   // 1
			toolBar.Buttons.Add(separator);
			toolBar.Buttons.Add(button2);   // 3 == Re-Configuration
			toolBar.Buttons.Add(separator);
			toolBar.Buttons.Add(button3);   // 5
			toolBar.Buttons.Add(separator);
			toolBar.Buttons.Add(button4);   // 7 == Result browser
			toolBar.Buttons.Add(separator);
			toolBar.Buttons.Add(button5);   // 9 == Exit
			toolBar.Buttons.Add(separator);
			toolBar.Buttons.Add(button6);   // 11 == About
			
			this.behind_  = new dbjept.Behind(this) ;
			this.behind_.on_form_activated( sender,e) ;

			Util.tooltip_to_control( this.output , "Double-click to erase this text") ;
			Util.tooltip_to_control( this.treeView1 , "Test configuration") ;
			Util.tooltip_to_control( this.toolBar , "(c) 2003-2006 by DBJSOLUTIONS.COM\n\rAll rights reserved" ) ;
            Util.tooltip_to_control(this.toolBar, "(c) 2018 by DBJ.Systems \n\rAll rights reserved");

            // initial info for the output
            this.writeln( 
				Environment.NewLine  + 
				System.Reflection.Assembly.GetExecutingAssembly().FullName  +
				Environment.NewLine  + 
				"Corelib Version : " + 
				dbj.fm.util.version() + Environment.NewLine + 
                ".NET environment : " +
                System.Environment.Version.ToString(4)                 
                ) ;
#if DEBUG
			string debug_info = string.Format(
				"AppDomain friendly name : [{0}]"	+ NL + 
				"Base directory : [{2}]"			+ NL + 
				"Files shadow copied : [{4}]"		+ NL +
				"Relative search path : [{1}]"		+ NL +
				"Dynamic directory : [{3}]"			+ NL ,
				AppDomain.CurrentDomain.FriendlyName ,
				AppDomain.CurrentDomain.RelativeSearchPath ,
				AppDomain.CurrentDomain.BaseDirectory,
				AppDomain.CurrentDomain.DynamicDirectory ,
				( AppDomain.CurrentDomain.ShadowCopyFiles ? "YES" : "NO" ) ) ;
			// System.Diagnostics.Debug.WriteLine("") ;
			// System.Diagnostics.Debug.WriteLine( debug_info ) ;
				Behind.trace.info( "Loaded and initialized the main form" ) ;
				this.writeln( debug_info ) ;
#endif
		}

		//==================================================================
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		//==================================================================
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.output = new System.Windows.Forms.TextBox();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.panel3 = new System.Windows.Forms.Panel();
			this.toolBar = new System.Windows.Forms.ToolBar();
			this.panel2 = new System.Windows.Forms.Panel();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.panel3.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel4.SuspendLayout();
			this.SuspendLayout();
			// 
			// output
			// 
			this.output.AcceptsReturn = true;
			this.output.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.output.CausesValidation = false;
			this.output.Cursor = System.Windows.Forms.Cursors.Default;
			this.output.Dock = System.Windows.Forms.DockStyle.Fill;
			this.output.Font = new System.Drawing.Font("Verdana", 9F);
			this.output.Location = new System.Drawing.Point(0, 0);
			this.output.Multiline = true;
			this.output.Name = "output";
			this.output.ReadOnly = true;
			this.output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.output.Size = new System.Drawing.Size(783, 513);
			this.output.TabIndex = 1;
			this.output.TabStop = false;
			this.output.Text = "...";
			this.output.DoubleClick += new System.EventHandler(this.output_dblclick);
			// 
			// statusBar
			// 
			this.statusBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.statusBar.Location = new System.Drawing.Point(0, 20);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(1031, 29);
			this.statusBar.TabIndex = 5;
			this.statusBar.Text = "statusBar";
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.statusBar);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel3.Location = new System.Drawing.Point(11, 598);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(1031, 49);
			this.panel3.TabIndex = 11;
			// 
			// toolBar
			// 
			// this.toolBar.ButtonSize = new System.Drawing.Size(39, 36);
			this.toolBar.DropDownArrows = true;
			this.toolBar.Font = new System.Drawing.Font("Verdana", 10.2F);
			this.toolBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.toolBar.Location = new System.Drawing.Point(0, 0);
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(1031, 42);
			this.toolBar.TabIndex = 4;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.toolBar);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Font = new System.Drawing.Font("Verdana", 10.2F);
			this.panel2.Location = new System.Drawing.Point(11, 11);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(1031, 74);
			this.panel2.TabIndex = 10;
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.Font = new System.Drawing.Font("Verdana", 10.2F);
			this.treeView1.ImageIndex = -1;
			this.treeView1.Indent = 21;
			this.treeView1.ItemHeight = 20;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectedImageIndex = -1;
			this.treeView1.Size = new System.Drawing.Size(248, 513);
			this.treeView1.TabIndex = 0;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.treeView1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(11, 85);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(248, 513);
			this.panel1.TabIndex = 13;
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.output);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel4.Location = new System.Drawing.Point(259, 85);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(783, 513);
			this.panel4.TabIndex = 14;
			// 
			// splitter2
			// 
			this.splitter2.CausesValidation = false;
			this.splitter2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.splitter2.Location = new System.Drawing.Point(259, 85);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(1, 513);
			this.splitter2.TabIndex = 15;
			this.splitter2.TabStop = false;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(9, 21);
			this.CausesValidation = false;
			this.ClientSize = new System.Drawing.Size(1053, 658);
			this.Controls.Add(this.splitter2);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.DockPadding.All = 11;
			this.Font = new System.Drawing.Font("Verdana", 10.2F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.on_loaded);
			this.panel3.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		
		//========================================================================
		internal void writeln ( string lineoftext )
		{
			if ( Behind.output_to_end() )
				this.output.Text = this.output.Text + lineoftext + Environment.NewLine ;
			else
				this.output.Text = lineoftext + Environment.NewLine + this.output.Text ;

			Behind.trace.info( lineoftext ) ;
		}
		//========================================================================
		internal void status ( string lineoftext )
		{
			this.statusBar.Text = lineoftext ;			
		}
		
		//========================================================================
		internal void output_clear (  )
		{
			this.output.Text = Environment.NewLine ;			
		}

		//========================================================================
		internal void output_copy()
		{
			Clipboard.SetDataObject(this.output.Text);
		}

		//========================================================================
		internal void output_dblclick(object sender, System.EventArgs e)
		{
			this.output.Text = "" ;
			this.status("");
		}

		//========================================================================
		private void toolBar_ButtonClick(
			object sender, 
			System.Windows.Forms.ToolBarButtonClickEventArgs e
			)
		{
			try 
			{
				this.Cursor = Cursors.WaitCursor ;
				ToolBar tb = (ToolBar)sender ;
				int which_button = (int)tb.Buttons.IndexOf(e.Button);

				switch(which_button)
				{
					case 1:
						// Insert code to call the component.
						this.behind_.on_test_requested( this.last_selected_progid_ ,this.selectedNode);						
						break; 
					case 3:
						// open config file and exit the app
						if ( this.behind_.open_config_file() )
						{
							this.behind_ = null ;
							Application.Exit() ;
						}
						break; 
					case 5:
						// Insert code to copy the output to the clipboard.  
						this.behind_.copy_output(sender,e);					  
						break;
					case 7:
						// results browser
						this.behind_.open_result_file() ;
						break;
					case 9:
						// EXIT
						this.behind_ = null ;
						Application.Exit() ;
						break;
					case 11:
						// About modal dialog
						(new dbj.about.AboutDialog()).ShowDialog(this) ;
						break;
					default:
						// an separator button ...
						break ;
				}
			} 
			catch ( System.Exception x ) {
				writeln( x.GetType().Name + " : " + x.Message ) ;
			}
			finally {
				this.Cursor = Cursors.Default ;
			}
		}		
		
		//========================================================================
		private string last_selected_progid_ = string.Empty ;
		private System.Windows.Forms.TreeNode selectedNode =  null ;
		//========================================================================
		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if(e.Node.Parent==null) 
			{
				selectedNode = e.Node ;
				CFG.EP ep = (CFG.EP)e.Node.Tag ;
				this.last_selected_progid_  = ep.progid ;
				this.behind_.status( ep.description );
				this.writeln( string.Empty ) ;
				this.writeln( ep.request ) ;
				this.writeln( string.Empty ) ;
			}
		}

	} // eof Form1
}
