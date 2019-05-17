<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrintTableUserControl.ascx.cs" Inherits="LeaveApproval.PrintTable.PrintTableUserControl" %>
<style>
.leavetable {
    border: 1pt solid black;
    border-collapse: collapse;
}

.leavetable td {
    border: 1pt solid black;
    font-size: 14px;
    padding: 5px;
    text-align: center;
    line-height: 20px;
}
</style>
<link rel="stylesheet" type="text/css" href="../../../../_layouts/15/LeaveApproval/css/LeaveListCSS.css">
<!--打印-->
<div style="text-align:center"><asp:Button ID="btnPrint" runat="server" Text="查看请假审批表" OnClick="btnPrint_Click" /></div>
<div id="printContent">
    <div id="printDiv" runat="server"  style='text-align:center;padding:5px;'>
  
    </div>
</div>
<div id="printBtn" runat="server" visible="false" style="text-align:center">
    <input name="a_print" type="button" class="ipt" onClick="printdiv('printContent');" value="打 印">
    <script type="text/javascript">
        function printdiv(printpage) {
            var headstr = "<html><head><title>请假审批表</title><link rel='stylesheet' type='text/css' href='../../../../_layouts/15/LeaveApproval/css/LeaveListCSS.css'></head><body>";
            var footstr = "</body>";
            var newstr = document.all.item(printpage).innerHTML;
            var oldstr = document.body.innerHTML;
            document.body.innerHTML = headstr + newstr + footstr;
            window.print();
            document.body.innerHTML = oldstr;
            return false;
        }
    </script>
</div>