<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppraiseUserControl.ascx.cs" Inherits="CreativeAppraise.Appraise.AppraiseUserControl" %>
<script type="text/javascript">
    $(function () {
        H_qqServer = {};
        H_qqServer.clickOpenServer = function () {
            $('.qq-client-open').click(function () {
                $('.qq-client').animate({
                    right: '-50'
                }, 400);
                $('.qq-client-content').animate({
                    right: '0',
                    opacity: 'show'
                }, 800);
            });
            $('.qq-client-close').click(function () {
                $('.qq-client').animate({
                    right: '0',
                    opacity: 'show'
                }, 400);
                $('.qq-client-content').animate({
                    right: '-250',
                    opacity: 'show'
                }, 800);
            });
        };
        H_qqServer.run = function () {
            this.clickOpenServer();
        };
        H_qqServer.run();
    });
</script>

<style type="text/css">
    .Appraise {
    box-shadow: 0 1px 3px #ccc;
    background-color: white;
    background-color: rgba(255, 255, 255, 0.6);
    padding: 0 20px;
    margin-right: 10px;
    margin-top: 5px;
    width:400px;
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
    .Appraise .title a.more {
    font-weight: 400;
    font-size: 12px;
    color: #888;
    vertical-align: baseline;
    float: right;
}

       .FormList {
    }
        .showc{

            }
        .formul{
        list-style:none;
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
        /* qq-client */
/*.qq-client{position:fixed;right:0px;top:50%;margin-top:-80px;}
.qq-client a{width:50px;height:160px;text-align:center;border:#ebebeb solid 1px;padding:0px 0px;line-height:40px;display:block;}
.qq-client-content{position:fixed;right:-250px;top:38%;border:#ebebeb solid 1px;width:200px;display:none;background:#ffffff;}
.qq-client-content h1{font-size:14px;width:90%;margin:0px auto;text-align:center;height:50px;line-height:50px;border-bottom:#ebebeb solid 1px;position:relative;}
.qq-client-content h1 span{font-size:12px;font-weight:normal;position:absolute;left:-30px;top:-20px;cursor:pointer;background:#ffffff;border:#ebebeb solid 1px;width:40px;height:40px;line-height:40px;border-radius:20px;text-align:center;}
.qq-client-list{background:#ffffff;}
.client-list{overflow:hidden;line-height:40px;width:90%;margin:0px auto;border-bottom:dashed 1px #ebebeb;}
.client-list a{display:block;}
.client-list a:hover{color:red;}
.client-list span{float:left;}
.client-list label{float:left;width:60px;text-align:right;padding-right:10px;}*/
    </style>
<script type="text/javascript" src="/_layouts/15/js/jquery-1.11.2.min.js"></script>
<%--<div class="qq-client">
        <a href="javascript:void(0);" class="qq-client-open">创<br/>意<br/>评<br/>审</a>
    </div>--%>
<%--<div class="qq-client-content">--%>

<div id="AppraiseDiv" class="Appraise" runat ="server">
    <div class="title">
        <asp:Label ID="lbAppraise" runat="server" Text="专家评审"></asp:Label>
        <%--<h1><span class="qq-client-close">关闭</span></h1>--%>
    </div>
    <div id="AppAction" runat="server" visible="true">
        <ul class="formul">
            <li>
                <asp:Label ID="lbContentNum" runat="server" Text="创意字数:N" Font-Size="16px" Font-Bold="true"></asp:Label>
            </li>
            <li>
                <asp:Label ID="lbScore" runat="server" Text="评 分:" Font-Size="16px" Font-Bold="true"></asp:Label>
                &nbsp;
                <asp:Label ID="lbAScore" runat="server" Text="评分不能为空,且必须是数字" ForeColor="Red" Font-Size="14px" Visible="false"></asp:Label><br/>
                <asp:TextBox ID="txtScore" runat="server" TextMode="SingleLine"  CssClass="FormList" ToolTip="请输入0-N之间的数字" Height="22px" Width="300px" ></asp:TextBox>
                <br/>
                <div style="color:#a0a0a0">评分必填,<%=webObj.Score %>分制评分,请输入0-<%=webObj.Score %>之间的数字</div>
            </li>
            <li>
                <asp:Label ID="lbComments" runat="server" Text="评 语:" Font-Size="16px" Font-Bold="true"></asp:Label>
                &nbsp;
                <asp:Label ID="lbACommnts" runat="server" Text="评语不得小于N字!" ForeColor="Red" Font-Size="14px" Visible="false"></asp:Label><br/>
                <asp:TextBox ID="txtAppraise"    runat="server" TextMode="MultiLine" CssClass="FormList" MaxLength="100" Rows="5" ToolTip="请输入不少于N字的评语"  Height="100px" Width="300px"></asp:TextBox>
                <br/>
                <div style="color:#a0a0a0">评语必填,内容不少于<%=webObj.AppraiseNum %>字,主要针对创意的有效性,科学性以及解决问题的思想性,创新性</div>
            
            </li>
            <li style="padding-left:22%;">
                <asp:Button ID="btnAppraise" runat="server" Text="提 交" />&nbsp;&nbsp;
                <asp:Button ID="btnCancle" runat="server" Text="返 回" />
            </li>
            <li>
                <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </li>
           

        </ul>
    </div>
</div>
<%--</div>--%>
