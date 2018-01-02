
namespace dbj.fm.reflection
{
using System  ;	
using fm ;
using fm.data ;

		/// <summary>
		/// Reflection service 
		/// </summary>
		internal sealed class service : fm.Itypeinfo 
		{
			internal service ()	{ }
			/// <summary>
			/// describes about the type 
			/// </summary>
			/// <param name="specimen">Object to examine</param>
			/// <param name="what_part">what the caller wants to describe</param>
			/// <returns></returns>
			public override string describe(object specimen, fm.Itypeinfo.WHAT what_part)
			{
				lock ( this ) 
				{
					reflector reflector_ = new reflector(specimen);
					return reflector_.describe( what_part ) ;
				}
			}
		}
	
		/// <summary>
		/// This reflector sheds the light on the object given. 
		/// Example :
		/// 
		///	the_specimen = make_complus_instance( "TZXMSG.IMAGECOLLECTOR", "LOCALHOST" ) ;
		///	fm.reflector reflec = new fm.reflector( the_specimen) ;
		///	Console.WriteLine( 
		///		reflec.describe( 
		///			fm.reflector.Itypeinfo.WHAT.PARENT 
		///			| fm.reflector.Itypeinfo.WHAT.INTERFACES 
		///			| fm.reflector.Itypeinfo.WHAT.METHODS  
		///			)
		///		);
		///</summary>
		class reflector : System.IDisposable
		{
			//-----------------------------------------------------------
			/// <summary>
			/// Contains various string definitions used in reflection
			/// </summary>
			class STRNGS 
			{
				public static readonly string CR  = Environment.NewLine  ; // same as Environment.NewLine
				public static readonly string LNE = CR + "----------------------------------------------------" ;
				public static readonly string IFS = CR + "Interfaces" ;
				public static readonly string FDS = CR + "Fields" ;
				public static readonly string PTS = CR + "Properties" ;
				public static readonly string MTS = CR + "Methods" ;
				public static readonly string BTP = CR + "Base" ;
				public static readonly string NRA = "No read access" ;
				public static readonly string NME = "Name" ;
				public static readonly string ATR = "\t" ;
				// eXception Strings
				public static readonly string XS1 = "Reflection Permision NOT granted." ;
			}
			//-----------------------------------------------------------
			private object specimen_ = null ;

			/// <summary>
			/// default ctor is forbiden
			/// </summary>
			private reflector () {}

