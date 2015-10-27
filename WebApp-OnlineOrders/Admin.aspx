
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="Admin"  Trace="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="Styles/Admin.css" rel="stylesheet" type="text/css" />

    <script src="Scripta/jquery-1.3.2.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function() {
            try {
                var tabeli = $('#Rezultat').children('table');
                tabeli.hide();
                $('#Rezultat table:first').show('normal');

                if (tabeli.length > 1) {
                    var footer = $("#TabelaFooter");
                    var rez = '<ul>';
                    for (i = 0; i < tabeli.length; i++) {
                        rez += '<li><a href="#" id="' + i + '">' + (i + 1) + '</a></li>';
                    }
                    rez += '</ul>';
                    footer.html(rez);
                    $('#TabelaFooter a:first').addClass('selektiran');
                }

                $('#Rezultat a[class="red"]').click(function() {
                    $("table .selektiran").removeClass();
                    var id = $(this).parent().next().html();
                    var url = location.search;
                    url = url.replace('?', '');
                    var tabeli = url.split('&');
                    var brTabela = tabeli[0].split('=');
                    var akcija = tabeli[1].split('=');
                    if (akcija[1] == 2) {
                        akcija[1] = 4;
                    }
                    if (brTabela[1] == 11 || brTabela[1] == 12) {
                        var id2 = $(this).parent().next().next().html();
                        $(this).attr('href', 'Admin.aspx?T=' + brTabela[1] + '&A=' + akcija[1] + '&ID=' + id + '&ID2=' + id2);
                    } else {

                        $(this).attr('href', 'Admin.aspx?T=' + brTabela[1] + '&A=' + akcija[1] + '&ID=' + id);
                    }
                });
                $('#Rezultat tr').hover(function() {
                    $('#Rezultat .selektiranRed').removeClass('selektiranRed');
                    $(this).addClass('selektiranRed');
                });
                $('#TabelaFooter a').click(function() {
                    $('#TabelaFooter a').removeClass('selektiran');
                    $('#Rezultat table').hide('fast');
                    var tab = $(this).attr('id');
                    $('#Rezultat table:eq(' + tab + ')').show('fast');
                    $(this).addClass('selektiran');
                }
            );

                $('#submit_kopce').click(function() {
                    var array_values = "";
                    $('#Rezultat input[type="text"]').each(function() {
                        array_values += $(this).val() + '*';
                    });

                    $('#Rezultat select').each(function() {
                        array_values += $(this).val() + '*';
                    });



                    $('#vrednost').attr('value', array_values);
                });

                $('#dodadi_kopce').click(function() {
                    var array_values = "";
                    $('#Rezultat input[type="text"]').each(function() {
                        array_values += $(this).val() + '*';
                    });

                    $('#Rezultat select').each(function() {
                        array_values += $(this).val() + '*';
                    });


                    $('#vrednost').attr('value', array_values);
                });

                $('#brisi_kopce').click(function() {
                    var array_values = "";
                    $('#Rezultat input[type="text"]').each(function() {
                        array_values += $(this).val() + '*';
                    });

                    $('#Rezultat select').each(function() {
                        array_values += $(this).val() + '*';
                    });


                    $('#vrednost').attr('value', array_values);
                });

            }
            catch (Error) {
            }

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div id="Meni" class="AdminMeni">
        <ul id="AdminMeni" runat="server">
            <li>
               <a href="#">Додади</a> 
                <ul class="nav">
                    <li><a href="Admin.aspx?T=1&A=1">Данок</a></li>
                    <li><a href="Admin.aspx?T=2&A=1">Град</a></li>
                    <li><a href="Admin.aspx?T=3&A=1">Фирма</a></li>
                    <li><a href="Admin.aspx?T=4&A=1">Комитент</a></li>
                    <li><a href="Admin.aspx?T=5&A=1">Корисници</a></li>
                    <li><a href="Admin.aspx?T=6&A=1">Локации</a></li>
                    <li><a href="Admin.aspx?T=7&A=1">Производ</a></li>                    
                    <li><a href="Admin.aspx?T=8&A=1">Тип на Цена</a></li>
                    <li><a href="Admin.aspx?T=9&A=1">Група на Производ</a></li>
                    <li><a href="Admin.aspx?T=10&A=1">Нарачка</a></li>
                    <li><a href="Admin.aspx?T=11&A=1">Цена по производ</a></li>
                    <li><a href="Admin.aspx?T=12&A=1">Цена по група на производ</a></li>
                </ul>
            </li>
            <li><a href="#">Измени</a> 
                <ul class="nav">
                    <li><a href="Admin.aspx?T=1&A=2">Данок</a></li>
                    <li><a href="Admin.aspx?T=2&A=2">Град</a></li>
                    <li><a href="Admin.aspx?T=3&A=2">Фирма</a></li>
                    <li><a href="Admin.aspx?T=4&A=2">Комитент</a></li>
                    <li><a href="Admin.aspx?T=5&A=2">Корисници</a></li>
                    <li><a href="Admin.aspx?T=6&A=2">Локации</a></li>
                    <li><a href="Admin.aspx?T=7&A=2">Производ</a></li>                    
                    <li><a href="Admin.aspx?T=8&A=2">Тип на Цена</a></li>
                    <li><a href="Admin.aspx?T=9&A=2">Група на Производ</a></li>
                    <li><a href="Admin.aspx?T=10&A=2">Нарачка</a></li>
                    <li><a href="Admin.aspx?T=11&A=2">Цена по производ</a></li>
                    <li><a href="Admin.aspx?T=12&A=2">Цена по група на производ</a></li>
                </ul>
            </li>
             <li><a href="#">Бриши</a> 
                <ul class="nav">
                    <li><a href="Admin.aspx?T=1&A=3">Данок</a></li>
                    <li><a href="Admin.aspx?T=2&A=3">Град</a></li>
                    <li><a href="Admin.aspx?T=3&A=3">Фирма</a></li>
                    <li><a href="Admin.aspx?T=4&A=3">Комитент</a></li>
                    <li><a href="Admin.aspx?T=5&A=3">Корисници</a></li>
                    <li><a href="Admin.aspx?T=6&A=3">Локации</a></li>
                    <li><a href="Admin.aspx?T=7&A=3">Производ</a></li>                    
                    <li><a href="Admin.aspx?T=8&A=3">Тип на Цена</a></li>
                    <li><a href="Admin.aspx?T=9&A=3">Група на Производ</a></li>
                    <li><a href="Admin.aspx?T=10&A=3">Нарачка</a></li>
                    <li><a href="Admin.aspx?T=11&A=3">Цена по производ</a></li>
                    <li><a href="Admin.aspx?T=12&A=3">Цена по група на производ</a></li>
                </ul>
            </li>
        </ul>
    </div>
    <div id="Rezultat">
    <p id="TabelaRez" runat="server"></p>
    </div>
    <div class="Clear"></div>
</asp:Content>

