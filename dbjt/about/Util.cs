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

namespace dbj.about
{
using System;
using System.Diagnostics ;
using System.Drawing ;
using System.Reflection ;
using System.Windows.Forms ;
	/// <summary>
	/// Utilities implementations
	/// </summary>
	internal sealed class Util
	{

		private Util () { /* can't make this class */ }

		public static void show_resources ( )
		{
			Assembly this_asm = Assembly.GetExecutingAssembly() ;
			string [] resources = this_asm.GetManifestResourceNames() ;
			System.Diagnostics.Debug.WriteLine("Assembly: " + this_asm.FullName  ) ;
			foreach ( string res_name in resources )
			{
				System.Diagnostics.Debug.WriteLine("Resource: " + res_name ) ;

				ManifestResourceInfo mri =  this_asm.GetManifestResourceInfo( res_name ) ;
				System.Diagnostics.Debug.WriteLine("\tFile Name: " + ( mri.FileName == null ? "NULL" : mri.FileName ) ) ;
				System.Diagnostics.Debug.WriteLine("\tReferenced Assembly: " + ( mri.ReferencedAssembly == null ? "NULL" : mri.ReferencedAssembly.FullName ) ) ;
				System.Diagnostics.Debug.WriteLine("\tLocation: " + mri.ResourceLocation.ToString() ) ;
			}
		}
		//-------------------------------------------------------------------------------------------------
		public static Image GetImageResource(string fileName)
		{
			StackTrace trace = new StackTrace();
			StackFrame parentFrame = trace.GetFrame(1);
			MethodBase parentMethod = parentFrame.GetMethod();
			Type parentType = parentMethod.DeclaringType;
			Assembly parentAssembly = parentType.Assembly;

#if DEBUG
			show_resources() ;
#endif
			System.IO.Stream stream = parentAssembly.GetManifestResourceStream(fileName);
			Image image = Image.FromStream(stream);
			return image;
		}
		//-------------------------------------------------------------------------------------------------
		public static System.IO.Stream GetResourceStream(string fileName)
		{
			StackTrace trace = new StackTrace();
			StackFrame parentFrame = trace.GetFrame(1);
			MethodBase parentMethod = parentFrame.GetMethod();
			Type parentType = parentMethod.DeclaringType;
			Assembly parentAssembly = parentType.Assembly;
			return parentAssembly.GetManifestResourceStream(fileName);
		}

		//-------------------------------------------------------------------------------------------------
		// Example
		//	If the full name of type is "MyNameSpace.MyClasses" and name is "Net", 
		//  GetManifestResourceStream will search for a resource named 
		//  MyNameSpace.Net.
        //
		public static System.IO.Stream GetResourceStream(Assembly assembly, string namespaceName, string fileName)
		{
#if DEBUG
			string full_name  = assembly.GetType().FullName ;
			string nspace_name  = assembly.GetType().Namespace ;
			string look_for = nspace_name + "." + fileName ;
#endif
			return assembly.GetManifestResourceStream(namespaceName + "." + fileName);
		}

		//-------------------------------------------------------------------------------------------------
		public static void ErrorBox(System.Windows.Forms.IWin32Window parent, string message)
		{
			System.Windows.Forms.MessageBox.Show(
					parent, message, Application.ProductName, 
					System.Windows.Forms.MessageBoxButtons.OK, 
					System.Windows.Forms.MessageBoxIcon.Error);
		}

	} // eof class Util



}

#if SPECIAL_INFO_PROVIDER 
namespace dbj 
{
	/// <summary>
	/// An assembly information provider.
	/// </summary>
	internal sealed class assembly_info
	{
		private static readonly object locker_ = new object() ;
		private assembly_info()		{ 	}
		//-------------------------------------------------------------------------
		/// <summary>
		/// Attributes supported
		/// </summary>
		public enum Attribute_id 
		{
			Title,
			Description,
			Configuration,
			Company,
			Product,
			Copyright,
			Trademark,
			Culture,
			Version,
			DelaySign,
			KeyFile,
			KeyName
		} ;

