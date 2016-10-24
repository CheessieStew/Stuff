<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dekl.aspx.cs" Inherits="z1_7.dekl" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <br />
    <form id="Deklarka" method="post" runat="server">
        <div>
            <table>
                <tr>
                    <td><asp:Label runat="server" Text="Imię:" /></td>
                    <td><asp:TextBox runat="server" ID="Name"/></td>
                </tr>
                <tr>
                    <td><asp:Label runat="server" Text="Nazwisko:" /></td>
                    <td><asp:TextBox runat="server" ID="Surname" /></td>
                </tr>
                <tr>
                    <td><asp:Label runat="server" Text="Data:" /></td>
                    <td><asp:TextBox runat="server" type="date" ID="Date"/></td>
                </tr>
                <tr>
                    <td><asp:Label runat="server" Text="Numer zestawu:"/></td>
                    <td><asp:TextBox runat="server" type="number" ID="ListNo"/></td>
                </tr>
                <tr><td></td></tr>

                <asp:Repeater runat="server" ID="TasksRepeater" EnableViewState="False" ViewStateMode="Disabled">
                    <ItemTemplate >
                    <tr>
                        <td>Punkty za zad. <%#Container.DataItem.ToString()%>:</td>
                        <td><asp:TextBox type="number" runat="server" ID="TaskN"/></td>
                    </tr>
                    </ItemTemplate>
                </asp:Repeater>

                <tr><td><asp:Button runat="server" ID="Send" Text="To wszystko" /></td></tr>
            </table>
        </div>
    </form>
</body>
</html>
