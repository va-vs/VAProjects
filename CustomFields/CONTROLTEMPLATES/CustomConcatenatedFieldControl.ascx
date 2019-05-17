<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" %>
<SharePoint:RenderingTemplate ID="DefaultCustomFieldControl" runat="server">
    <Template>

         <table id="tblLevel" style ="border-spacing:0px 10px" runat ="server"   ><tr>
             <td><asp:DropDownList Width ="120px" ID ="DropDownList1"  runat ="server"></asp:DropDownList></td><td><span id="spanDesc"  runat ="server" style="color:red " ></span><asp:DropDownList ID ="ddlDes" runat ="server"  Visible ="false" ></asp:DropDownList></td></tr>
         </table>
        <div ><span id="spanTxtDesc"  visible ="false"  runat ="server" style="margin-left:0px; color:red " >列表中没有你要的操作,请在上方文本框输入</span></div>
        
     </Template>
</SharePoint:RenderingTemplate>
<SharePoint:RenderingTemplate ID="DisplayCustomFieldControl" runat="server">
    <Template>
        <asp:Label ID="lblDisplayText" runat="server" />
    </Template>
</SharePoint:RenderingTemplate>