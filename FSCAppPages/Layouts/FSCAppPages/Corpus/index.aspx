<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="FSCAppPages.Layouts.FSCAppPages.Corpus.index" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
	<%--<link rel="stylesheet" href="../css/stylelc.css" type="text/css" charset="utf-8" />--%>

	<style type="text/css">
			.div{
		margin:0 auto;
			width:100%;
			max-width:750px;
			text-align:center;
			vertical-align:middle;
			font-size:1.2em;
			padding-top:100px;
		}
		.div table{
			width:100%;
		}
		.div table a{
			color:#000;
			border:1px solid #808080;
			padding:5px;
		}
		.div table a:hover{
			color:#ff6a00;
			cursor:pointer;
		}
		.div table a:visited{
			color:#000;
		}
		/* 底部快速链接与版权 */

.foot {
    margin:0 auto;
	width: 100%;
	margin-top: -20px;
	/* margin-bottom: -40px; */
	background: #ecf7fb;
	border-top: #efefef 1px solid;
}

.ext_link {
	width: auto;
	height: 12px;
	margin: 0 auto;
	text-align: center;
	max-width: 1000px;
}

.ext_link li {
	list-style: none;
	width: auto;
	display: inline-block;
	line-height: 20px;
	text-align: center;
	border-left: #000 1px solid;
	border-color: #525252;
	font-size: 14px;
	padding-left: 10px;
	padding-right: 10px;
}

.ext_link li:first-child {
	border-left: 0px;
}

.ext_link a {
	color: #525252;
	text-decoration: none;
}

.ext_link a:hover {
	color: red;
	text-decoration: none;
}
	</style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

	<div class="div">
		<h3>NEU English Corpus</h3>
		<p></p>
		<table>
			<tr>
				<td>
					<a href="neulc.aspx" title="NEULC">NEULC</a>
				</td>
				<td>
					<a href="neuac.aspx" title="NEUAC" style="pointer-events:none;color:#afafaf;cursor:default;">NEUAC</a>
				</td>
			</tr>
		</table>
		  <hr style="margin-top:100px;"/>
		  <ul class="ext_link">
			<li><a href="https://www.english-corpora.org/bnc/" target="_blank">BNC</a></li>
			<li><a href="https://www.english-corpora.org/coca/" target="_blank">COCA</a></li>
			<li><a href="http://corpus.leeds.ac.uk/itweb/htdocs/Query.html#" target="_blank">IntelliText</a></li>
			<li><a href="https://www.lextutor.ca/vp/eng/" target="_blank">Lextutor</a></li>
			<li><a href="http://www.natcorp.ox.ac.uk/corpus/creating.xml" target="_blank">Natcorp</a></li>
		  </ul>
	</div>
	<asp:Label ID="lbErr" runat="server" Text="" ForeColor="red"></asp:Label>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
NEU English Corpus
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
NEU English Corpus
</asp:Content>
