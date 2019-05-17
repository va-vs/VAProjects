<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewsApprovalUserControl.ascx.cs" Inherits="NewsApproval.NewsApproval.NewsApprovalUserControl" %>
<script type="text/javascript" >
    window.onload=function(){
        var name = "<%= txt1Body.ClientID %>";

        (document.getElementById(name + "_toolbar")).style.width = "600px";
        (document.getElementById(name + "_toolbar")).style.height  = "50px";
        (document.getElementById(name + "_iframe")).style.width = "600px";
    }
</script>
<script>

    //function RTE_ModalDialog( strBaseElementID,strDialogName,width, height,dialogArg) {

    //    ULSopi:;

    //    var variables = RTE_GetEditorInstanceVariables(strBaseElementID);

    //    if (strDialogName == "InsertImage") {

    //        return showModalDialog(

    //                variables.aSettings.urlWebRoot + "/_layouts/15/NewsApproval/CustUploadImage.aspx?IsDlg=1&Dialog=" + strDialogName + "&LCID=" + RTE_GetWebLocale(strBaseElementID) + "&IsDlg=1",

    //                dialogArg,

    //                "resizable: no; status: no; help: no; " + "scroll:no;center: yes; dialogWidth:" + width + "px; " + "dialogHeight:" + height + "px;");

    //    }

    //    else {

    //        return showModalDialog(

    //                variables.aSettings.urlWebRoot + "/_layouts/15/RteDialog.aspx?Dialog=" + strDialogName + "&LCID=" + RTE_GetWebLocale(strBaseElementID),

    //                dialogArg,

    //                "resizable: yes; status: no; help: no; " + "center: yes; dialogWidth:" + width + "px; " + "dialogHeight:" + height + "px;");

    //    }

    //}

    </script>
<label id="errShow" runat ="server"></label>
<asp:Table ID="Table1" runat="server" CellSpacing="3" CellPadding="3" HorizontalAlign="Left">
   
    <asp:TableRow>
        <asp:TableCell Width="200px">标 题:</asp:TableCell>
        <asp:TableCell><asp:TextBox ID="txtTitle" runat="server" Width="600px"></asp:TextBox></asp:TableCell>
    </asp:TableRow>    
    <asp:TableRow>
        <asp:TableCell Width="200px" VerticalAlign="Top">正 文:</asp:TableCell>
        <asp:TableCell><SharePoint:InputFormTextBox  ID="txt1Body" Runat="server" TextMode="MultiLine"  Rows="15" RichText="true" AllowHyperlink="true" RichTextMode="FullHtml" Width="600px" Height="400px" /></asp:TableCell></asp:TableRow><asp:TableRow>
        <asp:TableCell>是否主页显示</asp:TableCell><asp:TableCell><asp:CheckBox ID="chkIsPage" runat="server" /></asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>是否图片要闻</asp:TableCell><asp:TableCell><asp:CheckBox ID="chkIsPic" runat="server" /></asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>新闻类别</asp:TableCell><asp:TableCell><asp:DropDownList ID="ddlNewsType" runat="server"></asp:DropDownList></asp:TableCell></asp:TableRow><asp:TableRow>
        <asp:TableCell>审批意见</asp:TableCell><asp:TableCell><asp:TextBox ID="txtModerationComments" runat="server" Width="600px" Height="60px"></asp:TextBox></asp:TableCell></asp:TableRow><asp:TableRow>
        <asp:TableCell>&nbsp;</asp:TableCell><asp:TableCell><asp:Button ID="btnReject" runat="server" Text="拒绝"  OnClick ="btnReject_Click"/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnPiZhu" runat="server" Text="同意并保存" OnClick ="btnPiZhu_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnClose" runat="server" Text="关闭" OnClick ="btnClose_Click" /></asp:TableCell>
    </asp:TableRow>
</asp:Table>
        
   