		//-------------------------------------------------------------------------
		private static object [] attributes_ = null ;
		private static object [] attributes 
		{
			get {
				if ( assembly_info.attributes_ == null ) 
				attributes_ = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(false);
				return assembly_info.attributes_ ;
			}
		}
		//-------------------------------------------------------------------------
		private static object find_attribute ( Attribute_id atid_ )
		{
			string att_name = "Assembly" + atid_.ToString() + "Attribute" ;

			foreach ( object att in assembly_info.attributes )
			{
				if ( att.GetType().Name.StartsWith( att_name ) ) return att ; 
			}
				return null ; // NOT found
		}
		//-------------------------------------------------------------------------
		public static string attribute_value ( Attribute_id atid_, string property_name )
		{
			lock ( assembly_info.locker_ )
			{
				object the_att = assembly_info.find_attribute( atid_ ) ;

				if ( the_att == null ) return string.Empty ;

				System.Reflection.PropertyInfo pi_ =   the_att.GetType().GetProperty( property_name ) ;

				if ( pi_ != null ) 
					return (string)pi_.GetValue(the_att,null) ;

				return string.Empty ;
			}
		}
		/// <summary>
		/// Simpler to use. We always want same properties from same assembly attributes
		/// </summary>
		public static string attribute_value ( Attribute_id atid_ )
		{
			lock ( assembly_info.locker_ )
			{
				object the_att = assembly_info.find_attribute( atid_ ) ;

				if ( the_att == null ) 	
#if DEBUG
					return "Attribute " + atid_.ToString() + " NOT found on executing assembly" ;
#else
				throw new System.ApplicationException( "Attribute " + atid_.ToString() + " NOT found on executing assembly" ) ;
#endif

				System.Reflection.PropertyInfo pi_ = null ;
				string prop_name = string.Empty ;
				
				switch ( atid_ )
				{
					case Attribute_id.Title : 
						prop_name = ( "Title" ) ;
						break ;
					case Attribute_id.Description : 
						prop_name = ( "Description" ) ;
						break ;
					case Attribute_id.Configuration : 
						prop_name = ( "Configuration" ) ;
						break ;
					case Attribute_id.Company : 
						prop_name = ( "Company" ) ;
						break ;
					case Attribute_id.Product : 
						prop_name = ( "Product" ) ;
						break ;
					case Attribute_id.Copyright : 
						prop_name = ( "Copyright" ) ;
						break ;
					case Attribute_id.Trademark : 
						prop_name = ( "Trademark" ) ;
						break ;
					case Attribute_id.Culture : 
						prop_name = ( "Culture" ) ;
						break ;
					case Attribute_id.Version : 
						prop_name = ( "Version" ) ;
						break ;
					case Attribute_id.DelaySign : 
						prop_name = ( "DelaySign" ) ;
						break ;
					case Attribute_id.KeyFile : 
						prop_name = ( "KeyFile" ) ;
						break ;
					case Attribute_id.KeyName : 
						prop_name = ( "KeyName" ) ;
						break ;
				}

				pi_ = the_att.GetType().GetProperty ( prop_name ) ;
				if ( pi_ != null ) 
					return (string)pi_.GetValue(the_att,null) ;
#if DEBUG
				return ( "Property by name: " + prop_name+ ", NOT found on attribute " + the_att.GetType().Name ) ;
#else
				throw new System.ApplicationException( "Property by name: " + prop_name+ ", NOT found on attribute " + the_att.GetType().Name ) ;
#endif
			}
		}
#if DEBUG
		public static string all2string ()
		{
			string rez = string.Empty ;
			foreach ( string name_ in System.Enum.GetNames( typeof(assembly_info.Attribute_id )) )
			{
				try  {
					rez += "\n" + name_ + " = " + attribute_value( 
						(Attribute_id)System.Enum.Parse( typeof(Attribute_id), name_) 
						) ; 
				} catch ( System.Exception x ) {
					rez += "\n" + name_ + " = " + x.Message ;
				}
			}
				return rez ;
		}
#endif
	} // eof assembly_info
} // eof dbj
#endif // SPECIAL_INFO_PROVIDER 