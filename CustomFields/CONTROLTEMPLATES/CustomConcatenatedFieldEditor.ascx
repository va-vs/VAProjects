<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/15/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/15/InputFormSection.ascx" %>
<%@ Control Language="C#" AutoEventWireup="false" CompilationMode="Always"  Inherits="CustomFields.CustomConcatenatedFieldEditor" %>
<wssuc:InputFormSection runat="server" id="RFilterLookupField" Title="">
  <template_inputformcontrols>
  <table style ="border-spacing:0px 10px" border="0">
    <tr>
        <td>
            <b>信息来源：</b>
        </td>
        <td>
         
            <asp:DropDownList ID="ddlListName" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>
            <b>此栏包含：</b>
        </td>
        <td>
         
            <asp:DropDownList ID="ddlFieldName" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
      <tr>
        <td>
            <b>控件宽度：</b>
        </td>
        <td>
         <asp:TextBox runat ="server" ID ="txtWidth"></asp:TextBox>
            
        </td>
    </tr>
</table>
    </template_inputformcontrols>
</wssuc:InputFormSection>