<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StarRating.ascx.cs" Inherits="StarRating.StarRating" %>

<style>
    /* Lav ikke inline styling. Dette er kun for demostration */
    .rating {
        margin: 15px 0;
    }
    .rating--score {
        margin-right: 10px;
        display: inline;
        font-weight:bold;
    }
    .rating i{
        font-size:1.5em;
        letter-spacing: 0.5em;
    }
</style>

<div class="rating">
    <asp:Panel ID="Article_Rating" runat="server"></asp:Panel>
    <asp:Literal ID="Article_Score" runat="server"></asp:Literal>
    <asp:Label ID="Article_Message" runat="server"></asp:Label>
</div>
