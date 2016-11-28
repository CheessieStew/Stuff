<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="bind1.aspx.cs" Inherits="z3_1.bind1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:DropDownList id="List1" runat="server"></asp:DropDownList>
    <asp:DropDownList id="List2" runat="server" DataTextFormatString ="{0:d}"></asp:DropDownList>
    <asp:DropDownList id="List3" runat="server"></asp:DropDownList>

    </div>
    </form>
</body>
</html>
