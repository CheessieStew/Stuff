﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin2.aspx.cs" Inherits="z4_2.Admin.admin2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Only for Admins 2! Welcome, <%= User.Identity.Name %>
    </div>
    </form>
</body>
</html>