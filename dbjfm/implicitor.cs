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

namespace dbj.fm
{
	/// <summary>Implicitly convert value contained in the instance of this class, into the required type</summary>
	[System.CLSCompliant(true)]
	public sealed class Implicitor
	{
		 
		object val_  ;
		/// <summary>
		/// Construct by delivering an arbitrary object.
		/// Unless the object given is another Implicitor class in which 
		/// case an Error will be thrown.
		/// </summary>
		/// <param name="val">value to be converted</param>
		public Implicitor( object val ) 
		{
			if ( val != null )
			{
				if ( val.GetType().Equals(this)) 
					throw new Error("Having nested Implicitors is probably NOT a good idea.") ;
				val_ = val ; 
			} 
			else 
			{
				val_ = null ;
			}
		}

		/// <summary>
		/// make exception
		/// </summary>
		/// <param name="msg">error message</param>
		/// <returns>the exception</returns>
		Error mex ( string msg ) 
		{
			Error x = new Error("Implicitor of value:" + this.val_.ToString() + ",has failed, error is: " + msg ) ;
			x.Source = "Implicitor" ;
			return x ;
		}
		/// <summary>
		/// converts the given date to yyyy-MM-dd format
		/// </summary>
		/// <param name="newdate">date to be converted</param>
		/// <returns>string representing the converted date</returns>	
		public static string date2sqldate(string newdate)
		{
			return System.DateTime.Parse(newdate).ToString("yyyy-MM-dd") ;
		}
		/// <summary>value as SQL friendly short date</summary>
		public string asSqlDate	
		{
			get  
			{
				try 
				{
					if ( this.val_ != null )
					{
						return date2sqldate( this.val_.ToString() );
					} else 
						return "NULL" ;
				} 
				catch ( Error x)
				{
					throw mex("Failed to convert to SqlDate, " + x.Message ) ;
				}

			}
		}
		/// <summary>
		/// converts the value contained in Implicitor to double
		/// </summary>
		/// <param name="ep">Implicitor</param>
		/// <returns>double</returns>
		public static implicit operator double(Implicitor ep) 
		{
			try 
			{
				return Convert.ToDouble( ep.val_ );
			} 	
			catch ( Error ){throw ep.mex("Failed to convert to double " ) ;}
		}
		/// <summary>
		/// converts the value contained in Implicitor to int
		/// </summary>
		/// <param name="ep">Implicitor</param>
		/// <returns>int</returns>
		public static implicit operator int (Implicitor ep) 
		{
			try 
			{
				return Convert.ToInt32( ep.val_ );
			} 	
			catch ( Error ){throw ep.mex("Failed to convert to int " ) ;}
		}
		/// <summary>
		/// converts the value contained in Implicitor to string
		/// </summary>
		/// <param name="ep">Implicitor</param>
		/// <returns>string</returns>
		public static implicit operator string (Implicitor ep) 
		{
			try 
			{
				if ( ep.val_ != null )
					return ep.val_.ToString() ;
				else
					return string.Empty ;
			} 	
			catch ( Error ){throw ep.mex("Failed to convert to string " ) ;}
		}
		/// <summary>
		/// converts the value contained in Implicitor to bool
		/// </summary>
		/// <param name="ep">Implicitor</param>
		/// <returns>bool</returns>
		public static implicit operator bool (Implicitor ep) 
		{
			try 
			{
				return Convert.ToBoolean ( ep.val_ );
			} 	
			catch ( Error ){throw ep.mex("Failed to convert to boolean " ) ;}
		}
		// ---------------------------------------------------------------------
		/// <summary>
		/// return val_ defined in here to string
		/// </summary>
		/// <returns>string </returns>
		public override string ToString() { return val_.ToString(); }
		// ---------------------------------------------------------------------
		// quite the compiler warning with these two overloads
		/// <summary>
		/// compares with object
		/// </summary>
		/// <param name="o">object</param>
		/// <returns>bool</returns>
		public override bool Equals(object o) {   return base.Equals(o); }
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() {	return base.GetHashCode(); 	}
		// binary  operators ----------------------------------------------------
		
		/// <summary>
		/// compare with boolean
		/// </summary>
		/// <param name="im">Implicitor</param>
		/// <param name="other">bool value</param>
		/// <returns>true if the implicitor equals supplied bool value</returns>
		public static bool operator == ( Implicitor im, bool other){ return (bool)im == other ; }
		/// <summary>
		/// compare with boolean
		/// </summary>
		/// <param name="im">Implicitor</param>
		/// <param name="other">bool value</param>
		/// <returns>true if the implicitor not equals supplied bool value</returns>
		public static bool operator != ( Implicitor im, bool other){ return (bool)im != other ; }
		
		/// <summary>
		/// compare with string
		/// </summary>
		/// <param name="im">Implicitor</param>
		/// <param name="other">bool value</param>
		/// <returns>true if the implicitor equals supplied string value</returns>
		public static bool operator == ( Implicitor im, string other){ return (string)im == other ; }
		/// <summary>
		/// compare with string
		/// </summary>
		/// <param name="im">Implicitor</param>
		/// <param name="other">bool value</param>
		/// <returns>true if the implicitor not equals supplied string value</returns>
		public static bool operator != ( Implicitor im, string other){ return (string)im != other ; }
		/// <summary>
		/// compare with int
		/// </summary>
		/// <param name="im">Implicitor</param>
		/// <param name="other">bool value</param>
		/// <returns>true if the implicitor equals supplied int value</returns>		
		public static bool operator == ( Implicitor im, int other){ return (int)im == other ; }
		/// <summary>
		/// compare with int
		/// </summary>
		/// <param name="im">Implicitor</param>
		/// <param name="other">bool value</param>
		/// <returns>true if the implicitor not equals supplied int value</returns>		
		public static bool operator != ( Implicitor im, int other){ return (int)im != other ; }
		
		/// <summary>
		/// compare with double
		/// </summary>
		/// <param name="im">Implicitor</param>
		/// <param name="other">bool value</param>
		/// <returns>true if the implicitor equals supplied double value</returns>		
		public static bool operator == ( Implicitor im, double other){ return (double)im == other ; }
		/// <summary>
		/// compare with double
		/// </summary>
		/// <param name="im">Implicitor</param>
		/// <param name="other">bool value</param>
		/// <returns>true if the implicitor not equals supplied double value</returns>		
		public static bool operator != ( Implicitor im, double other){ return (double)im != other ; }
	}
}
