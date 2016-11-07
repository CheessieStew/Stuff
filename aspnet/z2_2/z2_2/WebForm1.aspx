<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="z2_2.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>


    <form id="form1" runat="server">
    <div>
        <asp:Label ID="lab1" runat="server" />
        <asp:TextBox ID="text1" runat="server"/> 
        <asp:Button ID="button1" runat="server"/>
        <br />
        <asp:Label ID="lab2" runat="server" />
        <br />
        <asp:Label ID="lab3" runat="server" />
        <br />
        <asp:FileUpload ID="file" runat="server" />
        <asp:Label ID="lc1" runat="server" />
        <asp:Label ID="lc2" runat="server" />
        <asp:Label ID="lc3" runat="server" />
        
    </div>
    </form>
</body>
</html>
