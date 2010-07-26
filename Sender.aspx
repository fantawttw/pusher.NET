<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Sender.aspx.vb" Inherits="Sender" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<!--
=====================================================================================================================


Copyright 2010, Jon Burrows. Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php


=====================================================================================================================
-->
    <title>Pusher.NET Sender Page</title>
</head>

<body>
    <form id="form1" runat="server">
    <div>
    Message to Push
    <asp:textbox runat="server" ID="txtMessageToPush"></asp:textbox>
    <asp:LinkButton ID="lnkPushIt" runat="server">Push It</asp:LinkButton>

    </div>
    </form>
</body>
</html>
