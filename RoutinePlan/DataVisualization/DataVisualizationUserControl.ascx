<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataVisualizationUserControl.ascx.cs" Inherits="RoutinePlan.DataVisualization.DataVisualizationUserControl" %>
<%-- CSS --%>
<style type="text/css">
    .Appraise {
    box-shadow: 0 1px 3px #ccc;
    background-color: white;
    background-color: rgba(255, 255, 255, 0.6);
    padding:10px 20px 10px 10px;
    margin-right: 10px;
    margin-top: 5px;
    width:100%;
    max-width:420px;
    line-height:30px;
    font-size:14px;

}
    .Appraise .title {
    height: 50px;
    line-height: 50px;
    font-family: "微软雅黑", "PingFang SC", sans;
    font-weight: 600;
    color: #000;
    font-size: 20px;
    border-bottom: 1px solid #ccc;
}

        .formul{
        list-style:none;
        margin-left:auto;
        }
        .formul li{
        line-height:25px;
        padding-bottom:10px;
        }
        .formul li button{
        font-size:14px;
        }
        .att{
            background-color:#cfcfcf;
            color:red;
            font-size:14px;
        }
.button{
        width: 50px;
        text-align: center;
        font-weight: bold;
        color: #000;
        border-radius: 5px;
        margin:0 5px 5px 0;
        overflow: hidden;
        cursor:pointer;
    }
.mydtc{
    width:100px;
    border:1px solid #ffd800;
    background-color:#cfcfcf;
}

#content{                                /* 具体内容 */
    border-left:1px solid #11a3ff;        /* 左边框 */
    border-right:1px solid #11a3ff;        /* 右边框 */
    border-bottom:1px solid #11a3ff;    /* 下边框 */
    padding:5px;
    font-size:12px;
}
ul#tabnav{
    list-style-type:none;
    margin:0px;
    padding-left:0px;                    /* 左侧无空隙 */
    padding-bottom:23px;
    border-bottom:1px solid #11a3ff;    /* 菜单的下边框 */
    font:bold 12px verdana, arial;
}
ul#tabnav li{
    float:left;
    height:22px;
    background-color:#a3dbff;
    margin:0px 3px 0px 0px;
    border:1px solid #11a3ff;
}
ul#tabnav a:link, ul#tabnav a:visited{
    display:block;                        /* 块元素 */
    color:#006eb3;
    text-decoration:none;
    padding:5px 10px 3px 10px;
}
ul#tabnav a:hover{
    background-color:#006eb3;
    color:#FFFFFF;
}
#home li.home, #dev li.dev,    /* 当前页面的菜单项 */
#design li.design, #map li.map{
    border-bottom:1px solid #FFFFFF;    /* 白色下边框，覆盖<ul>中的蓝色下边框 */
    color:#000000;
    background-color:#FFFFFF;
}
#home li.home a:link, #home li.home a:visited,    /* 当前页面的菜单项的超链接 */
#dev li.dev a:link, #dev li.dev a:visited,
#design li.design a:link, #design li.design a:visited,
#map li.map a:link, #map li.map a:visited{
    color:#000000;
    background-color:#FFFFFF;
}
#home li.home a:hover, #dev li.dev a:hover,
#design li.design a:hover, #map li.map a:hover{
    color:#006eb3;
    text-decoration:underline;
}
     </style>
<%-- js --%>


<%-- 前台html --%>
<ul id="tabnav">
    <li class="home">
        <asp:LinkButton ID="lnkData" runat="server">图形</asp:LinkButton>
    </li>
    <li class="dev">
        <asp:LinkButton ID="lnkImage" runat="server">数据</asp:LinkButton>
    </li>
</ul>
<div id="content">
    <asp:Panel ID="Panel1" runat="server">

    </asp:Panel>
</div>

<br/>
    <div id="charts">

    </div>
<asp:Label ID="lbErr" runat="server" ForeColor="red" Text=""></asp:Label>