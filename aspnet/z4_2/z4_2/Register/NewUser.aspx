<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewUser.aspx.cs" Inherits="z4_2.Register.NewUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:HyperLink ID="HyperLink1" NavigateUrl="~/loginPage.aspx" Text="Go back" runat="server" />
        </div>
        <div>
            <asp:Label runat="server" Text="Name"/>
            <asp:TextBox runat="server" ID="NameBox" />
        </div>
        <div>
            <asp:Label runat="server" Text="Email"/>
            <asp:TextBox runat="server" ID="EmailBox" />
        </div>
        <div>
            <asp:Label runat="server" Text="Password"/>
            <asp:TextBox runat="server" ID="PasswordBox" TextMode="Password" />
        </div>
        <div>
            <asp:Button runat="server" ID="CreateAccountButton" Text="Register!" OnClick="CreateAccountButton_Click" />
        </div>
        <div>
            <asp:Label runat="server" ID="CreateAccountResults" Text="" />
        </div>
    </form>
</body>
</html>
