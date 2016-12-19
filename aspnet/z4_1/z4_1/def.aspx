<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="def.aspx.cs" Inherits="z4_1.scripts.def" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Menu ID="Menu1" runat="server" BackColor="#F7F6F3" DataSourceID="SiteMapDataSource1" DynamicHorizontalOffset="2" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#7C6F57" Orientation="Horizontal" StaticSubMenuIndent="10px" Width="100%">
            <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            <DynamicHoverStyle BackColor="#7C6F57" ForeColor="White" />
            <DynamicMenuStyle BackColor="#F7F6F3" />
            <StaticSelectedStyle BackColor="#5D7B9D" />
            <DynamicSelectedStyle BackColor="#5D7B9D" />
            <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            <StaticHoverStyle BackColor="#7C6F57" ForeColor="White" />
        </asp:Menu>
        <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" ShowStartingNode="False" />
    </div>
    </form>
</body>
</html>
