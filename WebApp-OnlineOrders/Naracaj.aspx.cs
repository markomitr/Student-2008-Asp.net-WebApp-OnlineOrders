using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Web.Configuration;

public partial class Naracaj : System.Web.UI.Page
{
    NarackaClass nar;
    String[] reklami;
    int RandomNumber;
    Random RandomClass;
    String oradb;
    OracleConnection OCon;
    OracleCommand OCom;
    protected void Page_Load(object sender, EventArgs e)
    {

        EventHandler ins = new EventHandler(this.insertBtn_Click);
        EventHandler del = new EventHandler(this.brisiBtn_Click);

            Dictionary<String, String> pomosna = (Dictionary<String, String>)Session["Korisnik"];
            if (pomosna != null)
            {
                if (pomosna["Vid"].Trim() != "Korisnik")
                {
                    Response.Redirect("Default.aspx");
                }
                else
                {
                    defaultDiv.Visible = false;
                    if (Request.QueryString["Mod"] != null && Request.QueryString["A"] != null)
                    {

                        nar = new NarackaClass(this, this.vnesiUP, this.podatociUP, this.narackaListaUP,ins,del);
                        if (Request.QueryString["mod"].ToString() == "1" && Request.QueryString["A"].ToString() == "1")
                        {
                            vnesiUP.ContentTemplateContainer.Controls.Add(nar.vratiNapraviNaracka());
                            this.Session.Remove("BarajNar");
                            this.Session.Remove("Vnes");
                        }
                        else if (Request.QueryString["mod"].ToString() == "1" && Request.QueryString["A"].ToString() == "2")
                        {

                            vnesiUP.ContentTemplateContainer.Controls.Add(nar.vratiBarajProizvod());
                            if (Session["Vnes"] != null)
                            {
                                String[] vnes = (String[])Session["Vnes"];
                                TextBox pom = (TextBox)vnesiUP.FindControl("imeTextBox");
                                DropDownList pom1 = (DropDownList)vnesiUP.FindControl("grupaProizvodiLista");
                                pom.Text = vnes[0].ToString();
                                pom1.SelectedIndex = Convert.ToInt16(vnes[1].ToString());
                            }

                            nar.nacrtajTabela();
                            nar.nacrtajTabelaLista();
                            this.Session.Remove("BarajNar");

                        }
                        else if (Request.QueryString["mod"].ToString() == "2" && Request.QueryString["A"].ToString() == "1")
                        {
                            this.Session.Remove("Vnes");
                            vnesiUP.ContentTemplateContainer.Controls.Add(nar.vratiBarajNaracka());
                            nar.nacartajTabelaNarackiNapraveni();
                        }
                        else if (Request.QueryString["mod"].ToString() == "2" && Request.QueryString["A"].ToString() == "2")
                        {
                            this.Session.Remove("BarajNar");
                            nar.nacrtajListaProizvodiIzvestaj();
                        }
                        else if (Request.QueryString["mod"].ToString() == "2" && Request.QueryString["A"].ToString() == "3")
                        {
                            this.Session.Remove("BarajNar");
                            nar.nacrtajTabelaPopust();                            
                        }
                    }
                    else
                    {
                        this.Session.Remove("BarajNar");
                        this.Session.Remove("Vnes");
                        NarackaClass pom = new NarackaClass();
                        pom.iscitiStaticni();
                        defaultDiv.Visible = true;

                    }
                }
            }           

                this.reklamiNarackiDiv.Controls.Clear();
                this.nacrtajReklami();      
    }
    void nacrtajReklami()
    {
        int i = 1;
        if (Session["Korisnik"] != null)
        {
        
            this.Master.podatociFirma = (Dictionary<String, String>)(Session["Korisnik"]);
            try
            {
                oradb = WebConfigurationManager.ConnectionStrings["Tunel"].ToString();
                String komanda = "Select p.ime as IME,tipcena.Ime as Tip,p.Cena as Cena,VratiCena(p.id," + this.Master.podatociFirma["KomitentId"].ToString() + ") as CenaP,Sliki.Slika_URL as URL from edinecnacena e  join komitent on e.Komitent_ID=komitent.id  join tipcena on tipcena.id=e.tipcena_id join proizvod p on p.id = e.proizvod_id join sliki on p.id = sliki.proizvod_id where komitent.id=" + this.Master.podatociFirma["KomitentId"].ToString();
                OCon = new OracleConnection(oradb);
                OCom = new OracleCommand(komanda, OCon);
                OracleDataReader Odr;
                OCon.Open();
                Odr = OCom.ExecuteReader();
                this.reklamiNarackiDiv.Controls.AddAt(0, new LiteralControl("<div id=\"slikiReklami\"  class=\"nar\">"));
                if (Odr.HasRows)
                {

                    while (Odr.Read())
                    {
                        String info = "<div class=\"hide\" id=\"" + i.ToString() + "\">Производ:" + Odr["Ime"].ToString() + "<br/>ТипЦена:" + Odr["Tip"].ToString() + "<br/>Цена регуларна:" + Odr["Cena"].ToString() + ",00ден<br/>Цена крајна:" + Odr["CenaP"].ToString() + ",00ден</div>";
                        this.proba.ContentTemplateContainer.Controls.Add(new LiteralControl(info));
                        this.reklamiNarackiDiv.Controls.Add(new LiteralControl("<img onmouseout=\"iscisti(this.alt)\" onmouseover=\"prikazi(this.alt)\" alt=\"" + i.ToString() + "\" src=\"" + Odr["URL"].ToString() + " \" width=\"150px\" height=\"550px\" />"));
                        i++;
                    }

                }
                else
                {
                    komanda = "Select p.ime as Ime,p.Cena as Cena,s.Slika_URL as URL,VratiCena(p.id," + this.Master.podatociFirma["KomitentId"].ToString() + ") as CenaP from sliki s join proizvod p on p.id = s.proizvod_id";
                    OCom = new OracleCommand(komanda, OCon);
                    Odr = OCom.ExecuteReader();
                    while (Odr.Read())
                    {
                        String info = "<div class=\"hide\" id=\"" + i.ToString() + "\">Производ:" + Odr["Ime"].ToString() + "<br/>Цена регуларна:" + Odr["Cena"].ToString() + ",00ден<br/>Цена крајна:" + Odr["CenaP"].ToString() + ",00ден</div>";
                        this.proba.ContentTemplateContainer.Controls.Add(new LiteralControl(info));
                        this.reklamiNarackiDiv.Controls.Add(new LiteralControl("<img onmouseout=\"iscisti(this.alt)\" onmouseover=\"prikazi(this.alt)\" alt=\"" + i.ToString() + "\" src=\"" + Odr["URL"].ToString() + " \" width=\"150px\" height=\"550px\" />"));
                        i++;
                    }
                }
                this.reklamiNarackiDiv.Controls.AddAt(this.reklamiNarackiDiv.Controls.Count, new LiteralControl("</div>"));
                OCon.Close();
            }
            catch (Exception ex)
            {
                OCon.Close();
                Response.Write("Sliki Baza " + ex.Message);
            }
        }

    }
    void insertBtn_Click(object sender, EventArgs e)
    {
        String[] pomString = ((Button)sender).ID.ToString().Split('.');
        int red = Convert.ToInt32(pomString[2].ToString());
        int tab = Convert.ToInt32(pomString[1].ToString());
        Table[] tabNa = nar.tabelaProiz();
        TextBox pom = ((TextBox)tabNa[tab].Rows[red].FindControl("kolicinaTextBox" + (red).ToString()));
        try
        {
            oradb = WebConfigurationManager.ConnectionStrings["Tunel"].ToString();
            OCon = new OracleConnection(oradb);
            String komanda;
            String narackaId = nar.vratinarackaId();
            OCon.Open();
            komanda = "Select Kolicina from listaproizvodi where naracka_id=" + narackaId.ToString().Trim() + " and proizvod_id=" + tabNa[tab].Rows[red].Cells[1].Text.ToString().Trim();
            OracleCommand OCom = new OracleCommand(komanda, OCon);
            int kol;
            if (OCom.ExecuteScalar() != null)
            {
                kol = Convert.ToInt32(OCom.ExecuteScalar().ToString());
            }
            else
            {
                kol = -1;
            }
            if (pom.Text.ToString().Trim() != "" && Convert.ToInt64(pom.Text.ToString().Trim()) > 0)
            {
                if (kol == -1)
                {
                    komanda = "Insert into listaproizvodi(NARACKA_ID,PROIZVOD_ID,DANOK_ID,KOLICINA,CENA) values(" + narackaId.ToString().Trim() + "," + tabNa[tab].Rows[red].Cells[1].Text.ToString().Trim() + "," + tabNa[tab].Rows[red].Cells[3].Text.ToString().Trim() + "," + pom.Text.Trim().ToString() + "," + tabNa[tab].Rows[red].Cells[7].Text.ToString().Trim() + ")";
                }
                else
                {
                    kol += Convert.ToInt32(pom.Text.Trim().ToString());
                    komanda = "Update listaproizvodi set kolicina =" + kol.ToString() + " where naracka_id=" + narackaId.ToString().Trim() + " and proizvod_id=" + tabNa[tab].Rows[red].Cells[1].Text.ToString().Trim();
                }
            }
            OCom = new OracleCommand(komanda, OCon);
            OCom.ExecuteNonQuery();
            OCon.Close();
        }
        catch (Exception ex)
        {
            OCon.Close();
        }

        //UpdatePanel podatociLista = ((UpdatePanel)strana.FindControl("narackaListaUP"));
        //podatociLista.Update();
        //stranica.Response.Redirect("Naracaj.aspx?Mod=1&A=2");
        narackaListaUP.ContentTemplateContainer.Controls.Clear();
        nar.nacrtajTabelaLista();
        narackaListaUP.Update();
    }
    void brisiBtn_Click(object sender, EventArgs e)
    {

        String[] pomString = ((Button)sender).ID.ToString().Split('.');
        int red = Convert.ToInt32(pomString[2].ToString());
        int tab = Convert.ToInt32(pomString[1].ToString());
        Table[] tabLN = nar.tabelaListaNar();
        try
        {
            oradb = WebConfigurationManager.ConnectionStrings["Tunel"].ToString();
            OCon = new OracleConnection(oradb);
            String narackaId = nar.vratinarackaId();
            OCon.Open();
            String komanda = "Delete from listaproizvodi where listaproizvodi.naracka_id=" + narackaId.Trim().ToString() + " and  listaproizvodi.proizvod_id=" + tabLN[tab].Rows[red].Cells[1].Text.ToString().Trim();
            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OCom.ExecuteNonQuery();
            OCon.Close();
        }
        catch (Exception ex)
        {
            OCon.Close();
        }
        narackaListaUP.ContentTemplateContainer.Controls.Clear();
        nar.nacrtajTabelaLista();
        narackaListaUP.Update();
    }
}
