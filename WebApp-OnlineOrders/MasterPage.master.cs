using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data;
public partial class MasterPage : System.Web.UI.MasterPage
{
    public string oradb;
    public Dictionary<String, String> podatoci;

    public Image SlikaKomitent
    {
        get
        {
            return this.SlikaKomin;
        }
        set
        {
            this.SlikaKomin = value;
        }
    }
    public Dictionary<String, String> podatociFirma
    {
        get { return this.podatoci; }
        set { this.podatoci = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
       oradb = WebConfigurationManager.ConnectionStrings["Tunel"].ToString();
        int rez = 0;

        if (Session["Korisnik"] != null)//Korisnikot go ima vo Sesija znaci e veke logiran. Se zemaat podatocie od sesijata.
        {
            this.LogOutLinkButton.Visible = true;
            Dictionary<String, String> podatoci = (Dictionary<String, String>)Session["Korisnik"];
            if (podatoci["Vid"].ToString() == "Admin")
            {
                SetirajAdmin();
            }
            else
            {
                SetirajKorisnik();
            }
        }
        else //Korisnikot ne e logiran
        {
            if (IsPostBack)//Znaci deka pravi Probuva da se logira .
            {
                String User = UserName.Text.Trim();
                String Pass = Password.Text.Trim();
                if (ProveriKorisnik(User, Pass, ref rez))//Takov korisnik Postoi
                {
                    //Podatocite ze zapisuvaat vo Sesion["Korisnik"] kako array od podatoci 
                    this.LogOutLinkButton.Visible = true;
                    if (rez == 1) //se raboti za admin 
                    {
                        PodatociAdmin();
                        SetirajAdmin();
                    }
                    else//Obicen korisnik
                    {
                        PodatociKorisnik(User);
                        SetirajKorisnik();
                    }

                }
                else//Podatocite za korinsikot ne se tocni . Toj ne moze da se logira .
                {
                    this.Status.InnerHtml = "Грешен Корисник !";
                }
            } //Korisnikot ne e logiran, i ne probuva da se logira.
            else
            {
                this.LogOutLinkButton.Visible = false;
                //Se zema od URL samo imeto na stranta (Defaulat.aspx) bidejki na nea se pravi Redirect.
                //Ako ne se stavi ova togas ke pravi beskonecen ciklus .
                if (Request.Url.Segments[2].ToString().ToLower() != "default.aspx")
                {
                    Response.Redirect("Default.aspx");
                }
            }
        }
     
    }
    public void SetirajAdmin()
    {
        podatoci = (Dictionary<String, String>)Session["Korisnik"];
        this.Login.Visible = false;
        this.Info.Visible = true;

        this.Podatoci.InnerHtml = "Se logira Admin";
        this.Meni.InnerHtml += "<li><a  href=\"Admin.aspx\">Администратор</a></li>";
    }
    public void SetirajKorisnik()
    {
        podatoci = (Dictionary<String, String>)Session["Korisnik"];
        this.Login.Visible = false;
        this.Info.Visible = true;
        this.Podatoci.InnerHtml = "Se logira Korisnik <br />";
        this.Podatoci.InnerHtml += "<br />" + podatoci["KorisnikId"].ToString();
        this.Podatoci.InnerHtml += "<br />" + podatoci["KorisnikIme"].ToString();
        this.Podatoci.InnerHtml += "<br />" + podatoci["Prezime"].ToString();
        this.Podatoci.InnerHtml += "<br />" + podatoci["KomitentId"].ToString();
        this.Podatoci.InnerHtml += "<br />" + podatoci["KomitentIme"].ToString();

        this.Meni.InnerHtml += "<li><a href=\"Firma.aspx\">За Фирмата</a></li>";
        this.Meni.InnerHtml += "<li><a href=\"Naracaj.aspx\">Нарачка</a></li>";
    }
    private void PodatociKorisnik(String User)
    {
        try
        {
            String komanda = "Select k.ID as KorisnikId,k.Ime as KorisnikIme,k.Prezime,t.Ime as KomitentIme,t.ID as KomitentId From Korisnik k Join Komitent t on k.Komitent_ID=t.ID where k.Username='" + User + "'";

            using (OracleConnection OCon = new OracleConnection(oradb))
            {
                OCon.Open();

                podatoci = new Dictionary<string, string>();

                OracleCommand OCom = new OracleCommand(komanda, OCon);
                OracleDataReader dr = OCom.ExecuteReader();
                dr.Read();

                podatoci.Add("Vid", "Korisnik");
                podatoci.Add("KorisnikId", dr["KorisnikId"].ToString());
                podatoci.Add("KorisnikIme", dr["KorisnikIme"].ToString());
                podatoci.Add("Prezime", dr["Prezime"].ToString());
                podatoci.Add("KomitentId", dr["KomitentId"].ToString());
                podatoci.Add("KomitentIme", dr["KomitentIme"].ToString());

                OCon.Close();

                Session["Korisnik"] = podatoci;
            }

        }
        catch (Exception ex)
        {
            this.Status.InnerHtml = "Нема Конекција со базата ! . <br />" + ex.Message;
        }
    }
    public void PodatociAdmin()
    {
        try
        {
            String komanda = "Select * From Firma";

            using (OracleConnection OCon = new OracleConnection(oradb))
            {
                OCon.Open();

               podatoci = new Dictionary<string, string>();

                OracleCommand OCom = new OracleCommand(komanda, OCon);
                OracleDataReader dr = OCom.ExecuteReader();
                dr.Read();
                podatoci.Add("Vid","Admin");
                podatoci.Add("ID",dr["ID"].ToString());
                podatoci.Add("ImeFirma",dr["Ime"].ToString());
                podatoci.Add("UserName",dr["Username"].ToString());
                OCon.Close();

                Session["Korisnik"] = podatoci;
            }

        }
        catch (Exception ex)
        {
            this.Status.InnerHtml = "Нема Конекција со базата ! . <br />" + ex.Message;
        }
    }
    public Boolean ProveriKorisnik(string user,string pass,ref int rez)
    {
        //Funkcija za proverka dali korisnikot postoi vo bazata.Vraka true ako korinikot vnesol validni podatoci.
        //SQL funkcijata vraka integer i toa :
        // 1  - ako korisnikot e admin , a 2-ako korisnikot e nekoj korisnik od komitentite

        try
        {

            String komanda = "Select ProveriKorisnik('" + user + "','" + pass + "') from dual" ;
          
            using (OracleConnection  OCon = new OracleConnection(oradb))
            {
                OCon.Open();

                OracleCommand OCom = new OracleCommand(komanda, OCon);
                OracleDataReader dr = OCom.ExecuteReader();
                dr.Read();
                rez = Convert.ToInt32(dr[0].ToString());
                OCon.Close();

                //if (rez == 1)
                //{
                //    Response.Write("Admin se logira !");
                //}
                //else if ((rez == 2)) Response.Write("Korisnik se logira !");
            }

        }
        catch (Exception ex)
        {
            this.Status.InnerHtml = "Нема Конекција со базата ! . <br />" + ex.Message;
            return false;
        }

        if (rez  != 0 ) return true;
        else return false;
    }
    protected void Logout(object sender, EventArgs e)
    {
        if (Session["Korisnik"] != null)
        {
            Session.Remove("Korisnik");
            //Response.Redirect("Default.aspx");
        }
        Server.Transfer("Default.aspx");
    }
}
