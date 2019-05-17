<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CorpusUserControl.ascx.cs" Inherits="FSCWebParts.Corpus.CorpusUserControl" %>
<div id="divhead">
    <style type="text/css">
        .addBtn {
            font-weight: bold;
            border: none;
            background-color: #6699FF;
            padding: 5px;
            font-size: 2em;
            color: #fff;
            border-radius: 100%;
            width: 60px;
            height: 60px;
            display: flex;
            justify-content: center;
            align-items: center;
            cursor: pointer;
            cursor: hand;
            margin-top: 5px;
            outline: none;
            user-select: none;
        }

        .myform {
            padding: 5px;
        }

            .myform tr {
                line-height: 30px;
                padding-bottom:5px;
            }

                .myform tr th {
                    text-align: right;
                    width: 100px;
                    max-width:300px;
                    vertical-align:top;
                }

                .myform tr td {
                    text-align: left;
                }

                    .myform tr td text {
                        width: 280px;
                    }
        .myGrid{
            min-width:600px;
        }
    </style>
    <script type="text/javascript">
        var currentRowId = 0;
        var styleName = "";
        function SelectRow(ev, strGvName) {
            var e = window.event || ev;
            var keyCode = -1;
            if (e.which == null)
                keyCode = e.keyCode; // IE
            else
                if (e.which > 0)
                    keyCode = e.which; // All others
            if (keyCode == 40)
                MarkRow(currentRowId + 1, strGvName);
            if (keyCode == 38) {
                MarkRow(currentRowId - 1, strGvName);
            }

            document.getElementById("NUM").value = currentRowId;
        }
        function MarkRow(rowId, strGvName) {
            var Grid = document.getElementById(strGvName);
            var rowCount = Grid.rows.length;
            if (document.getElementById(strGvName + rowId) == null)
                return;
            if (rowId == rowCount) {
                return;
            }
            if (document.getElementById(strGvName + currentRowId) != null)
                document.getElementById(strGvName + currentRowId).style.backgroundColor = styleName;
            currentRowId = rowId;
            styleName = document.getElementById(strGvName + rowId).style.backgroundColor;
            document.getElementById(strGvName + rowId).style.backgroundColor = 'red';
            var obj = document.getElementById(strGvName);
            obj.rows[rowId].cells[0].focus();
            document.getElementById("NUM").value = currentRowId;

        }
    </script>
    <script type="text/javascript">
        var prevselitem = null;
        function selectx(row) {
            if (prevselitem != null) {
                prevselitem.style.backgroundColor = '#ffffff';
            }
            row.style.backgroundColor = '#CDE6F7';
            prevselitem = row;

        }

    </script>
