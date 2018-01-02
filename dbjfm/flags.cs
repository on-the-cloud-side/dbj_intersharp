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

namespace dbj.fm
{
	using System;

	/// <summary>
	/// Bit field encapsulation. Alloes enums and their element to be treated and operated upon,
	/// as bit fields.
	/// </summary>
	/// <remarks>
	/// An attribute [Flags] must be used on the enum whose elements you want to use as 'flags'. Attempt to use enums
	/// without this attribute will cause an exception. See the example.
	/// </remarks>
	/// <example><code>
	///
	///		class driver
	///		{
	///			[Flags]
	///				enum opis { dupe = 1,sise = 2 ,noge = 4, faca = 8 }
	///				enum wrong { a,b,c,d }
	///
	///			[STAThread]
	///			static void Main(string[] args)
	///			{
	///				try 
	///				{
	///					dbj.flags mistaken = new dbj.flags( wrong.a ) ;
	///				} 
	///				catch ( dbj.flags.exception x )
	///				{
	///					Console.WriteLine( x.Message ) ;
	///				}
	///				dbj.flags bf = new dbj.flags( opis.dupe | opis.noge ) ;
	///
	///				compare(bf, opis.dupe) ;
	///				compare(bf, opis.sise) ;
	///				compare(bf, opis.noge) ;
	///				compare(bf, opis.faca) ;
	///
	///				compare( bf, new dbj.flags( opis.noge ) ) ;
	///				compare( bf, bf ) ;
	///
	///				compare( opis.dupe , opis.dupe | opis.noge ) ;
	///				compare( opis.dupe | opis.noge , opis.dupe | opis.noge ) ;
	///
	///			}
	///			static void compare ( dbj.flags bf , Enum v ) 
	///			{
	///				// here we use the '==' operator
	///				Console.WriteLine ( "[{0}] == [{1}], returns {2}" , bf,v, bf == v ) ;
	///			}
	///			static void compare ( dbj.flags bf1 , dbj.flags bf2 ) 
	///			{
	///				compare( bf1, (Enum)bf2 ) ;
	///			}
	///			static void compare ( Enum v1 , Enum v2 ) 
	///			{
	///			    // here we use the flags.Equals() method
	///				Console.WriteLine ( "[{0}] == [{1}], returns {2}" , v1,v2, v1.Equals(v2) ) ;
	///			}
	///		}
	///
	///	}
	///</code></example>
	[CLSCompliant(true)]
	public sealed class flags 
	{	
		/// <summary>
		/// Used for formating exception raised or occured while using flags
		/// </summary>
		public sealed class exception : System.ComponentModel.InvalidEnumArgumentException 
		{
			private readonly string message_ = string.Empty ;
			/// <summary>
			/// Exception class used to better explain what is worng with your usage of dbj.fm.flags
			/// </summary>
			public exception ( string argumentName, int invalidValue, Type enumClass)
				: base( argumentName, invalidValue , enumClass )
			{
				this.message_ =
					string.Format("Type of the argument '{0}' of value {1} is Enum type '{2}'.{3}But this Enum type is not decorated with {4}" ,
					argumentName, invalidValue, enumClass.Name , Environment.NewLine , typeof( FlagsAttribute ).Name ) ;
			}
			/// <summary>
			/// returns exception message
			/// </summary>
			public override string Message { get { return this.message_ ;	}	}
		}

		ValueType flags_ ;

		private flags () {}
		/// <summary>
		/// flags class can be constructed only through this constructor
		/// </summary>
		/// <param name="v_">A valid Enum instance</param>
		public flags( Enum v_ ) 
		{ 
			
			if ( ! is_flag( v_ ) )
				throw new flags.exception (
					"v_", // argumentName,
					(int)(ValueType)v_, // invalidValue,
					v_.GetType() // enumClass
					);
			flags_ = (ValueType)v_ ; 
		}
		/// <summary>
		/// Check if the enum is adorned with the required attribute: 'Flags'
		/// </summary>
		/// <param name="e_">Enum instance</param>
		/// <returns>true if 'Flags' is on of the attributes, false otherwise</returns>
		public static bool is_flag ( Enum e_ )
		{
			return System.Attribute.IsDefined( e_.GetType(), typeof( FlagsAttribute )) ;
		}
		/// <summary>
		/// Return true if enum parameter is present insde flags, in the current instance of this class.
		/// </summary>
		/// <param name="e">Enum instance</param>
		/// <returns>true or false</returns>
		public bool contains ( Enum e ) { return this.Equals(e); }
		/// <summary>
		/// Checks wether specified flag is equal to enum value passed to the method
		/// </summary>
		/// <param name="bf">flags</param>
		/// <param name="e">enum</param>
		/// <returns>true or false</returns>
		public static bool operator == ( flags bf, Enum e ) 
		{
			return bf.Equals(e) ;
		}
		/// <summary>
		/// Checks wether specified flag is not equal to enum value passed to the method
		/// </summary>
		/// <param name="bf">flags</param>
		/// <param name="e">enum</param>
		/// <returns>true or false</returns>
		public static bool operator != ( flags bf, Enum e ) 
		{
			return ! bf.Equals(e) ;
		}
		/// <summary>
		/// method using the '==' (Equals) operator on elements of Flags.
		/// </summary>
		/// <param name="e">Flags element</param>
		/// <returns>true or false</returns>
		public override bool Equals(object e)
		{
			return ( (int)flags_ & (int)e ) == (int)e  ;
		}
		/// <summary>
		/// Obtains the hashcode of the flags
		/// </summary>
		public override int GetHashCode()
		{
			return flags_.GetHashCode() ;
		}
		/// <summary>
		/// Obtains string form of the flags
		/// </summary>
		public override string ToString()
		{
			return flags_.ToString() ;
		}		
		/// <summary>
		/// An explicit conversion to an Enum
		/// </summary>
		static public explicit operator Enum(flags bf)
		{
			return (Enum)bf.flags_ ;
		}
	}
}
