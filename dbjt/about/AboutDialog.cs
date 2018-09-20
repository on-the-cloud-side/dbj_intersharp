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

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace dbj.about
{
    /// <summary>
    /// Summary description for AboutDialog.
    /// </summary>
    internal class AboutDialog : System.Windows.Forms.Form  
    {
        private System.Windows.Forms.PictureBox logoBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richCreditsBox;
        private System.Windows.Forms.TextBox copyrightLabel;
        private System.Windows.Forms.TextBox versionLabel;
		// private Skybound.VisualStyles.VisualStyleProvider visualStyleProvider;
		private System.Windows.Forms.LinkLabel linkLabel;

		// IMPORTANT:
		// resource names are case sensitive
		// full resource paths are to be given 
		// if in doubt ALWAYS chack on these proper names by using Util.show_resources() 
		//
		private readonly static string logobox_image_name = "dbjept.about.dbjatwork.gif" ;
		private readonly static string credibox_file_name = "dbjept.about.about.rtf" ;

		//
		private readonly static int dialogue_width = 640 ;

        public AboutDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.Text = "About " + Application.ProductName;

			this.logoBox.SizeMode = PictureBoxSizeMode.CenterImage ;
            this.logoBox.Image = Util.GetImageResource(logobox_image_name);
            this.logoBox.Width = dialogue_width ;
            this.SetClientSizeCore (dialogue_width, this.ClientRectangle.Height);

            this.versionLabel.Text = Application.ProductName + "[" + Application.ProductVersion + "]" ;
			this.linkLabel.Text = "Go to the " + Application.CompanyName + ", web site." ;
			try 
			{
				this.richCreditsBox.LoadFile(Util.GetResourceStream(credibox_file_name), RichTextBoxStreamType.RichText);
			} 
			catch ( Exception x ) 
			{
				this.richCreditsBox.Text = x.ToString() ;
			}
			this.richCreditsBox.Width = this.ClientRectangle.Width - 20 ;
            this.copyrightLabel.Text = "(c) 2003-2006-2018 by " + Application.CompanyName ;

            // this.Icon = new Icon(Util.GetResourceStream("dbj.ico"));
		}

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AboutDialog));
			this.okButton = new System.Windows.Forms.Button();
			this.logoBox = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.richCreditsBox = new System.Windows.Forms.RichTextBox();
			this.copyrightLabel = new System.Windows.Forms.TextBox();
			this.versionLabel = new System.Windows.Forms.TextBox();
			this.linkLabel = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.okButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.okButton.Location = new System.Drawing.Point(173, 399);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(90, 27);
			this.okButton.TabIndex = 2;
			this.okButton.Text = "OK";
			// 
			// logoBox
			// 
			this.logoBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.logoBox.Location = new System.Drawing.Point(0, 0);
			this.logoBox.Name = "logoBox";
			this.logoBox.Size = new System.Drawing.Size(120, 58);
			this.logoBox.TabIndex = 3;
			this.logoBox.TabStop = false;
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label1.Location = new System.Drawing.Point(8, 157);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(125, 18);
			this.label1.TabIndex = 5;
			this.label1.Text = "Credits:";
			// 
			// richCreditsBox
			// 
			this.richCreditsBox.CausesValidation = false;
			this.richCreditsBox.Location = new System.Drawing.Point(12, 175);
			this.richCreditsBox.Name = "richCreditsBox";
			this.richCreditsBox.ReadOnly = true;
			this.richCreditsBox.Size = new System.Drawing.Size(397, 217);
			this.richCreditsBox.TabIndex = 7;
			this.richCreditsBox.Text = "richCreditsBox.Text";
			this.richCreditsBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richCreditsBox_LinkClicked);
			// 
			// copyrightLabel
			// 
			this.copyrightLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.copyrightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
			this.copyrightLabel.Location = new System.Drawing.Point(11, 88);
			this.copyrightLabel.Multiline = true;
			this.copyrightLabel.Name = "copyrightLabel";
			this.copyrightLabel.ReadOnly = true;
			this.copyrightLabel.Size = new System.Drawing.Size(403, 41);
			this.copyrightLabel.TabIndex = 9;
			this.copyrightLabel.Text = "";
			// 
			// versionLabel
			// 
			this.versionLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.versionLabel.Location = new System.Drawing.Point(11, 70);
			this.versionLabel.Name = "versionLabel";
			this.versionLabel.ReadOnly = true;
			this.versionLabel.Size = new System.Drawing.Size(393, 15);
			this.versionLabel.TabIndex = 10;
			this.versionLabel.Text = "";
			// 
			// linkLabel
			// 
			this.linkLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.linkLabel.LinkArea = new System.Windows.Forms.LinkArea(0, 9);
			this.linkLabel.Location = new System.Drawing.Point(8, 132);
			this.linkLabel.Name = "linkLabel";
			this.linkLabel.Size = new System.Drawing.Size(405, 20);
			this.linkLabel.TabIndex = 11;
			this.linkLabel.TabStop = true;
			this.linkLabel.Text = "linkLabel";
			this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// AboutDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.CancelButton = this.okButton;
			this.ClientSize = new System.Drawing.Size(432, 432);
			this.Controls.Add(this.linkLabel);
			this.Controls.Add(this.versionLabel);
			this.Controls.Add(this.copyrightLabel);
			this.Controls.Add(this.richCreditsBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.logoBox);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AboutDialog";
			this.ResumeLayout(false);

		}
        #endregion

        private void richCreditsBox_LinkClicked(object sender, System.Windows.Forms.LinkClickedEventArgs e)
        {
            if (null != e.LinkText && e.LinkText.StartsWith("http://"))
            {
                System.Diagnostics.Process.Start(e.LinkText);
            }
        }

		private void linkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
				System.Diagnostics.Process.Start(new Uri("https://dbj.systems").ToString());
		}
    }
}
