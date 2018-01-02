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

#if INCLUDE_DEPRECATED
using System;

namespace dbj.fm
{
	/// <summary>
	/// Summary description for error_handler.
	/// </summary>
	internal class error_handler : Ierror_handler 
	{
		internal error_handler()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	
		override public string make_tranzax_error_node(string foreign_xml_error)
		{
			// TODO:  Add error_handler.make_tranzax_error_node implementation
			return null;
		}
	
		override public int tranzax_system_error(Exception x)
		{
			// TODO:  Add error_handler.tranzax_system_error implementation
			return 0;
		}
	}
}
#endif