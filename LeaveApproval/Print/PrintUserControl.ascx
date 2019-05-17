<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrintUserControl.ascx.cs" Inherits="LeaveApproval.Print.PrintUserControl" %>
    <script src="../../../../_layouts/15/LeaveApproval/js/jquery-1.6.2.min.js" type="text/javascript"></script>
    <style>
        .black_overlay {
            display: none;
            position: absolute;
            top: 0%;
            left: 0%;
            width: 100%;
            height: 100%;
            background-color: black;
            z-index: 1001;
            -moz-opacity: 0.8;
            opacity: .80;
            filter: alpha(opacity=80);
        }
        
        .white_content {
            display: none;
            position: absolute;
            top: 5%;
            left: 10%;
            width: 80%;
            height: 90%;
            border: 16px solid lightblue;
            background-color: white;
            z-index: 1002;
            overflow: auto;
        }
        
        .white_content_small {
            display: none;
            position: absolute;
            top: 20%;
            left: 30%;
            width: 40%;
            height: 50%;
            border: 16px solid lightblue;
            background-color: white;
            z-index: 1002;
            overflow: auto;
        }
        .mybtn{
            cursor:pointer;
        }
        .mybtn:hover{
            cursor:pointer;
            background-color:red;
        }
    </style>
    <script type="text/javascript">
        //弹出隐藏层
        function ShowDiv(show_div, bg_div) {
            document.getElementById(show_div).style.display = 'block';
            document.getElementById(bg_div).style.display = 'block';
            var bgdiv = document.getElementById(bg_div);
            bgdiv.style.width = document.body.scrollWidth;  // bgdiv.style.height = $(document).height();

            $("#" + bg_div).height($(document).height());
        };
        //关闭弹出层
        function CloseDiv(show_div, bg_div) {
            document.getElementById(show_div).style.display = 'none';
            document.getElementById(bg_div).style.display = 'none';
        };
    </script>
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
<div style="text-align:center" runat="server" visible="false" id="viewdiv"> 
    <input id="btnView" type="button" value="预览审批表" onclick="ShowDiv('MyDiv', 'fade')" class="mybtn" />

    <!--弹出层时背景层DIV-->
    <div id="fade" class="black_overlay">
    </div>
    <!-- 弹出层内容区域 -->
    <div id="MyDiv" class="white_content">
        <!-- 打印内容div -->
        <div id="printContent">
            <div id="printDiv" runat="server"  style='text-align:center;padding:5px;'>  
            </div>
        </div>

        <!-- 打印操作按钮区域 -->
        <div id="printBtn" runat="server" visible="false" style="text-align:center">
            <input name="a_print" type="button" class="mybtn" onClick="printdiv('printContent');" value="打 印">
            <input id="btnClose" type="button" class="mybtn" value="关 闭"  onclick="CloseDiv('MyDiv', 'fade')"/>
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
    </div>
</div>