			/// <summary>
			/// construct and take the instance to examine
			/// </summary>
			/// <param name="to_examine">object to examine</param>
			internal reflector ( object to_examine ) 
			{ 

				System.Security.Permissions.ReflectionPermission perm =	
					new System.Security.Permissions.ReflectionPermission(
					System.Security.Permissions.ReflectionPermissionFlag.AllFlags 
					) ;
				if ( ! util.have_permission( perm ) )
					throw new ApplicationException( STRNGS.XS1 ) ;

				this.specimen_ = to_examine; 
			}
			/// <summary>
			/// The only methods on the public interface used for getting the description.
			/// </summary>
			/// <param name="what_part">or-ed enum values teling us what caller wants described</param>
			/// <returns>report in a string</returns>
			public string describe ( fm.Itypeinfo.WHAT what_part )
			{
				lock (this)
				{
					fm.flags what = new flags( what_part ) ;
					bool SHOW_ATTR = what.contains( Itypeinfo.WHAT.ATTRIBUTES ) ;
				
					Type type =  this.specimen_.GetType() ;
					System.Text.StringBuilder retval = new System.Text.StringBuilder() ;
					// this info we return always
					retval.AppendFormat( "{0}{1}", Environment.NewLine , STRNGS.NME ) ;
					retval.AppendFormat( "{0}\t{1}", Environment.NewLine , base_data(type, true ) ) ;


					if (  what == Itypeinfo.WHAT.INTERFACES )
					{
						bool show_m = what.contains( Itypeinfo.WHAT.INTERFACE_METHODS ) ;
						retval.Append( interfaces(this.specimen_, SHOW_ATTR, show_m ) ) ;
					}
			
					if ( what == Itypeinfo.WHAT.PARENT )
						if ( type.BaseType != null )
						{
							retval.Append( STRNGS.BTP ) ;
							retval.AppendFormat( "{0}\t{1}", Environment.NewLine , base_data( type.BaseType, SHOW_ATTR ) ) ;
						}
			
					if ( what == Itypeinfo.WHAT.FIELDS  )
						retval.Append( fields(this.specimen_) ) ;
			
					if ( what == Itypeinfo.WHAT.PROPERTIES )
						retval.Append( propertys(this.specimen_) ) ;

					if ( what == Itypeinfo.WHAT.METHODS )
						retval.Append( methods(this.specimen_.GetType(), SHOW_ATTR ) ) ;

					return retval.ToString() ;
				}
			}
			//---------------------------------------------------------------------------
			/// <summary>
			/// Returns formated string containg type name and its attributes 
			/// </summary>
			/// <param name="type">Name of type</param>
			/// <param name="with_attributes">boolean value indicating wether given contains attributes or not</param>
			/// <returns>string</returns>
			string base_data ( Type type, bool with_attributes )
			{
				if ( with_attributes )
					return string.Format( "{0}{1}\t[{2}]" , type.FullName , Environment.NewLine,   type.Attributes ) ;
				else
					return string.Format( "{0}" , type.FullName ) ;
			}
			//---------------------------------------------------------------------------
			/// <summary>
			/// Examines the type and returns a string containing list of fields contained in the type
			/// </summary>
			/// <param name="specimen">object to examine</param>
			/// <returns>string representing the fileds in the given type</returns>
			string fields ( object specimen )
			{
				System.Text.StringBuilder retval = new System.Text.StringBuilder() ;
				Type   type   = null ;
				try 
				{
					type= specimen.GetType() ;
					if ( type.GetFields().Length < 1 ) return "" ;
					retval.Append( STRNGS.FDS ) ;
					foreach ( System.Reflection.FieldInfo  finfo in type.GetFields() )
					{
						retval.AppendFormat( "{0}{1}.{2} = {3}" , 
							Environment.NewLine, type.Name , finfo.Name , finfo.GetValue( specimen ) ) ;
					}
				} 
				catch ( Exception x ) 
				{
					retval.AppendFormat( "{0}{1}:{2}", Environment.NewLine, x.GetType().FullName, x.Message ) ;
				}
				return retval.ToString() ;
			}
			//---------------------------------------------------------------------------
			/// <summary>
			/// Examines the type and returns a string containing list of properties contained in the type
			/// </summary>
			/// <param name="specimen">object to examine</param>
			/// <returns>string representing the propertys in the given type</returns>
			string propertys ( object specimen )
			{
				System.Text.StringBuilder retval = new System.Text.StringBuilder() ;
				object result = null ;
				Type type = specimen.GetType() ;
				try 
				{
					if ( type.GetProperties().Length  < 1 ) return "" ;
					retval.Append( STRNGS.PTS ) ;
					foreach ( System.Reflection.PropertyInfo pinfo in type.GetProperties())
					{
						if ( pinfo.CanRead )
							result = pinfo.GetGetMethod().Invoke(specimen, null) ;
						else 
							result = STRNGS.NRA; // "No read access" ;
						retval.AppendFormat( "{0}{1}.{2} = {3}" , Environment.NewLine, type.Name , pinfo.Name , result ) ;
					}
				} 
				catch ( Exception x )
				{
					retval.AppendFormat( "{0}{1}:{2}", Environment.NewLine,  x.GetType().Name, x.Message );
				}
				finally 
				{
					result = null ; type = null ;
				}
				return retval.ToString() ;
			}
			//---------------------------------------------------------------------------
			/// <summary>
			/// Examines the type and returns a string containing list of Methods contained in the type
			/// </summary>
			/// <param name="type">Name of the type to examine</param>
			/// <param name="with_attributes">boolean value </param>
			/// <returns>string representing the methods in the given type</returns>
			string methods ( Type type, bool with_attributes )
			{
				System.Text.StringBuilder retval = new System.Text.StringBuilder() ;
				try 
				{
					if ( type.GetMethods().Length  < 1 ) return "" ;
					retval.Append( STRNGS.MTS ) ;
					string result = "" ;

					foreach ( System.Reflection.MethodInfo minfo in type.GetMethods() )
					{
						result = minfo.ReturnType.Name + "\t" + minfo.Name + "( " ;
						foreach( System.Reflection.ParameterInfo pi in minfo.GetParameters() )
						{
							result += pi.ParameterType.Name + " " + pi.Name + ", " ;
						}
						if ( result.LastIndexOf(",") > 0 )
							result = result.Remove( result.LastIndexOf(","), 1 ) ;

						result += " )" ;
						if ( with_attributes )
							result += Environment.NewLine + "\t[" + minfo.Attributes.ToString() + "]" ;
						retval.AppendFormat( "{0}\t{1}" , Environment.NewLine, result ) ;
					}
				} 
				catch ( Exception x )
				{
					retval.Append( x.GetType().Name + " : " + x.Message ) ;
				}
				return retval.ToString() ;
			}
			//---------------------------------------------------------------------------
			/// <summary>
			/// Examines the specified type and returns a string representing
			/// the interfaces implemented or inherited by the current Type.
			/// </summary>
			/// <param name="specimen">Object to examine</param>
			/// <param name="SHOW_ATTRIB">boolean value</param>
			/// <param name="show_methods">boolean value</param>
			/// <returns></returns>
			string interfaces ( object specimen, bool SHOW_ATTRIB, bool show_methods )
			{
				System.Text.StringBuilder retval = new System.Text.StringBuilder() ;
				try 
				{
					if ( specimen.GetType().GetInterfaces().Length < 1 ) return "" ;

					retval.Append( STRNGS.IFS ) ;
					foreach ( Type  iinfo in specimen.GetType().GetInterfaces() )
					{
						retval.AppendFormat("{0}\t{1}", Environment.NewLine, base_data(iinfo, SHOW_ATTRIB ) ) ;
						if ( show_methods )
							retval.Append( prefix_with( methods( iinfo, SHOW_ATTRIB ),"\t") ) ;
					}
				} 
				catch ( Exception x )
				{
					retval.Append( Environment.NewLine + "\t" + x.GetType().Name + " : " + x.Message ) ;
				}
				return retval.ToString() ;
			}
			/// <summary>
			/// prefix each text line found in a string with prefix given
			/// </summary>
			/// <param name="to_prefix">string of textlines</param>
			/// <param name="prefix">the prefix</param>
			/// <returns></returns>
			public static string prefix_with( string to_prefix, string prefix )
			{
				return prefix + to_prefix.Replace( Environment.NewLine , Environment.NewLine + prefix ) ;
			}
			//---------------------------------------------------------------------------
			#region IDisposable Members
			/// <summary>
			/// sets specimen_ object to null.
			/// </summary>
			public void Dispose()
			{
				this.specimen_ = null ;
			}

