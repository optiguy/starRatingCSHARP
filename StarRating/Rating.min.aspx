<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rating.min.aspx.cs" Inherits="StarRating.RatingMin" %>

<%@ Register Src="~/Controls/StarRating.min.ascx" TagPrefix="uc1" TagName="StarRating" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Rating Eksempel</title>
    <link rel="stylesheet" type="text/css" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css" />
    <script type="text/javascript" src="https://code.jquery.com/jquery-2.2.3.min.js"></script>
    <script type="text/javascript" src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js"></script>
    <style> 
        /* Aldrig inline styling!!! Men dette er en demo */ 
        img { max-width:100% }
        .container { background-color: #f8f8f8; border: solid 1px #e7e7e7; box-shadow: #ccc 0 0 7px }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="container">

        <header>
            <nav class="navbar navbar-default">
                <div class="container-fluid">
                    <div class="navbar-header">
                        <a class="navbar-brand" href="#">
                            <img style="max-height:100%;" alt="Brand" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACgAAAAoCAMAAAC7IEhfAAAA81BMVEX///9VPnxWPXxWPXxWPXxWPXxWPXxWPXz///9hSYT6+vuFc6BXPn37+vz8+/z9/f2LeqWMe6aOfqiTg6uXiK5bQ4BZQX9iS4VdRYFdRYJfSINuWI5vWY9xXJF0YJR3Y5Z4ZZd5ZZd6Z5h9apq0qcW1qsW1q8a6sMqpnLyrn76tocCvpMGwpMJoUoprVYxeRoJjS4abjLGilLemmbrDutDFvdLPx9nX0eDa1OLb1uPd1+Td2OXe2eXh3Ofj3+nk4Orl4evp5u7u7PLv7fPx7/T08vb08/f19Pf29Pj39vn6+fuEcZ9YP35aQn/8/P1ZQH5fR4PINAOdAAAAB3RSTlMAIWWOw/P002ipnAAAAPhJREFUeF6NldWOhEAUBRvtRsfdfd3d3e3/v2ZPmGSWZNPDqScqqaSBSy4CGJbtSi2ubRkiwXRkBo6ZdJIApeEwoWMIS1JYwuZCW7hc6ApJkgrr+T/eW1V9uKXS5I5GXAjW2VAV9KFfSfgJpk+w4yXhwoqwl5AIGwp4RPgdK3XNHD2ETYiwe6nUa18f5jYSxle4vulw7/EtoCdzvqkPv3bn7M0eYbc7xFPXzqCrRCgH0Hsm/IjgTSb04W0i7EGjz+xw+wR6oZ1MnJ9TWrtToEx+4QfcZJ5X6tnhw+nhvqebdVhZUJX/oFcKvaTotUcvUnY188ue/n38AunzPPE8yg7bAAAAAElFTkSuQmCC">
                        </a>
                    </div>
                </div>
            </nav>
        </header>

        <main>
            
            <!-- Article content -->
            <h1><asp:Literal runat="server" ID="Article_Title"></asp:Literal></h1>
            
            <!-- Meta data -->
            <p><small><asp:Literal ID="Article_Date" runat="server"></asp:Literal></small></p>
            <uc1:StarRating runat="server" id="Article_StarRating" />
            <!-- END: Meta data-->
            
            <asp:Literal runat="server" ID="Article_Content"></asp:Literal>
            <!-- END: Article content-->
        
        </main>

        <footer>
            <div class="row">
                <div class="col-sm-6 col-md-3"><img src="http://placekitten.com/600/399" /></div>
                <div class="col-sm-6 col-md-3"><img src="http://placekitten.com/600/400" /></div>
                <div class="col-sm-6 col-md-3"><img src="http://placekitten.com/600/401" /></div>
                <div class="col-sm-6 col-md-3"><img src="http://placekitten.com/600/402" /></div>
            </div>
            <!-- Add footer here -->
        </footer>

    </form>
</body>
</html>
