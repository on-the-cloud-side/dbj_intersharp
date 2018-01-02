<?xml version="1.0" encoding="UTF-8" ?>
<!--
		Last edited by : $Author: dusan $
		$JustDate: 30.06.06 $
		$Revision: 1 $
-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html" indent="yes" /> 
<xsl:template match="/">
<html>
	<head>
		<title>Result of DBJ*EPT Calling the Extension point</title>
	</head>
	<body>
		<style>
			body  { font-family:verdana,tahom,arial; font-size:x-small; margin:1px; padding 1px; }
			TABLE { table-layout:fixed ; width:100% ; align:center ;border:0 }
			TR    { background-color:gainsboro  }
			TD    { word-break :break-all }
		</style>
		<xsl:apply-templates/>
		<p style="font-size:xx-small;" align="right">DBJ*EPT <i>End Point Tester</i> (c) 2003-2005 by <a href="mailto:dbj@dbj.org">DBJSolutions Ltd.</a></p>
	</body>
</html>
</xsl:template>
<xsl:template match="test">
	<table >
		<tr >
			<td width="20%"><B>Date</B></td>
			<td width="1%"><B>:</B></td>
			<td width="79%"><xsl:value-of select="date" /></td>
		</tr>
		<tr >
			<td width="20%"><B>Time</B></td>
			<td width="1%"><B>:</B></td>
			<td width="79%"><xsl:value-of select="time" /></td>
		</tr>
		<tr>
			<td width="20%"><B>Progid</B></td>
			<td width="1%"><B>:</B></td>
			<td width="79%"><Font STYLE="background-color:windowtext;color:window;font-weight:bold"><xsl:value-of select="progid" /></Font></td>
		</tr>
		<tr>
			<td width="20%"><B>Request</B></td>
			<td width="1%"><B>:</B></td>
			<td width="79%"><xsl:value-of select="request" /></td>
		</tr>
		<tr>
			<td width="20%"><B>Number Of Calls</B></td>
			<td width="1%"><B>:</B></td>
			<td width="79%"><xsl:value-of select="no_of_calls" /></td>
		</tr>
		<tr>
			<td width="20%"><B>Call Delay Time</B></td>
			<td width="1%"><B>:</B></td>
			<td width="79%"><xsl:value-of select="call_delay" /></td>
		</tr>
		<tr>
			<td width="20%"><B>Machine Name</B></td>
			<td width="1%"><B>:</B></td>
			<td width="79%"><xsl:value-of select="machine_name" /></td>
		</tr>
	</table>
	<xsl:apply-templates select="call" />
	<table>
		<tr>
			<td width="20%"><B>Total Elapsed Time</B></td>
			<td width="1%"><B>:</B></td>
			<td width="79%"><xsl:value-of select="elapsed" /></td>
		</tr>	
	</table>
</xsl:template>
<xsl:template match="call">	
<table>
	<!--<tr border="1">
		<td colspan="2" align="center">-->
	<caption>		
		<B>
		<Font STYLE="background-color:window;font-weight:bold">Call Number :</Font>
		<xsl:value-of select="@count" />
		</B>
	</caption>	
	<!--	</td>
	</tr>	-->
	<tr>
		<td width="20%">
			<B>Result</B>
		</td>
		<td width="1%"><B>:</B></td>
		<td width="79%" STYLE="font-size:xx-small">
			<xsl:value-of select="result" />
		</td>
	</tr>
	<tr>
		<td width="20%">
			<B>Elapsed Time</B>
		</td>
		<td width="1%"><B>:</B></td>
		<td width="79%">
		    <xsl:value-of select="elapsed" />
		</td>
	</tr>
</table>
</xsl:template>
</xsl:stylesheet>


  