			#endregion
		} // eof reflector class

		//------------------------------------------------------------------------------
} // eof fm.reflection namespace

#region PROPERTIES OF THE Type class
	/*
	 * 
	 * 
	Assembly				Gets the Assembly that the type is declared in. 
	AssemblyQualifiedName	Gets the fully qualified name of the Type, including the name of the assembly from which the Type was loaded. 
	Attributes				Gets the attributes associated with the Type. 
	BaseType				Gets the type from which the current Type directly inherits. 
	DeclaringType			Overridden. Gets the class that declares this member. 
	DefaultBinder			Gets a reference to the default binder, which implements internal rules for selecting the appropriate members to be called by InvokeMember. 
	FullName				Gets the fully qualified name of the Type, including the namespace of the Type. 
	GUID					Gets the GUID associated with the Type.		
	HasElementType			Gets a value indicating whether the current Type encompasses or refers to another type; that is, whether the current Type is an array, a pointer, or is passed by reference. 
	IsAbstract				Gets a value indicating whether the Type is abstract and must be overridden. 
	IsAnsiClass				Gets a value indicating whether the string format attribute AnsiClass is selected for the Type. 
	IsArray					Gets a value indicating whether the Type is an array. 
	IsAutoClass				Gets a value indicating whether the string format attribute AutoClass is selected for the Type. 
	IsAutoLayout			Gets a value indicating whether the class layout attribute AutoLayout is selected for the Type. 
	IsByRef					Gets a value indicating whether the Type is passed by reference. 
	IsClass					Gets a value indicating whether the Type is a class; that is, not a value type or interface. 
	IsCOMObject				Gets a value indicating whether the Type is a COM object. 
	IsContextful			Gets a value indicating whether the Type can be hosted in a context. 
	IsEnum					Gets a value indicating whether the current Type represents an enumeration. 
	IsExplicitLayout		Gets a value indicating whether the class layout attribute ExplicitLayout is selected for the Type. 
	IsImport				Gets a value indicating whether the Type was imported from another class. 
	IsInterface				Gets a value indicating whether the Type is an interface; that is, not a class or a value type. 
	IsLayoutSequential		Gets a value indicating whether the class layout attribute SequentialLayout is selected for the Type. 
	IsMarshalByRef			Gets a value indicating whether the Type is marshaled by reference. 
	IsNestedAssembly		Gets a value indicating whether the Type is nested and visible only within its own assembly. 
	IsNestedFamANDAssem 	Gets a value indicating whether the Type is nested and visible only to classes that belong to both its own family and its own assembly. 
	IsNestedFamily			Gets a value indicating whether the Type is nested and visible only within its own family. 
	IsNestedFamORAssem		Gets a value indicating whether the Type is nested and visible only to classes that belong to either its own family or to its own assembly. 
	IsNestedPrivate			Gets a value indicating whether the Type is nested and declared private. 
	IsNestedPublic			Gets a value indicating whether a class is nested and declared public. 
	IsNotPublic				Gets a value indicating whether the top-level Type is not declared public. 
	IsPointer				Gets a value indicating whether the Type is a pointer. 
	IsPrimitive				Gets a value indicating whether the Type is one of the primitive types. 
	IsPublic				Gets a value indicating whether the top-level Type is declared public. 
	IsSealed				Gets a value indicating whether the Type is declared sealed. 
	IsSerializable			Gets a value indicating whether the Type is serializable. 
	IsSpecialName			Gets a value indicating whether the Type has a name that requires special handling. 
	IsUnicodeClass			Gets a value indicating whether the string format attribute UnicodeClass is selected for the Type. 
	IsValueType				Gets a value indicating whether the Type is a value type. 
	MemberType				Overridden. Gets a bitmask indicating the member type. 
	Module					Gets the module (the DLL) in which the current Type is defined. 
	Name					Gets the name of this member. 
	Namespace				Gets the namespace of the Type. 
	ReflectedType			Overridden. Gets the class object that was used to obtain this member. 
	TypeHandle				Gets the handle for the current Type. 
	TypeInitializer			Gets the initializer for the Type. 
	UnderlyingSystemType	Indicates the type provided by the common language runtime that represents this type. 
	*/
#endregion