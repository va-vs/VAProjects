<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Neuec_IndexUserControl.ascx.cs" Inherits="FSCWebParts.Neuec_Index.Neuec_IndexUserControl" %>
<script type="text/javascript">
    function showt(obj) {
        var id = obj.getAttribute("data-id");
        var divObj = document.getElementById(id);
        divObj.style.display = "";
    }

    function hiddThis(obj) {
        var divObj = document.getElementById(obj);
        divObj.style.display = "none";
    }
</script>
<style type="text/css">


    .divMain {
        position: absolute;
        width: 100%;
        margin-left: 180px;
        margin-top: -60px;
    }

    .ma {
        position: absolute;
        top: 45px;
        left: 45px;
        padding: 5px;
        width: 850px;
        height: 420px;
        background-image: url('http://202.118.11.33/NEU_EC/PublishingImages/images/info.png');
        background-repeat: no-repeat;
    }

    .ma0 {
        margin-top: 5px;
    }

        .ma0 table {
            width: 100%;
        }

            .ma0 table th {
                width: 96%;
                text-align: center;
                font-weight: bold;
                font-size: 1.2em;
            }

                .ma0 table th a {
                    text-align: center;
                    font-weight: bold;
                    text-decoration: underline;
                    color: #0066CC;
                }

                    .ma0 table th a:visited {
                        font-weight: bold;
                        text-decoration: underline;
                        color: #0066CC;
                    }

                    .ma0 table th a:hover {
                        font-weight: bold;
                        text-decoration: underline;
                        color: #AA0000;
                        cursor: pointer;
                    }

                    .ma0 table th a:active {
                        font-weight: bold;
                        text-decoration: underline;
                        color: #AA0000;
                        cursor: pointer;
                    }

            .ma0 table td {
                width: 4%;
                text-align: right;
            }

    .ma1 {
        margin: 5px;
        text-align: left;
        overflow-y: auto;
        height: 400px;
        width: 100%;
        line-height: 25px;
        font-size: 14px;
    }

    .flexbox_a {
        height: 16px;
        width: 16px;
        text-align: center;
        color: black;
        text-decoration: none;
        border: 1px solid #808080;
        background-color: #f0f0f0;
        border-radius: 50%;
        -moz-border-radius: 50%;
        -webkit-border-radius: 50%;
        vertical-align: central;
        padding-left: 5px;
        padding-right: 5px;
        font-weight: bold;
    }

        .flexbox_a:visited {
            text-decoration: none;
        }

        .flexbox_a:hover {
            cursor: pointer;
            text-decoration: none;
            background-color: #808080;
            color: #ffffff;
        }

        .flexbox_a:active {
            text-decoration: none;
        }
</style>
<div style="clear: both;"></div>
<div class="divMain">
    <img width="946" height="600" border="0" usemap="#Map" src="http://202.118.11.33/NEU_EC/PublishingImages/images/neuec.png" hidefocus="true" />
    <map name="Map" id="Map">
        <area shape="rect" coords="135,180,805,250" href="#1" data-id="ma1" onclick="showt(this)" onfocus="blur(this)" alt="" title="Click to view the introduction of NEU English Corpus"/>
        <area shape="rect" coords="225,325,360,405" href="#2" data-id="ma2" onclick="showt(this)" onfocus="blur(this)" alt="" title="Click to view the introduction of NEULC">
        <area shape="rect" coords="585,325,725,405" href="#3" data-id="ma3" onclick="showt(this)" onfocus="blur(this)" alt="" title="Click to view the introduction of NEUAC"/>
    </map>
    <div id="ma1" style="display: none;" class="ma">
        <div id="ma10" runat="server" class="ma0">
            <table>
                <tr>
                    <th>
                        <asp:Label ID="lbNEUEC" runat="server" Text="NEU English Corpus" Font-Bold="true" Font-Size="16px"></asp:Label>
                    </th>
                    <td>
                        <a onclick="hiddThis('ma1')" class="flexbox_a">×</a>
                    </td>
                </tr>
            </table>

        </div>
        <div id="divNEUEC" runat="server" class="ma1">
            The introduction of NEU English Corpus
        </div>

    </div>
    <div id="ma2" style="display: none;" class="ma">
        <div id="ma20" runat="server" class="ma0">
            <table>
                <tr>
                    <th>
                        <asp:HyperLink ID="lnkNEULC" NavigateUrl="http://202.118.11.33/NEU_EC/_layouts/15/FSCAppPages/Corpus/neulc.aspx?cp=neulc" ToolTip="Click to visit the NEULC website" runat="server" Font-Bold="true" Font-Size="16px"> >> NEULC << </asp:HyperLink>
                    </th>
                    <td>
                        <a onclick="hiddThis('ma2')" class="flexbox_a">×</a>
                    </td>
                </tr>
            </table>

        </div>
        <div id="divNEULC" runat="server" class="ma1">
            The introduction of NEULC
        </div>

    </div>
    <div id="ma3" style="display: none;" class="ma">
        <div class="ma0">
            <table>
                <tr>
                    <th>
                        <asp:HyperLink ID="lnkNEUAC" NavigateUrl="http://202.118.11.33/NEU_EC/_layouts/15/FSCAppPages/Corpus/neulc.aspx?cp=neuac" ToolTip="Click to visit the NEUAC website" Font-Bold="true" Font-Size="16px" runat="server" > >> NEUAC << </asp:HyperLink>
                    </th>
                    <td>
                        <a onclick="hiddThis('ma3')" class="flexbox_a">×</a>
                    </td>
                </tr>
            </table>
        </div>
        <div id="divNEUAC" runat="server" class="ma1">
            The introduction of NEUAC
        </div>
    </div>
</div>
<div style="clear: both;"></div>
