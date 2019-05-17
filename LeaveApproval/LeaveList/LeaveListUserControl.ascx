<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeaveListUserControl.ascx.cs" Inherits="LeaveApproval.LeaveList.LeaveListUserControl" %>
<script type="text/javascript" src="../../../../_layouts/15/LeaveApproval/js/jquery-1.4.1.min.js"></script>
<script type="text/javascript" src="../../../../_layouts/15/LeaveApproval/js/pagination.js"></script>
<script type="text/javascript" src="../../../../_layouts/15/LeaveApproval/js/jquery-1.11.1.min.js"></script>
<link rel="stylesheet" type="text/css" href="../../../../_layouts/15/LeaveApproval/css/LeaveListCSS.css">
<script type="text/javascript">
    var numCount; //数据总数量
    var columnsCounts; //数据列数量
    var pageCount; //每页显示的数量
    var pageNum; //总页数
    var currPageNum; //当前页数

    //页面标签变量
    var blockTable;
    var preSpan;
    var firstSpan;
    var nextSpan;
    var lastSpan;
    var pageNumSpan;
    var currPageSpan;
    window.onload = function() {
        //页面标签变量
        blockTable = document.getElementById("ListArea");
        preSpan = document.getElementById("spanPre");
        firstSpan = document.getElementById("spanFirst");
        nextSpan = document.getElementById("spanNext");
        lastSpan = document.getElementById("spanLast");
        pageNumSpan = document.getElementById("spanTotalPage");
        currPageSpan = document.getElementById("spanPageNum");
        totalSpan = document.getElementById("spanTotal");
        numDiv = document.getElementById("divnum");

        numCount = document.getElementById("ListArea").rows.length - 1; //取table的行数作为数据总数量（减去标题行1）
        
        pageCount = 8;
        //每页8条
        totalSpan.textContent = "共 " + numCount + " 条  每页 " + pageCount + " 条";        
        if (numCount <= pageCount) {
            numDiv.innerText = "共 " + numCount + " 条";
            numDiv.style.display = "block";
            document.getElementById("pagiDiv").style.display = "none";
        } else {            
            pageNum = parseInt(numCount / pageCount);
            if (0 != numCount % pageCount) {
                pageNum += 1;
            }            
            firstPage();
        }

    };
</script>
<script type="text/javascript">
    $(document).ready(function() { //这个就是传说的ready  
        $(".t1 tr").mouseover(function() {
                //如果鼠标移到class为stripe的表格的tr上时，执行函数  
                $(this).addClass("over");
            }).mouseout(function() {
                //给这行添加class值为over，并且当鼠标一出该行时执行函数  
                $(this).removeClass("over");
            }) //移除该行的class  
        $(".t1 tr:even").addClass("alt");
        //给class为stripe的表格的偶数行添加class值为alt
    });
</script>
<div style="padding:5px;" id="lvList" runat="server">
    <div id="wbtitle" runat="server" visible="false" style="font-size:20px;font-weight:700;line-height:50px;">待批复请假列表</div>
    <div id="divData" runat="server" style="padding-bottom:5px;line-height:25px;"></div>
    <div id="pagediv" runat="server" visible="false">
        <table style="font-size:14px;color:#000;width:100%;padding:8px;height:30px;background: #ebebeb; margin-right: 5px;">
            <tr>
                <td align="left">
                    <div id="chtml" runat="server" style="padding-left:10px;font-weight:bold;"></div>
                </td>
                <td align="right">
                    <div id="divnum" style="padding-right:20px;"></div>
                    <div id="pagiDiv" align="right" style="margin-right: 5px;">
                        <span id="spanTotal"></span>&nbsp;
                        <span id="spanFirst"><<</span>&nbsp;&nbsp;
                        <span id="spanPre"><</span>&nbsp;&nbsp;
                        <span id="spanNext">></span>&nbsp;&nbsp;
                        <span id="spanLast">>></span>&nbsp;&nbsp;第
                        <span id="spanPageNum"></span>&nbsp;页/共&nbsp;
                        <span id="spanTotalPage"></span>页&nbsp;&nbsp;
                    </div>
                </td>
            </tr>
        </table>
    </div>
</div>