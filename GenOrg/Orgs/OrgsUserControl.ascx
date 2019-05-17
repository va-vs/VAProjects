<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrgsUserControl.ascx.cs" Inherits="GenOrg.Orgs.OrgsUserControl" %>
<%@ Register TagPrefix="oc" Namespace="Whidsoft.WebControls" Assembly="Whidsoft.WebControls.OrgChart" %>


<style type="text/css">
        .alink a,a:hover,a:visited{
             color:Black;
              text-decoration:none;
        }
    </style>
<div class="alink">
   <oc:orgchart id="OrgChart1" style="z-index: 101; left: 104px; position: absolute; top: 88px" runat="server" LineColor="Silver" Width="100%" Height="100%" ChartStyle="Vertical" ToolTip="test"></oc:orgchart>
</div>