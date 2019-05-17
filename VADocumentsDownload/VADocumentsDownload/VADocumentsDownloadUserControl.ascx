<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VADocumentsDownloadUserControl.ascx.cs" Inherits="VADocumentsDownload.VADocumentsDownload.VADocumentsDownloadUserControl" %>
<!--[if gte mso 9]><xml>
<mso:CustomDocumentProperties>
<mso:_dlc_DocId msdt:dt="string">M5D332FNS7RA-1082079179-81</mso:_dlc_DocId>
<mso:_dlc_DocIdItemGuid msdt:dt="string">0fcc3fbb-3c80-40de-9a86-df3715c21d39</mso:_dlc_DocIdItemGuid>
<mso:_dlc_DocIdUrl msdt:dt="string">http://va.neu.edu.cn/Courses/CollegeEnglish/SmartLL/_layouts/15/DocIdRedir.aspx?ID=M5D332FNS7RA-1082079179-81, M5D332FNS7RA-1082079179-81</mso:_dlc_DocIdUrl>
</mso:CustomDocumentProperties>
</xml><![endif]-->
<link type="text/css" rel="stylesheet" href="/_layouts/15/VADocumentsDownload/css/AlbumCSS.css"/>
<style type="text/css">
    .Appraise {
        box-shadow: 0 1px 3px #ccc;
        background-color: white;
        background-color: rgba(255, 255, 255, 0.6);
        margin-right: 10px;
        margin-top: 5px;
        width: 800px;
    }

        .Appraise .title a.more {
            font-weight: 400;
            font-size: 13px;
            color: #888;
            vertical-align: baseline;
            float: right;
        }

    .grouptitle {
        font-weight: 600;
        font-size: 20px;
        margin-top: 20px;
        margin-left: 10px;
    }

        .grouptitle span {
            font-size: 14px;
            font-weight: normal;
        }

    .mytable {
        border: none;
        color: #666;
        table-layout: fixed;
        empty-cells: show;
        border-collapse: collapse;
        margin: 0 auto;
        margin-left: 10px;
        line-height: 25px;
    }

        .mytable tr {
            border-bottom: 1px dotted #cad9ea;
        }

        .mytable td {
            padding: 0 1em 0;
        }

        .mytable tr.a1 {
            background-color: #f5fafe;
        }

        .mytable tr:hover {
            background-color: #CBE0EC;
            cursor: pointer;
        }
</style>
<script src="/_layouts/15/VADocumentsDownload/js/jquery-1.8.3.min.js"></script>
<script src="/_layouts/15/VADocumentsDownload/js/jquery.tn3lite.min.js"></script>
<script>
$(document).ready(function() {
	var tn1 = $('.mygallery').tn3({
		skinDir:"skins",
		imageClick:"fullscreen",
		image:{
		maxZoom:1.5,
		crop:true,
		clickEvent:"dblclick",
		transitions:[{
		type:"blinds"
		},{
		type:"grid"
		},{
		type:"grid",
		duration:460,
		easing:"easeInQuad",
		gridX:1,
		gridY:8,
		// flat, diagonal, circle, random
		sort:"random",
		sortReverse:false,
		diagonalStart:"bl",
		// fade, scale
		method:"scale",
		partDuration:360,
		partEasing:"easeOutSine",
		partDirection:"left"
		}]
		}
	});
});
</script>
<!-- 脚本代码 结束 -->
<div id="AppraiseDiv" runat ="server">
	<div id="divImgs" runat="server"></div>
	<div id="divContent" runat ="server">
	</div>
	<asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
</div>