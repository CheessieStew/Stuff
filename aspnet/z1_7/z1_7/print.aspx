<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="print.aspx.cs" Inherits="z1_7.print" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" href="table.css" />
    <title>Print</title>
</head>
<body>
    <table>
        <tr>
            <td>Imię i nazwisko:</td>
            <td><%:Request.Form["Name"] + " " + Request.Form["Surname"] %></td>
            <td>Lista:</td>
            <td><%:Request.Form["ListNo"] %></td>
        </tr>
    </table>
    <table border="0">
        <tr>
            <td>Zadanie:</td>
            <asp:Repeater runat="server" ID="TaskNo">
                <ItemTemplate>
                    <td> <%#Container.DataItem.ToString()%></td>
                </ItemTemplate>
            </asp:Repeater>
            <td>Data:</td>
            <td><%:Request.Form["Date"] %></td>
        </tr>
        <tr>
            <td>Punkty:</td>
            <asp:Repeater runat="server" ID="TaskPo">
                <ItemTemplate>
                    <td> <%# PointsString(Container.DataItem)%></td>
                </ItemTemplate>
            </asp:Repeater>
            <td>suma:</td>
            <td><%:sum %></td>
        </tr>
    </table>
</body>
</html>
