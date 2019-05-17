<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustUploadImage.aspx.cs" Inherits="NewsApproval.Layouts.NewsApproval.CustUploadImage" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

   
    <basetarget="_self">

   <script  type="text/javascript" >
    function ReturnPageValue(imgurl, des)
    {
        alert("ik");
        var array = new Array();
        array[0] = imgurl;
        array[1] = des;
        window.returnValue = array;
        window.opener = null;
        window.close();
    }
   </script>
    <style type="text/css">
   
        .TdNowrap
   
        {
   
            white-space: nowrap;
   
            vertical-align: top;
   
        }
   
    </style>
   
</asp:Content>
   
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
   
    <div style="">
   
        <table style="margin: auto;width: 250px;">
   
            <tr class="TdNowrap">
   
                <td>
   
                    请选择你要上传的图片
   
                </td>
   
            </tr>
   
            <tr>
   
                <td class="TdNowrap">
   
                    文字描述：<input id="txtDes" type="text" runat="server" />
   
                </td>
   
            </tr>
   
            <tr>
   
                <td class="TdNowrap">
   
                    <asp:FileUpload ID="flUpload" runat="server"  />
   
                </td>
   
            </tr>
   
            <tr>
   
                <td class="TdNowrap" align="center">
   
                    <asp:Button ID="BtnSubmit" runat="server" Text="提交" OnClick="btnSave_Click" />
   
                    <input id="BtnClose" type="button" value="关闭" onclick="window.close();" />
   
                </td>
   
            </tr>
   
        </table>
   
    </div>
   
</asp:Content>