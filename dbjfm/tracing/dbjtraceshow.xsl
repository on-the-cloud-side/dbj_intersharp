<?xml version="1.0" encoding="UTF-8" ?>
<!--
		(c) 2001-2005 by DBJ*Solutions ltd.
		Default XSL file to displaying DBJ*Corelib, XML Trace files
		
		Last edited by : $Author: Admin $
		$JustDate: 27/02/05 $
		$Revision: 1 $
-->

<!-- xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" -->
<!-- xsl:output method="html" indent="yes" / --> 
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/TR/WD-xsl" >
 <xsl:script>
//------------------------------------------------------------- 
// Return human readable date/time string from an ISO timestamp
//            01234567890123 : positions for substring()
// ISO TS is: 20050225131415
// NOTE: ISO months are starting from 1 for Janury, while
// javascript Date object months are starting from 0, thus
// the '-1' on the substring(4,6) call bellow
function format_iso_date( xmlnode, time, isodatestamp )
{
	try {
		var att = xmlnode.attributes.getNamedItem("timestamp") ;
		if (att == null) 
			throw new Error(0xFF, "'timestamp' attribute not found.") ;
		isodatestamp = att.value ;
	var jsdate = new Date( 
		isodatestamp.substring(0,4) ,
		isodatestamp.substring(4,6) - 1 ,
		isodatestamp.substring(6,8) ,
		isodatestamp.substring(8,10) ,
		isodatestamp.substring(10,12) ,
		isodatestamp.substring(12,14) 
	) ;
		if ( time != null )
			return jsdate.toLocaleTimeString() ;
		return jsdate.toLocaleString() ;	
	} catch (x) {
		return "ERROR in format_iso_date():" + x.message ;
	}
}
</xsl:script>
<xsl:template match="/">
<html>
	<head>
		<title>DBJ*Corelib Trace view</title>
		<style>
			body { font-family:Verdana,tahoma,arial;margin:2;padding:2;	color:windowtext;background-color:window ; }
			td   { border-top:2px solid black; }
		</style>		
	</head>
	<body>		
		<a href="javascript:history.back()" style="font-family:Webdings" title="Click to view other trace files">7</a>	
		<table border="0"   >
			<caption align="left">
				Trace Details
			</caption>
			<xsl:apply-templates select="trace" />
		</table>
		<p style="font-size:xx-small;" align="right">DBJ*Corelib <i>.NET Core Library</i> (c) 2003-2005 by <a href="mailto:dbj@dbj.org">DBJSolutions Ltd.</a></p>
	</body>
</html>
</xsl:template>
<xsl:template match="trace">
	<tr style="font-size:xx-small;" border="1" >	
		<td colspan="4" align="left" >			
			Trace Start :	<xsl:eval>format_iso_date(this)</xsl:eval>
		</td>
	</tr>	
	<tr border="1" style="font-size:xx-small;" >
		<td colspan="4" align="left" >			
			Assembly Traced :	<xsl:value-of select="@assembly" />			
		</td>
	</tr>	
	<tr style="font-size:xx-small;">
		<td align="left" colspan="4" >
				Tracing Level: <xsl:value-of select="switch/@level" />						
		</td>
	</tr>
	<tr height="10">
		<td align="left" colspan="4" >
		</td>
	</tr>
	<xsl:for-each select="line"  >
		<tr style="font-size:xx-small;">
		</tr>
		<tr style="font-size:xx-small;">
			<td align="left" valign="top" width="5%" style="background-color:silver;border-right:2px solid black;" >
			<div align="left" colspan="4"  style="font-size:xx-small;font-family:courier new;background-color:transparent;">
				<xsl:eval>format_iso_date(this, 1)</xsl:eval>
			</div>		
			</td>	
			<td align="left" STYLE="lineBreak:strict;word-break:break-all;  " >
				<xsl:value-of select="." />						
			</td>
		</tr>
</xsl:for-each>
</xsl:template>
</xsl:stylesheet>