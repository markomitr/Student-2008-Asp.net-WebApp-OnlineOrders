<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Naracaj.aspx.cs" Inherits="Naracaj" %>
<%@ MasterType VirtualPath="~/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="Scripta/jquery-1.3.2.js" type="text/javascript"></script>
    <script src="Scripta/jquery.cycle.all.min.js" type="text/javascript"></script>
    <script src="Scripta/Scripti.js" type="text/javascript"></script> 
    <style type="text/css">
        
        table
        {
            margin:20px 0 0 10px;
        }
        table td
        {
        	padding:0 4px;
        }
        
    .hide
    {
    	display:none;
    	
    }
    .show
    {
    	position:absolute;
    	top:10px;
    	left:10px;
        background-color:White;
        z-index:200;
    }
    .textCellLevo
    {
    	text-align:right;
    	color:Green;
    }
    .textVkupno
    {
    	color:Maroon;
    	font-weight:bolder;
    }
    .nar {  
    height:  530px;  
    width:   120px;  
    padding: 0;  
    margin:  0;  
    } 
 
    .nar img {  
    padding: 15px;  
    border:  1px solid #ccc;  
    background-color: #eee;  
    height:  530px;  
    width:   120px; 
    top:  0; 
    left: 0 
    } 
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="narackaMeni">
<a href="Naracaj.aspx">Нарачка Дома</a><a href="Naracaj.aspx?Mod=1&A=1">Направи Нарачка</a><a href="Naracaj.aspx?Mod=2&A=1">Извештаи Нарачки</a><a href="Naracaj.aspx?Mod=2&A=3">ПОПУСТИ-Листа</a>
</div>
        <asp:ScriptManager ID="ScriptManagerNaracka" runat="server">
        </asp:ScriptManager>
    <div id="glavenContentNaracki" style="float:left; width:850px;">
        <div id="defaultDiv" class="narackaPrva" runat="server">
        <table>
        <tr>
        <td>
        <a href="Naracaj.aspx?Mod=1&A=1"><img src="Images/SlikaCart.jpg" /></a>
        </td>
         <td><a href="Naracaj.aspx?Mod=2&A=1"><img src="Images/SlikaCart.jpg" /></a></td>
        </tr>
        <tr>
        <td>
        Направи Нарачка
        </td>
         <td>
         Извештаи по нарачки
        </td>
        </tr>
        </table>
        </div>
        <div id="vnesDiv" runat="server">
        <asp:UpdatePanel ID="vnesiUP" runat="server"></asp:UpdatePanel>
        </div>
        <div id="podatociDiv" >
       <asp:UpdatePanel  ID="podatociUP" runat="server" UpdateMode="Conditional"></asp:UpdatePanel>
        </div>
        <div id="narackaLista">
        <asp:UpdatePanel ID="narackaListaUP"   runat="server" UpdateMode="Conditional">
        </asp:UpdatePanel>
        </div>
        </div>
        <div id="reklami" style=" position:relative; height:650px; width:200px; float:left;">
        <asp:UpdatePanel ID="proba" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <div id="reklamiNarackiDiv" style="float:left; width:150px; height:570px; overflow:hidden;" runat="server"></div>
        </ContentTemplate>
        </asp:UpdatePanel>
        </div>
        <div id="shadow"></div>
</asp:Content>

