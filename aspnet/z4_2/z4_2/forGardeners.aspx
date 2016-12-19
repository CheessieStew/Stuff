<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="forGardeners.aspx.cs" Inherits="z4_2.forGardeners" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Only for Gardeners! Welcome, <%= User.Identity.Name %>
    </div>
    <div>
        <asp:Button runat="server" ID="LogOutButton" Text="Log out" OnClick ="LogOutButton_Click" />
    </div>
    </form>
</body>
</html>