</div>
<div id="divBody" runat="server">
    <asp:Label ID="lbPTitle" runat="server" Font-Size="16" Text="Corpus"></asp:Label><hr />
    <table>
        <tr>
            <td>
                <div id="divList" runat="server">
                    <div style="padding: 5px; text-align: right;">
                        <asp:Button ID="btnAdd" runat="server" Text="Add Corpora" ToolTip="Add a New Corpora" />
                    </div>
                    <div id="divQuery">
                        <table class="myform">
                            <tr>
                                <th style="text-align: right;">KeyWord：
                                </th>
                                <td>
                                    <asp:TextBox ID="tbKeyWord" runat="server" AutoPostBack="true"></asp:TextBox>

                                    <asp:Button ID="btnQuery" runat="server" Text="Find matching strings" />
                                </td>
                            </tr>
                            <tr>
                                <th style="text-align: right;">Style：
                                </th>
                                <td>
                                    <asp:CheckBoxList ID="cblQueryStyle" runat="server" RepeatDirection="Horizontal" AutoPostBack="true"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th style="text-align: right;">Topic：
                                </th>
                                <td>
                                    <asp:CheckBoxList ID="cblQueryTopics" runat="server" RepeatDirection="Horizontal" AutoPostBack="true"></asp:CheckBoxList>
                                </td>
                            </tr>

                        </table>
                    </div>
                    <asp:GridView ID="gvCorpus" runat="server" AutoGenerateColumns="False" CellPadding="2" CellSpacing="2" BorderColor="#CCCCCC" BorderStyle="None" GridLines="Horizontal" DataKeyNames="ID" CssClass="myGrid" PageSize="20">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:TemplateField HeaderText="&nbsp;Title&nbsp;">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdfCorporaID" runat="server" Value='<%#Eval("ID")%>' />
                                    <asp:Label runat="server" Text='<%#Eval("Title")%>' ID="lbTitle"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="&nbsp;Topic&nbsp;">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdfTopicId" runat="server" Value='<%#Eval("TopicID")%>' />
                                    <asp:Label runat="server" Text="" ID="lbTopic"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="&nbsp;Literary&nbsp;">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdfStyleId" runat="server" Value='<%#Eval("StyleID")%>' />
                                    <asp:Label runat="server" Text="" ID="lbStyle"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="&nbsp;Context&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%#Eval("Context")%>' ID="lbContext"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="&nbsp;Manage&nbsp;" Visible="false">
                                <ItemTemplate>
                                    <div style="padding: 5px;">
                                        <asp:LinkButton ID="lnkEdit" runat="server" Font-Underline="false" CommandArgument='<%#Eval("ID") %>' CommandName="EditPlan">
                                 <img src="../../../../_layouts/15/VSProject/images/Edit.png" width="20" height="20" alt="" title="Edit this Corpora"/>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="lnkDel" runat="server" Font-Underline="false" CommandArgument='<%#Eval("ID") %>' CommandName="DelPlan" OnClientClick="return confirm('Are you sure to delete this Corpora？')" ToolTip="Delete this Corpora">
                                 <img src="../../../../_layouts/15/VSProject/images/Del.png" width="20" height="20" alt="" title="Delete this Corpora"/>
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <EditRowStyle BackColor="#2461BF" />
                        <EmptyDataTemplate>
                            <asp:Label ID="lbEmptPlan" runat="server" Text="There is none corpora here!" ForeColor="red"></asp:Label>
                        </EmptyDataTemplate>

                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" Height="28" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#EFF3FB" />
                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#F5F7FB" />
                        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                        <SortedDescendingCellStyle BackColor="#E9EBEF" />
                        <SortedDescendingHeaderStyle BackColor="#4870BE" />
                    </asp:GridView>

                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div id="divEditCorpora" runat="server">

                    <table class="myform" style="width: 100%">
                        <tr>
                            <th>Topic:
                            </th>
                            <td>
                                <asp:CheckBoxList ID="cblTopics" runat="server" RepeatColumns="5" RepeatDirection="Horizontal" ToolTip="Choose any of the Topic that the Corpora belongs to"></asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <th>Literary Style:
                            </th>
                            <td>
                                <asp:CheckBoxList ID="cblLiterary" runat="server" RepeatColumns="5" RepeatDirection="Horizontal" ToolTip="Choose any of the Literary Styles that the Corpora belongs to"></asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <th>Grade:
                            </th>
                            <td>
                                <asp:DropDownList ID="ddlGrade" Width="210px" runat="server" ToolTip="Choose one Grade that the Corpora Author belongs to"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th>Source:
                            </th>
                            <td>
                                <asp:TextBox ID="tbSource" runat="server" Width="200px" ToolTip="Fill in this Blank with the source of the Corpora"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <th style="vertical-align: top;">Context:
                            </th>
                            <td>
                                <asp:TextBox ID="tbDesc" runat="server" TextMode="MultiLine" Rows="5" Width="200px" ToolTip="FIll in this blank with source context!"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: center;">
                                <div style="padding: 10px;">
                                    &nbsp;&nbsp;
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" />&nbsp;&nbsp;
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div>
                                    <asp:HiddenField ID="hdfID" runat="server" Value="" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div id="divViewCorpora" runat="server" visible="false">
                    <div>
                        Source information:
                    </div>
                    <div>
                        Expanded context:
                    </div>
                </div>
            </td>
        </tr>
    </table>
</div>

<div id="divForm" >
    <asp:CheckBoxList ID="CheckBoxList1" runat="server">
        <asp:ListItem Value="1">F1</asp:ListItem>
        <asp:ListItem Value="2">S2</asp:ListItem>
        <asp:ListItem Value="3">J3</asp:ListItem>
        <asp:ListItem Value="4">S4</asp:ListItem>
    </asp:CheckBoxList>
</div>
<asp:Label ID="lbErr" runat="server" Text="" ForeColor="red"></asp:Label>

