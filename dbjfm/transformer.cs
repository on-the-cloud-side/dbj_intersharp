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
	/// <summary>
	/// Empty implementation of the Itransformer interface.
	/// </summary>
	public class transformer : Itransformer
	{
		/// <summary>
		/// constructor of transformer
		/// </summary>
		public transformer()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		/// <summary>
		/// transforms request xml
		/// </summary>
		/// <param name="xml_request"></param>
		/// <returns>Transformed xml</returns>
		override public string transform_for_request(string xml_request )
		{
			throw new NotImplementedException() ;
		}
	/// <summary>
	/// transforms reply xml
	/// </summary>
	/// <param name="xml_reply"></param>
	/// <returns>Transformed xml</returns>
		override public string transform_for_reply(string xml_reply)
		{
			throw new NotImplementedException() ;
		}
	}
}
