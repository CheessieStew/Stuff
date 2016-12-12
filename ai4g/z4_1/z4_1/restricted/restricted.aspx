<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="restricted.aspx.cs" Inherits="z4_1.restricted" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <p>stuff</p>
        <p>[<%=User.Identity.Name %>]</p>
    </div>
    </form>
</body>
</html>
