<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="z1_6.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <p> Nie mam na kebaba.</p>
    <p><%=SomeString %></p>
    <form id="getButton" method="get">
        <div>
            <input type="text" name="getInput1" />
            <input type="text" name="getInput2" />
            <input type="submit" value="żądanie GET" />

        </div>
    </form>
    <form id="Form1" method="post" runat="server">
        <div>
            <asp:TextBox runat="server" ID="postInput1" />
            <asp:TextBox runat="server" ID="postInput2" />
            <asp:LinkButton runat="server" Text="żądanie POST" />
        </div>
    </form>
</body>
</html>
