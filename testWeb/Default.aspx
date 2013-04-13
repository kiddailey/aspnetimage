<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="testWeb._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<asp:Image ID="Image1" ImageUrl="prova2.jpg" BorderColor="Black" BorderWidth=1 Width="50%" Height="50%" runat="server" />    
<asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click" />
</asp:Content>
