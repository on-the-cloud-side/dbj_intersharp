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
	using System.Xml ;
	using System.IO ;
	using System.Runtime ;
	/// <summary>
	/// Summary description for ResultFile.
	/// </summary>
	public class ResultFile
	{
		string		  filepath			= string.Empty ;
		string		  progid			= string.Empty ;
		string		  xml_req			= string.Empty ;
		string		  no_calls			= string.Empty ;
		string		  wait_time			= string.Empty ;
		XmlDocument	  xml_doc			= null ;

		private ResultFile () { /* frobidden*/ }

		public ResultFile( string new_progid ,string xml_request,string max_of_calls,string wait, DateTime begin_time )
		{
			this.progid		= new_progid   ;
			this.xml_req	= xml_request  ;
			this.no_calls   = max_of_calls ;
			this.wait_time  = wait         ;
			
			// after instance is made, users may need to know the filepath!
			this.filepath = ResultFile.format_file_name(
				folder_path(), 
				this.progid, 
				begin_time, 
				"yyyyMMddHHmmss") ;

			this.filepath = Path.GetFullPath(this.filepath).ToString();
			// create new xmldocument.
			xml_doc			= new XmlDocument();
			create_results_folder();
			create_xml_doc();
		}

		~ResultFile()
		{
			xml_doc=null;
		}

		// read only property
		public string file_path { get { return this.filepath ; } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//public XmlDocument create_xml_file()
		private void create_xml_doc()
		{
			CFG tzx_config = CFG.singleton();

			string xsl_type = tzx_config.coreconf.get_value("xsltype");
			string xslfile = tzx_config.coreconf.get_value("xslfile");
			string xsl_data="type='" + xsl_type + "'" +"  " + "href='" + xslfile +"'"; 
					
			// create xml declaration.
			XmlDeclaration xml_declaration=xml_doc.CreateXmlDeclaration("1.0",null,null); 
			//create xml stylesheet processing instruction				
			XmlProcessingInstruction xml_processing_inst=xml_doc.CreateProcessingInstruction("xml-stylesheet",xsl_data );
			xml_doc.AppendChild(xml_processing_inst); 
			XmlElement root_ele=xml_doc.CreateElement("test");
			xml_doc.AppendChild(root_ele);

			string machine_name = dbj.fm.util.computer_name() ;
			root_ele.AppendChild(create_element("date",format_date(DateTime.Now,"yyyy-MM-dd"),xml_doc));
			root_ele.AppendChild(create_element("time",format_date(DateTime.Now,"T"),xml_doc));
			//creating progid element.
			//XmlElement root_element=xml_doc.DocumentElement ;
			root_ele.AppendChild(create_element("progid",progid,xml_doc)); 
			//creating character data section
			root_ele.AppendChild(create_cd_element("request",xml_req,xml_doc)); 
			root_ele.AppendChild(create_element("no_of_calls",no_calls,xml_doc)); 
			root_ele.AppendChild(create_element("call_delay",wait_time,xml_doc)); 
			root_ele.AppendChild(create_element("machine_name",dbj.fm.util.computer_name() ,xml_doc)); 
			//return xml_doc;
		}
		/// <summary>
		/// creates a call element
		/// </summary>
		/// <param name="call_num"></param>
		/// <param name="response"></param>
		/// <param name="elapsed_time"></param>
		public void add_call_element(string call_num,string response , string elapsed_time )
		{
			XmlElement call_ele= create_element("call","",xml_doc); 
			call_ele.SetAttributeNode(create_attribute(xml_doc,"count",call_num ));
			call_ele.AppendChild(create_cd_element("result",response,xml_doc)); 
			call_ele.AppendChild(create_element("elapsed",elapsed_time ,xml_doc)); 
			xml_doc.DocumentElement.AppendChild(call_ele);
		}
		public void close_test(string total_elapsed_time,DateTime begin_time)
		{
			// creating total elapsed time tag.
			xml_doc.DocumentElement.AppendChild(create_element("elapsed",total_elapsed_time ,xml_doc)); 
			xml_doc.DocumentElement.AppendChild(create_comment_ele("This is the total elapsed time",xml_doc));
			
			this.create_xml_file( this.file_path );				
		}
		/// <summary>
		/// create xml file from the xml_doc
		/// </summary>		
		private void create_xml_file(string path)
		{
			XmlTextWriter xml_writer= new XmlTextWriter(path,null);
			xml_writer.Formatting=Formatting.Indented ; 
				xml_writer.Flush();
					xml_doc.Save(xml_writer);			
				xml_writer.Close();
		}

	
		/// <summary>
		/// create new xml element
		/// </summary>
		/// <param name="ele_name"></param>
		/// <param name="inner_text"></param>
		/// <param name="doc"></param>
		/// <returns></returns>
		public XmlElement create_element(string ele_name,string inner_text,XmlDocument doc)
		{
			//XmlElement root=doc.DocumentElement;
			XmlElement new_element=doc.CreateElement(ele_name);
			new_element.InnerText =inner_text ;
			return new_element;
		}
		/// <summary>
		/// creates a character data section element
		/// </summary>
		/// <param name="ele_name"></param>
		/// <param name="inner_text"></param>
		/// <param name="doc"></param>
		/// <returns></returns>
		public XmlElement create_cd_element(string ele_name,string inner_text,XmlDocument doc)
		{
			XmlElement new_element=doc.CreateElement(ele_name);

			if ( inner_text.IndexOf("]]>") > 0 ) 
				inner_text = inner_text.Replace("]]>", "]]]") ;

			if ( inner_text.IndexOf("]]&gt;") > 0 ) 
				inner_text = inner_text.Replace("]]>", "]]]") ;

			XmlCDataSection cd_element=doc.CreateCDataSection(inner_text);
			new_element.AppendChild(cd_element);
			return new_element;
		}
		/// <summary>
		/// converts the datetime to required format.
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		internal static string format_date(DateTime dt,string format)
		{
			return dt.ToString(format);
		}
		/// <summary>
		/// creates xml attribute
		/// </summary>
		/// <param name="xml_doc"></param>
		/// <param name="attr_name"></param>
		/// <param name="attr_value"></param>
		/// <returns></returns>
		public XmlAttribute create_attribute(XmlDocument xml_doc,string attr_name,string attr_value)
		{
			XmlAttribute xml_attr=xml_doc.CreateAttribute(attr_name);
			xml_attr.Value =attr_value; 
			return xml_attr;
		}
		/// <summary>
		/// creates file name.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="progid"></param>
		/// <param name="dt"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string format_file_name(string path,string progid,DateTime dt,string format)
		{
			//tzxcfg.grand_child_value("endpoint","filepath")+ "//" +tzxcfg.grand_child_value("endpoint","progid")+"_"+total_begin.ToString("yyyyMMddHHmmss")+".xml" 
			return path+ "//" +progid +"_"+format_date(dt,format)+".xml" ;
		}
		/// <summary>
		/// creates commet section in xml file.
		/// </summary>
		/// <param name="comment_text"></param>
		/// <param name="doc"></param>
		/// <returns></returns>
		public XmlComment create_comment_ele(string comment_text,XmlDocument doc)
		{
			XmlComment xml_comment=doc.CreateComment(comment_text);
			return xml_comment;
		}
		/// <summary>
		/// checks for the existence of results folder , if not creates one.
		/// </summary>
		public static void create_results_folder()
		{
			if(!Directory.Exists(folder_path()) )
			Directory.CreateDirectory( folder_path());  			
		}

		public static string folder_path ()
		{
			return dbj.fm.util.codebase + "//results" ;
		}
	}
}
