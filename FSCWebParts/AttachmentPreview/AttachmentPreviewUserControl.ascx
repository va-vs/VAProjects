<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttachmentPreviewUserControl.ascx.cs" Inherits="FSCWebParts.AttachmentPreview.AttachmentPreviewUserControl" %>

    <asp:Label ID="lbAttach" runat="server" Font-Bold="true" Font-Size="16" Text="附件浏览"></asp:Label>
    <div id="divAttach" runat="server" style="width:620px;height:500px;overflow-y:auto;">
    <dl>
        <dt><span style="padding-left:10px">附件1：</span>
            <br/>
            <iframe src='' width="600" height="450" ></iframe>
        </dt>
        <dt><span>附件2：</span>
            <br/>
            <img src="" width="600" height="450"/>
        </dt>
    </dl>

</div>

    <asp:Label ID="lbErr" runat="server" Text="" ForeColor="Red"></asp:Label>