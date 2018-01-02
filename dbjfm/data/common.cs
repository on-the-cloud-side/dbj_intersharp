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

//--------------------------------------------------------------------
//
//  The whole data part of this library was designed and developed 2002-2003
//  by :
//  
//  Mr. Dusan B. Jovanovic ( dbj@dbj.org )
//
//  Author donates this code to Clear-Technology Inc. (Denver,USA) 
//  Author reserves intelectual copyrights on this code.
//
//--------------------------------------------------------------------
namespace dbj.fm.data
{
	using System;
	using System.Text ;
	using System.Data ;
	using System.Runtime.InteropServices ;

	/// <summary>
	///  This kind of delegate is required to use for_each_row() methods. 
	/// </summary>
 	public delegate void visitor ( System.Object obj ) ;
	
	/// <summary>
	/// The common base class for all implementations of data providers
	/// </summary>
	abstract public class dbjsql : fm.Idata 
	{
		private string connection_string_ ; 

		/// <summary>
		/// This contructor is visible only to inheritors.
		/// </summary>
		protected dbjsql() {}

		/// <summary>
		/// Constructor of dbjsql
		/// </summary>
		/// <param name="connection_string">connection string</param>
		public dbjsql( string connection_string )
		{
			this.connection_string_ = connection_string ;
		}
		/// <summary>
		/// checks if connection string is null
		/// </summary>
		protected void check()
		{
			util.ASSERT(  null !=  this.connection_string_ ) ;
			//if(this.connection_string_ == null)
			//	throw new System.Exception("Connection string is null") ;
		}
		/// <summary>
		/// gets or sets connection string
		/// </summary>	
		public string connection_string
		{
			get
			{
				return this.connection_string_;
			}
			set
			{
				this.connection_string_ = value ;
			}
		}

		
	}
}
