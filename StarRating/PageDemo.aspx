<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageDemo.aspx.cs" Inherits="StarRating.PageDemo" %>

<%@ Register Src="~/Controls/Pageination.ascx" TagPrefix="uc1" TagName="Pageination" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc1:Pageination runat="server" ID="Pagination" />
    </div>
    </form>
</body>
</html>
