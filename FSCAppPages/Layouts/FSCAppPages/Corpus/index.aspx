<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="FSCAppPages.Layouts.FSCAppPages.Corpus.index" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
	<link rel="stylesheet" href="../css/stylelc.css" type="text/css" charset="utf-8" />
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
