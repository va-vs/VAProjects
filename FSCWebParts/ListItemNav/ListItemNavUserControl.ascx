<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListItemNavUserControl.ascx.cs" Inherits="FSCWebParts.ListItemNav.ListItemNavUserControl" %>
<meta name="viewport" content="width=device-width,initial-scale=1.0,minimum-scale=1.0,maximum-scale=1.0,user-scalable=no">
<meta name="apple-mobile-web-app-capable" content="yes" />
<meta name="apple-mobile-web-app-status-bar-style" content="black-translucent" />
<meta name="format-detection" content="telephone=yes"/>
<meta name="msapplication-tap-highlight" content="no" />

<style type="text/css">
.wrapper{
    width: 98%;
    margin: 0;
    padding:5px;
}
.wrapper ul
{
    list-style-type: none;
    padding: 0px;
    margin: 0px;
}
.wrapper ul li
{
    padding-left: 10px;
    padding-bottom:5px;
}
.wrapper ul li a{
    color: #0072c6;
    text-decoration:none;
    font-weight:600;
}
.wrapper ul li a:visited{
    color: #0072c6;
    text-decoration: none;
    font-weight:600;
}
.wrapper ul li a:hover{
    color: #c00;
    text-decoration: none;
    font-weight:600;
}

</style>
<div id="itemNav" runat="server" class="wrapper">
    <hr />
    <ul>
        <li>
            <a href="#">上一条</a>
        </li>
        <li>
            共 N 条
        </li>
        <li>
            <a href="#">下一条</a>
        </li>
    </ul>
</div>    