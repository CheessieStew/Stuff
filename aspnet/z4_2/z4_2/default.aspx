<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="z4_2._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:HyperLink ID="HyperLink1" NavigateUrl="~/forGardeners.aspx" Text="gardeners only!" runat="server" />
    </div>
    <div>
        <asp:HyperLink ID="HyperLink2" NavigateUrl="~/Admin/admin1.aspx" Text="admins only 1!" runat="server" />
    </div>
    <div>
        <asp:HyperLink ID="HyperLink3" NavigateUrl="~/Admin/admin2.aspx" Text="admins only 2!" runat="server" />
    </div>
    <div>
        <p>The default page! Welcome, <%= User.Identity.Name %> </p>
        <p>Your roles:</p>
        <%foreach (String s in Roles.GetRolesForUser(User.Identity.Name)){ %>
        <%=s %> <br />
        <%} %>


    </div>
    <div>
        <asp:Button runat="server" ID="LogOutButton" Text="Log out" OnClick ="LogOutButton_Click" />
    </div>
    </form>
</body>
</html>
