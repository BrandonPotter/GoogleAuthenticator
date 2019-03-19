<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Google.Authenticator.WebSample.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Google Authenticator Sample</title>
    <style type="text/css">
        body
        {
            font-family: Arial;
            font-size: 14px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <strong>Account Secret Key (randomly generated):</strong> <asp:Label runat="server" ID="lblSecretKey"></asp:Label>
        <hr />
        <strong>Setup QR Code:</strong><br />
        <asp:Image ID="imgQrCode" runat="server" /><br />
        <br />
        <strong>Manual Setup Code: </strong> <asp:Label runat="server" ID="lblManualSetupCode"></asp:Label>
        <hr />
        Validate Code: <asp:TextBox runat="server" ID="txtCode"></asp:TextBox> <asp:Button runat="server" ID="btnValidate" Text="Validate My Code!" OnClick="btnValidate_Click" /><br /><asp:Label runat="server" Font-Bold="true" ID="lblValidationResult"></asp:Label>
    </div>
    </form>
</body>
</html>
