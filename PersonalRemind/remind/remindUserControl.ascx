<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="remindUserControl.ascx.cs" Inherits="PersonalRemind.remind.remindUserControl" %>
<style>
    #notified {
        width: 10%;
        min-width: 120px;
        border: 1px solid #D4CD49;
        position: fixed;
        right: 0;
        bottom: 10px;
        z-index: 999;
        padding: 5px;
    }
     #notified b{
            font-weight:700;
            font-size:15px;
        }
        #notified a {
            float: right;
            text-decoration: none;
            color: #777788;
            font-size: 12px;
            padding-right:10px;
        }

            #notified a:hover {
                color: red;
            }
         #notified div {
            padding-right:5px;
            line-height:25px;
        }
         .myupdatetable {
				border: 1px solid #ADADAD;
				width: 100%;
				margin: 0;
				padding: 0;
				border-collapse: collapse;
				border-spacing: 0;
				margin: 0 auto;
			}
			.myupdatetable tr {
				border: 1px solid #E1EDF7;
				padding: 3px;
			}

			.myupdatetable tr:nth-child(even) {
				background-color: #F2E3B6;
			}

			.myupdatetable tr:nth-child(odd) {
				background-color: #E3E3E3;
			}

			.myupdatetable tr:hover {
				background-color: #E1EDF7;
				cursor: pointer;
			}

			.myupdatetable th,
			.myupdatetable td {
				padding: 3px 10px 3px 10px;
				text-align: center;
				border: 1px solid #E1EDF7;
			}

			.myupdatetable th {
				background: #44C7F4;
				font-weight: normal;
				line-height: 22px;
				font-size: 14px;
				color: #FFF;
			}


			@media screen and (max-width: 600px) {
				.myupdatetable {
					border: 0;
				}
				.myupdatetable thead {
					display: none;
				}
				.myupdatetable tr {
					margin-bottom: 5px;
					display: block;
					border-bottom: 2px solid #ddd;
				}
				.myupdatetable td {
					display: block;
					text-align: right;
					font-size: 12px;
					border-bottom: 1px dotted #ccc;
				}
				.myupdatetable td:last-child {
					border-bottom: 0;
				}
				.myupdatetable td:before {
					content: attr(data-label);
					float: left;
					text-transform: uppercase;
					font-weight: bold;
				}
			}

			.note {
				max-width: 80%;
				margin: 0 auto;
			}
</style>
<link rel="stylesheet" type="text/css" href="../_layouts/15/PersonalRemind/css/notified.css" />
<script type="text/javascript" src="../_layouts/15/PersonalRemind/js/notified.js"></script>
<div id="shownotified" runat ="server">
    <!-- 提醒弹出框 -->
		<div id='cs_online'>
			<span class='cs_title'>
				<img style='padding-top:10px;' src='../_layouts/15/PersonalRemind/images/notified.png' width='20'/>
				消息提醒
			</span>
			<ul class='cs_options'>
				<li><img src="../_layouts/15/PersonalRemind/images/update.png"/>&nbsp;&nbsp;更新提醒</li>
				<li><img src="../_layouts/15/PersonalRemind/images/level.png"/>&nbsp;&nbsp;排行提醒</li>
				<li><img src="../_layouts/15/PersonalRemind/images/rank.png"/>&nbsp;&nbsp;趋势提醒</li>
				<li><img src="../_layouts/15/PersonalRemind/images/system.png"/>&nbsp;&nbsp;系统消息</li>
			</ul>
			<div class='cs_context' style='display:block;' id="updateDiv" runat="server">
                暂无
			</div>
			<div class='cs_context' id="levelDiv" runat="server">
				<div id='cs_product'>
					<ul>
						<li>
							3H值
						</li>
						<li>
							作品数
						</li>
						<li>
							知识量
						</li>
						<li>
							互动值
						</li>
						<li>
							专注度
						</li>
						<li>
							积分
						</li>
					</ul>
				</div>
				<div id='cs_product_num'>
					<ul>
						<li>1</li>
						<li>2</li>
						<li>3</li>
						<li>4</li>
						<li>5</li>
						<li>6</li>
					</ul>
				</div>
			</div>
			<div class='cs_context' id="rankDiv" runat="server">
				<div id='cs_cooperation'></div>
			</div>
			<div class='cs_context' id="systemDiv" runat="server">
				<div id='cs_contact'>
					<span class='cs_contact_span'>
						暂无
					</span>
				</div>
			</div>
		</div>
    </div>