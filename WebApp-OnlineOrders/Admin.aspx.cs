using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using Oracle.DataAccess;
using Oracle.DataAccess.Client;
using System.Collections;
using System.Web.UI.HtmlControls;
public partial class Admin : System.Web.UI.Page
{
    ObjectDataSource Danoci,Proizvodi;
    ButtonField BrisiBF,IzmeniBF,DodadiBF;
    Administrator administrator;
    String[] Tabeli;
    int Akcija, Tabela;
    String[] Red;
    string ID,ID2;
    DropDownList lista;
    TableCell kelijaDN;
    public Dictionary<String, String> podatoci;
    protected void Page_Load(object sender, EventArgs e)
    {
        administrator = new Administrator();
        Tabeli = new String[20];
        Akcija=0;
        Tabela=0;

        Tabeli[1] = "Danok";
        Tabeli[2] = "Grad";
        Tabeli[3] = "Firma";
        Tabeli[4] = "Komitent";
        Tabeli[5] = "Korisnik";
        Tabeli[6] = "Lokacija";
        Tabeli[7] = "Proizvod";
        Tabeli[8] = "TipCena";
        Tabeli[9] = "TipProizvod";
        Tabeli[10] = "Naracka";
        Tabeli[11] = "EdinecnaCena";
        Tabeli[12] = "GrupnaCena";
        Tabeli[13] = "ZiroSmetka";
        Tabeli[14] = "ListaProizvodi";

        ID = "";
        ID2 = "";
        Red = new string[1];

        podatoci = (Dictionary<String, String>)Session["Korisnik"];

        if (podatoci != null)
        {
            if (podatoci.ContainsKey("Vid"))
            {
                if (podatoci["Vid"].Trim() == "Admin")
                {

                    try
                    {
                        if (Request.QueryString["A"] != null)
                        {
                            Akcija = Convert.ToInt32(Request.QueryString["A"].ToString());
                        }
                        if (Request.QueryString["T"] != null)
                        {
                            Tabela = Convert.ToInt32(Request.QueryString["T"].ToString());
                        }
                        if (Request.QueryString["ID"] != null)
                        {
                            ID = Request.QueryString["ID"].ToString();
                        }
                        if (Request.QueryString["ID2"] != null)
                        {
                            ID2 = Request.QueryString["ID2"].ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write(ex.Message);
                        Nacrtaj(0, 0);
                    }

                    if (Akcija == 0 && Tabela == 0)
                    {

                        Nacrtaj(0, 0);
                    }
                    else
                    {
                        Nacrtaj(Akcija, Tabela);
                    }
                }
                else
                {
                    Response.Redirect("Default.aspx");
                }
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }
        else
        {
            Response.Redirect("Default.aspx");
        }

    }
    public void Nacrtaj(int A,int T)
    {
        if (A > 0 && T > 0 && T < 15)
        {
            //   Response.Write("Akcija : " +A +" Tabela :"+ Tabeli[T].ToString());

            switch (A)
            {
                case 1:
                    {
                        //Dodavanje na nov red
                        if (!IsPostBack)
                        {
                            this.TabelaRez.Controls.Add(Dodadi(T, true,false));
                        }
                        else
                        {
                            if (Request.Params["Vrednost"] != null)
                            {
                                if (Request.Params["Vrednost"].Trim() != "")
                                {
                                    Red = Request.Params["Vrednost"].Remove(Request.Params["Vrednost"].Length - 1).Split('*');
                                    String rez = Insert();

                                    this.TabelaRez.InnerText = rez;
                                }
                            }
                        }
                        break;
                    };
                case 2:
                    {
                        //Izmena na podatok
                        this.TabelaRez.InnerHtml = Update(T);

                        break;
                    };
                case 3:
                    {
                        //Brisenje na podatok
                        if (!IsPostBack)
                        {
                            if (ID.Trim() != "")//Znaci deka ima ID sto treba da se izbrise. togas otvorame mod za brisenje
                            {
                                this.TabelaRez.Controls.Add(Dodadi(T,false,true));
                            }
                            else
                            {
                                this.TabelaRez.InnerHtml = Brisi(T);
                            }
                        }
                        else
                        {
                            if (Request.Params["Vrednost"] != null)
                            {
                                if (Request.Params["Vrednost"].Trim() != "")
                                {
                                    Red = Request.Params["Vrednost"].Remove(Request.Params["Vrednost"].Length - 1).Split('*');
                                    String rez = Zacuvaj(true);
                                    this.TabelaRez.InnerHtml = rez;
                                }
                            }
                        }
                        break;
                    };
                case 4:
                    {
                        //  Zacuvuvanje na podatok
                        if (!IsPostBack)
                        {
                            this.TabelaRez.Controls.Add(Dodadi(T, false,false));
                        }
                        else
                        {
                            if (Request.Params["Vrednost"] != null)
                            {
                                if (Request.Params["Vrednost"].Trim() != "")
                                {
                                    Red = Request.Params["Vrednost"].Remove(Request.Params["Vrednost"].Length - 1).Split('*');
                                    String rez = Zacuvaj(false);
                                    this.TabelaRez.InnerHtml = Update(T);
                                }
                            }
                        }


                        break;
                    };
                default:
                    break;
            }
        }
        else
        {

        }
    }
    public String Insert()
    {
        String Rezultat = "";

        try
        {
            switch (Tabela)
            {
                case 1:
                case 2:
                case 8:
                case 9:
                    {
                       Rezultat =  administrator.InsertVrednost(Tabeli[Tabela], Red);
                        break;
                    };
                case 4:
                    {
                        String[] pom = new String[7];
                        pom[0] = Red[0];
                        pom[1] = Red[6];
                        pom[2] = Red[1];
                        pom[3] = Red[2];
                        pom[4] = Red[3];
                        pom[5] = Red[4];
                        pom[6] = Red[5];
                        Rezultat = administrator.InsertVrednost(Tabeli[Tabela], pom);
                        break;
                    };
                case 5:
                    {
                        String[] pom = new String[8];
                        pom[0] = Red[0];
                        pom[1] = Red[6];
                        pom[2] = Red[1];
                        pom[3] = Red[2];
                        pom[4] = Red[7];
                        pom[5] = Red[5];
                        pom[6] = Red[3];
                        pom[7] = Red[4];
                        Rezultat = administrator.InsertVrednost(Tabeli[Tabela], pom);
                        break;
                    };
                case 6:
                    {
                        String[] pom = new String[5];
                        pom[0] = Red[0];
                        pom[1] = Red[3];
                        pom[2] = Red[4];
                        pom[3] = Red[1];
                        pom[4] = Red[2];

                        Rezultat = administrator.InsertVrednost(Tabeli[Tabela], pom);
                        break;
                    };
                case 7:
                    {
                        String[] pom = new String[6];
                        pom[0] = Red[0];
                        pom[1] = Red[5];
                        pom[2] = Red[4];
                        pom[3] = Red[1];
                        pom[4] = Red[2];
                        pom[5] = Red[3];
                        Rezultat = administrator.InsertVrednost(Tabeli[Tabela], pom);
                        break;
                    };
                case 11:
                    {
                        String[] pom = new String[3];
                        pom[0] = Red[2];
                        pom[1] = Red[4];
                        pom[2] = Red[3];
                        Rezultat = administrator.InsertVrednost(Tabeli[Tabela], pom);
                        break;
                    };
                case 12:
                    {
                        String[] pom = new String[3];
                        pom[0] = Red[3];
                        pom[1] = Red[2];
                        pom[2] = Red[4];
                        Rezultat = administrator.InsertVrednost(Tabeli[Tabela], pom);
                        break;
                    };
                    
                default:
                    {

                    };
                    break;
            }

            
        }
        catch (Exception ex)
        {
            
            throw;
        }

        return Rezultat;
 
    }
    public HtmlTable Dodadi(int brTabela,bool nov,bool daliBrisi)
    {
        String imeTabela = Tabeli[brTabela];
        HtmlTable Rezultat = new HtmlTable();

        if (brTabela >= 13)
        {
            HtmlTableRow red = new HtmlTableRow();
            HtmlTableCell kelija = new HtmlTableCell();
            kelija.InnerText = "Не е дозволена измена на избраната табела !";
            red.Cells.Add(kelija);
            Rezultat.Rows.Add(red);
        }
        else
        {
            if (nov)
            {
                if (brTabela == 3 || brTabela == 14)
                {
                    HtmlTableRow red = new HtmlTableRow();
                    HtmlTableCell kelija = new HtmlTableCell();
                    kelija.InnerText = "Не е дозволенo додавање на избраната табела !";
                    red.Cells.Add(kelija);
                    Rezultat.Rows.Add(red);
                }
                else
                {
                    Rezultat = administrator.EditVrednost(imeTabela, "", "", true,false);
                }
            }
            else
            {
                if (daliBrisi == false)
                {
                    Rezultat = administrator.EditVrednost(imeTabela, ID.ToString(), ID2.ToString(), false, false);
                }
                else
                {
                    Rezultat = administrator.EditVrednost(imeTabela, ID.ToString(), ID2.ToString(), false, true);
                }
            }
            
        }

        return Rezultat;
        
    }
    public String Update(int brTabela)
    {
        //Prikazuva tabela so rezultati za baranata tabela so kopce od desnata strana za IZMENI
        String rezultat = "";
        if (brTabela >= 0 && brTabela <= 14)
        {
            try
            {
                string imeTabela = Tabeli[brTabela].ToString();

                rezultat = administrator.PrikaziSredenaTabelaString(imeTabela, false);

            }
            catch (Exception ex)
            {
                rezultat = ex.Message;
            }
        }
        else
        {
            rezultat = "Погрешен избор !";
        }
        return rezultat;
    }
    public String Brisi(int brTabela)
    {
        //Prikazuva tabela so rezultati za baranata tabele so kopce od desnata strana BRISI
        String rezultat = "";
        if (brTabela >= 0 && brTabela <= 14)
        {
            try
            {
                string imeTabela = Tabeli[brTabela].ToString();

                rezultat = administrator.PrikaziSredenaTabelaString(imeTabela,true);

            }
            catch (Exception ex)
            {
                rezultat = ex.Message;
            }
        }
        else
        {
            rezultat = "Погрешен избор !";
        }
        return rezultat;
    }
    public string Zacuvaj(bool daliBrisi)
    {
        //Ako daliBrisi e FALSE , togas se povikuva UPDATE za toj red od tabelata 
        // a ako dali brisi e TRUE togas se povikuva DELETE za toj red od tabelata
        String rez = "";
        
        try
        {
            switch (Tabela)
            {
                case 1:
                case 2:
                case 3:
                case 8:
                case 9:
                    {
                        if (daliBrisi == false)
                        {
                            rez = administrator.Zacuvaj(Tabeli[Tabela].Trim(), Red);
                        }
                        else
                        {
                            rez =  administrator.DeleteVrednost(Tabeli[Tabela].Trim(), ID, ID2);
                        }
                        break;
                    };
                case 4:
                    {
                        String[] pom = new String[7];
                        pom[0] = Red[0];
                        pom[1] = Red[6];
                        pom[2] = Red[1];
                        pom[3] = Red[2];
                        pom[4] = Red[3];
                        pom[5] = Red[4];
                        pom[6] = Red[5];
                        if (daliBrisi == false)
                        {
                            rez = administrator.Zacuvaj(Tabeli[Tabela].Trim(), pom);
                        }
                        else
                        {
                            rez = administrator.DeleteVrednost(Tabeli[Tabela].Trim(), ID, ID2);
                        }
                        break;
                    };
                case 5:
                    {
                        String[] pom = new String[7];
                        pom[0] = Red[0];
                        pom[1] = Red[2];
                        pom[2] = Red[3];
                        pom[3] = Red[4];
                        pom[4] = Red[5];
                        pom[5] = Red[6];
                        pom[6] = Red[7];
                        if (daliBrisi == false)
                        {
                            rez = administrator.Zacuvaj(Tabeli[Tabela].Trim(), pom);
                        }
                        else
                        {
                            rez = administrator.DeleteVrednost(Tabeli[Tabela].Trim(), ID, ID2);
                        }
                        break;
                    };
                case 6:
                    {
                        String[] pom = new String[3];
                        pom[0] = Red[0];
                        pom[1] = Red[3];
                        pom[2] = Red[4];
                        if (daliBrisi == false)
                        {
                            rez = administrator.Zacuvaj(Tabeli[Tabela].Trim(), pom);
                        }
                        else
                        {
                            rez = administrator.DeleteVrednost(Tabeli[Tabela].Trim(), ID, ID2);
                        }
                        break;
                    };
                case 7:
                    {
                        String[] pom = new String[6];
                        pom[0] = Red[0];
                        pom[1] = Red[1];
                        pom[2] = Red[4];
                        pom[3] = Red[2];
                        pom[4] = Red[3];
                        pom[5] = Red[5];
                        if (daliBrisi == false)
                        {
                            rez = administrator.Zacuvaj(Tabeli[Tabela].Trim(), pom);
                        }
                        else
                        {
                            rez = administrator.DeleteVrednost(Tabeli[Tabela].Trim(), ID, ID2);
                        }
                        break;
                    };
                case 10:
                    {
                        String[] pom = new String[2];
                        pom[0] = Red[0];
                        pom[1] = Red[Red.Length-1];
                        if (daliBrisi == false)
                        {
                            rez = administrator.Zacuvaj(Tabeli[Tabela].Trim(), pom);
                        }
                        else
                        {
                            rez = administrator.DeleteVrednost(Tabeli[Tabela].Trim(), ID, ID2);
                        }
                        break;
                    };
                case 11:
                    {
                        String[] pom = new String[3];
                        pom[0] = Red[0];
                        pom[1] = Red[1];
                        pom[2] = Red[Red.Length - 1];
                        if (daliBrisi == false)
                        {
                            rez = administrator.Zacuvaj(Tabeli[Tabela].Trim(), pom);
                        }
                        else
                        {
                            rez = administrator.DeleteVrednost(Tabeli[Tabela].Trim(), ID, ID2);
                        }
                        break;
                    };
                case 12:
                    {
                        String[] pom = new String[3];
                        pom[0] = Red[0];
                        pom[1] = Red[1];
                        pom[2] = Red[Red.Length - 1];
                        if (daliBrisi == false)
                        {
                            rez = administrator.Zacuvaj(Tabeli[Tabela].Trim(), pom);
                        }
                        else
                        {
                            rez = administrator.DeleteVrednost(Tabeli[Tabela].Trim(), ID, ID2);
                        }
                        break;
                    };
                case 13:
                case 14:
                    {
                        rez = "Не е дозволено изменување на овие табели.";
                        break;
                    };
                    default:

                    break;
            }
        }
        catch (Exception ex)
        {

            Response.Write(ex.Message);
        }

        return rez;
    }
    //protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
    //{
    //    GridView1.EditIndex = e.NewEditIndex;
    //    GridView1.EditRowStyle.BackColor = System.Drawing.Color.Azure;
    //    GridView1.DataBind();
    //    try
    //    {

    //        // GridView1.Rows[GridView1.EditIndex].Cells[0].Controls.Clear();
    //        //LiteralControl link = new LiteralControl("<a href=\"Admin.aspx?A=4&T=" + Tabela + "&R=" + ((TextBox)GridView1.Rows[GridView1.EditIndex].Cells[2].Controls[0]).Text.Trim() + "\">Zacuvaj</a>");
    //        //  GridView1.Rows[GridView1.EditIndex].Cells[0].Controls.Add(link);

    //        // Lista od tabelite , samo za pregled , pogore se difinirani !
    //        //Tabeli[1] = "Danok";
    //        //Tabeli[2] = "Grad";
    //        //Tabeli[3] = "Firma";
    //        //Tabeli[4] = "Komitent";
    //        //Tabeli[5] = "Korisnik";
    //        //Tabeli[6] = "Lokacija";
    //        //Tabeli[7] = "Proizvod";
    //        //Tabeli[8] = "TipCena";
    //        //Tabeli[9] = "TipProizvod";
    //        //Tabeli[10] = "Naracka";
    //        //Tabeli[11] = "EdinecnaCena";
    //        //Tabeli[12] = "GrupnaCena";
    //        //Tabeli[13] = "ZiroSmetka";
    //        //Tabeli[14] = "ListaProizvodi";

    //        switch (Tabela)
    //        {
    //            case 1:
    //            case 2:
    //            case 3:
    //            case 8:
    //            case 9:
    //            case 13:
    //                {
    //                    GridView1.Rows[GridView1.EditIndex].Cells[2].Enabled = false;
    //                    break;
    //                };
    //            case 4:
    //                {
    //                    GridView1.Rows[GridView1.EditIndex].Cells[2].Enabled = false;
    //                    GridView1.Rows[GridView1.EditIndex].Cells[7].Enabled = false;

    //                    break;
    //                };
    //            case 5:
    //                {
    //                    lista = new DropDownList(); //Lista za D/N dali e aktvien user
    //                    lista.ID = "listaDN";
    //                    lista.Items.Add("D");
    //                     lista.Items.Add("N");

    //                    string vrednost = ((TextBox)GridView1.Rows[GridView1.EditIndex].Cells[9].Controls[0]).Text.Trim();
    //                    lista.SelectedIndex = lista.Items.IndexOf(lista.Items.FindByValue(vrednost));

    //                    GridView1.Rows[GridView1.EditIndex].Cells[2].Enabled = false;
    //                    GridView1.Rows[GridView1.EditIndex].Cells[3].Enabled = false;
    //                   GridView1.Rows[GridView1.EditIndex].Cells[9].Controls.Clear();
    //                    GridView1.Rows[GridView1.EditIndex].Cells[9].Controls.Add(lista);
    //                    break;
    //                };
    //            case 6:
    //                {
    //                    GridView1.Rows[GridView1.EditIndex].Cells[2].Enabled = false;
    //                    GridView1.Rows[GridView1.EditIndex].Cells[3].Enabled = false;
    //                    GridView1.Rows[GridView1.EditIndex].Cells[4].Enabled = false;
    //                    break;
    //                };
    //            case 7:
    //                {

    //                    string vrednost = ((TextBox)GridView1.Rows[GridView1.EditIndex].Cells[4].Controls[0]).Text.Trim();

    //                    DropDownList listaTipProizvod = new DropDownList();
    //                    listaTipProizvod.Items.AddRange(administrator.ListaParovi(Tabeli[9]).ToArray());
    //                    listaTipProizvod.SelectedIndex = listaTipProizvod.Items.IndexOf(listaTipProizvod.Items.FindByText(vrednost));

    //                    vrednost = ((TextBox)GridView1.Rows[GridView1.EditIndex].Cells[6].Controls[0]).Text.Trim();
    //                    DropDownList listaDanok = new DropDownList();
    //                    listaDanok.Items.AddRange(administrator.ListaParovi(Tabeli[1]).ToArray());
    //                    listaDanok.SelectedIndex = listaDanok.Items.IndexOf(listaDanok.Items.FindByText(vrednost));

    //                    GridView1.Rows[GridView1.EditIndex].Cells[2].Enabled = false;
    //                    GridView1.Rows[GridView1.EditIndex].Cells[4].Controls.Clear();
    //                    GridView1.Rows[GridView1.EditIndex].Cells[7].Controls.Clear();
    //                    GridView1.Rows[GridView1.EditIndex].Cells[4].Controls.Add(listaTipProizvod);
    //                    GridView1.Rows[GridView1.EditIndex].Cells[7].Controls.Add(listaDanok);
    //                    break;
    //                };
    //            case 10:
    //                {
    //                    for (int i = 2; i < GridView1.Rows[GridView1.EditIndex].Cells.Count; i++)
    //                    {
    //                        GridView1.Rows[GridView1.EditIndex].Cells[i].Enabled = false;
    //                    }
    //                    GridView1.Rows[GridView1.EditIndex].Cells[9].Enabled = true;
    //                    break;
    //                };
    //            case 11:
    //                {

    //                    string vrednost = ((TextBox)GridView1.Rows[GridView1.EditIndex].Cells[4].Controls[0]).Text.Trim();
    //                    DropDownList listaVidCena = new DropDownList();
    //                    listaVidCena.Items.AddRange(administrator.ListaParovi(Tabeli[8]).ToArray());
    //                    listaVidCena.SelectedIndex = listaVidCena.Items.IndexOf(listaVidCena.Items.FindByText(vrednost));

    //                    GridView1.Rows[GridView1.EditIndex].Cells[4].Controls.Clear();
    //                    GridView1.Rows[GridView1.EditIndex].Cells[4].Controls.Add(listaVidCena);
    //                    GridView1.Rows[GridView1.EditIndex].Cells[2].Enabled = false;
    //                    GridView1.Rows[GridView1.EditIndex].Cells[3].Enabled = false;
    //                    break;
    //                };
    //            case 12:
    //                {
    //                    string vrednost = ((TextBox)GridView1.Rows[GridView1.EditIndex].Cells[4].Controls[0]).Text.Trim();
    //                    DropDownList listaVidCena = new DropDownList();
    //                    listaVidCena.Items.AddRange(administrator.ListaParovi(Tabeli[8]).ToArray());
    //                    listaVidCena.SelectedIndex = listaVidCena.Items.IndexOf(listaVidCena.Items.FindByText(vrednost));

    //                    GridView1.Rows[GridView1.EditIndex].Cells[4].Controls.Clear();
    //                    GridView1.Rows[GridView1.EditIndex].Cells[4].Controls.Add(listaVidCena);
    //                    GridView1.Rows[GridView1.EditIndex].Cells[2].Enabled = false;
    //                    GridView1.Rows[GridView1.EditIndex].Cells[3].Enabled = false;
    //                    break;
    //                };
    //            default://Ze zabranuva edit sekade!
    //                foreach (TableCell item in GridView1.Rows[GridView1.EditIndex].Cells)
    //                {
    //                    item.Enabled = false;
    //                }
    //                break;
    //        }
    //    }
    //    catch (Exception ex)
    //    {

    //        Response.Write(ex.Message);
    //    }
    //}
}
