using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data;
using System.Web.Configuration;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;



public class NarackaClass
{
    // za DataBase
    String oradb;
    OracleConnection OCon;

    //Za Class
    String komitentId;
    String korisnikId;
    String komitentIme;
    String korisnikIme;
    String narackaId;
    Table tabela;
    Table[] tabelaN;
    Table[] tabelaListaN;
    Table[] tabelaNaracki;
    static int tabP;
    static int tabLN;
    static int tabNN;

    //Ze WebStrana
    Page strana;
    Page stranica;
    UpdatePanel vnes;
    UpdatePanel podatoci;
    UpdatePanel podatociLista;
    Dictionary<String, String> info;
    EventHandler insertKopce;
    EventHandler delKopce;

    public NarackaClass()
    {
        oradb = WebConfigurationManager.ConnectionStrings["Tunel"].ToString();
        OCon = new OracleConnection(oradb);
    }
    public NarackaClass(Page prefrlena, UpdatePanel vnesS, UpdatePanel podatociS, UpdatePanel podatociListaS, EventHandler insert, EventHandler delete)
    {
        oradb = WebConfigurationManager.ConnectionStrings["Tunel"].ToString();
        OCon = new OracleConnection(oradb);
        strana = prefrlena;
        stranica = prefrlena;
        vnes = vnesS;
        podatoci = podatociS;
        podatociLista = podatociListaS;
        if (stranica.Session["Korisnik"] != null)
        {
            info = (Dictionary<String, String>)stranica.Session["Korisnik"];
            komitentId = info["KomitentId"].ToString();
            korisnikId = info["KorisnikId"].ToString();
            komitentIme = info["KomitentIme"].ToString();
            korisnikIme = info["KorisnikIme"].ToString();
        }
        insertKopce = insert;
        delKopce = delete;
    }
    public Table vratiNapraviNaracka()
    {
        tabela = new Table();
        tabela.BorderStyle = BorderStyle.Solid;
        tabela.CellPadding = 5;
        for (int i = 1; i < 4; i++)
        {
            TableRow row = new TableRow();
            for (int j = 0; j < 6; j++)
            {
                TableCell cell = new TableCell();
                if (i == 3 && j == 1)
                {
                    cell.ColumnSpan = 3;
                    row.Controls.Add(cell);
                    break;
                }
                row.Controls.Add(cell);

            }

            tabela.Controls.Add(row);
        }
        TableCell nova = new TableCell();
        tabela.Rows[2].Controls.Add(nova);

        tabela.Rows[0].Cells[0].Text = "Комитент:";
        tabela.Rows[0].Cells[2].Text = "Корисник:";
        tabela.Rows[0].Cells[4].Text = "Датум:";
        tabela.Rows[1].Cells[0].Text = "Град";
        tabela.Rows[1].Cells[2].Text = "Локација";
        tabela.Rows[1].Cells[4].Text = "Адреса";
        tabela.Rows[2].Cells[0].Text = "Коментар";
        Label labela = new Label();
        labela.ID = "komitentIDLbl";
        labela.Text = komitentIme;
        tabela.Rows[0].Cells[1].Controls.Add(labela);
        labela = new Label();
        labela.ID = "korisnikIDLbl";
        labela.Text = korisnikIme;
        tabela.Rows[0].Cells[3].Controls.Add(labela);
        labela = new Label();
        labela.ID = "datumLbl";
        labela.Text = DateTime.Now.ToShortDateString();
        tabela.Rows[0].Cells[5].Controls.Add(labela);
        try
        {
            OCon.Open();
            String komanda = "Select distinct grad.Ime,grad.Id from lokacija join grad on grad.id = lokacija.grad_id and  lokacija.komitent_id=" + komitentId.ToString();

            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OracleDataReader dr = OCom.ExecuteReader();

            DropDownList lista = new DropDownList();
            lista.ID = "gradoviLista";
            lista.AutoPostBack = true;
            lista.SelectedIndexChanged += new EventHandler(listaGradovi_SelectedIndexChanged);
            while (dr.Read())
            {
                ListItem pom = new ListItem(dr["Ime"].ToString(), dr["ID"].ToString());
                lista.Items.Add(pom);
            }
            tabela.Rows[1].Cells[1].Controls.Add(lista);
            komanda = "Select distinct lokacija.Ime,lokacija.Id,lokacija.Adresa from lokacija where lokacija.grad_id = (Select id  from grad where ime='" + lista.Items[0].Text.ToString() + "') and  lokacija.komitent_id=" + komitentId;
            OCom = new OracleCommand(komanda, OCon);
            dr = OCom.ExecuteReader();

            lista = new DropDownList();
            lista.ID = "lokaciiLista";
            lista.AutoPostBack = true;
            lista.SelectedIndexChanged += new EventHandler(listaLokacii_SelectedIndexChanged);
            while (dr.Read())
            {
                ListItem pom = new ListItem(dr["Ime"].ToString(), dr["ID"].ToString());
                lista.Items.Add(pom);
            }
            tabela.Rows[1].Cells[3].Controls.Add(lista);
            komanda = "Select adresa from lokacija where lokacija.id=" + lista.Items[0].Value.ToString();
            OCom = new OracleCommand(komanda, OCon);
            labela = new Label();
            labela.ID = "adresaLbl";
            labela.Text = OCom.ExecuteScalar().ToString();
            tabela.Rows[1].Cells[5].Controls.Add(labela);
            OCon.Close();
        }
        catch (Exception ex)
        {
            OCon.Close();
        }

        Button potvrdiBtn = new Button();
        potvrdiBtn.Text = "Потврди";
        potvrdiBtn.Click += new EventHandler(potvrdiBtn_Click);
        tabela.Rows[2].Cells[2].Controls.Add(potvrdiBtn);
        TextBox komentarTexBox = new TextBox();
        komentarTexBox.ID = "komentarTexBox";
        komentarTexBox.TextMode = TextBoxMode.MultiLine;
        komentarTexBox.Width = Unit.Pixel(300);
        komentarTexBox.Rows = 3;
        komentarTexBox.MaxLength = 160;
        tabela.Rows[2].Cells[1].Controls.Add(komentarTexBox);
        return tabela;

    }
    void listaGradovi_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList lok = ((DropDownList)tabela.FindControl("lokaciiLista"));
        DropDownList grad = ((DropDownList)tabela.FindControl("gradoviLista"));
        Label pomAdresa = ((Label)tabela.FindControl("adresaLbl"));
        pomAdresa.Text = "";
        try
        {
            String komanda = "Select distinct lokacija.Ime,lokacija.Id from lokacija where lokacija.grad_id =" + grad.Items[grad.SelectedIndex].Value.ToString() + " and  lokacija.komitent_id=" + komitentId;

            OCon.Open();

            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OracleDataReader dr = OCom.ExecuteReader();
            lok.Items.Clear();
            while (dr.Read())
            {
                ListItem pom = new ListItem(dr["Ime"].ToString(), dr["ID"].ToString());
                lok.Items.Add(pom);
            }
            OCon.Close();
        }
        catch (Exception ex)
        {
            OCon.Close();
        }
        this.listaLokacii_SelectedIndexChanged(lok, new EventArgs());

    }
    void listaLokacii_SelectedIndexChanged(object sender, EventArgs e)
    {
        Label pom = ((Label)tabela.FindControl("adresaLbl"));
        try
        {
            String komanda = "Select adresa from lokacija where lokacija.id=" + ((DropDownList)sender).Items[((DropDownList)sender).SelectedIndex].Value.ToString();

            OCon.Open();

            OracleCommand OCom = new OracleCommand(komanda, OCon);
            String adresa = OCom.ExecuteScalar().ToString();
            pom.Text = adresa;
            OCon.Close();
        }
        catch (Exception ex)
        {
            OCon.Close();
        }

    }
    void potvrdiBtn_Click(object sender, EventArgs e)
    {
        DropDownList lok = ((DropDownList)tabela.FindControl("lokaciiLista"));
        TextBox kom = ((TextBox)tabela.FindControl("komentarTexBox"));
        try
        {
            OCon.Open();
            String komanda = "Insert into naracka(komitent_id,korisnik_id,lokacija_id,datumnaracka,komentar) values(" + komitentId.Trim() + "," + korisnikId.Trim() + "," + lok.Items[lok.SelectedIndex].Value.ToString().Trim() + ",to_Date('" + DateTime.Now.ToShortDateString() + "','mm/dd/yyyy'),'" + kom.Text.ToString().Trim() + "')";
            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OCom.ExecuteNonQuery();
            komanda = "Select id from naracka where naracka.komitent_id=" + komitentId.Trim().ToString() + " and naracka.korisnik_id=" + korisnikId.Trim() + " and naracka.lokacija_id=" + lok.Items[lok.SelectedIndex].Value.ToString().Trim() + " order by id DESC";
            OCom = new OracleCommand(komanda, OCon);
            narackaId = OCom.ExecuteScalar().ToString();
            if (info.ContainsKey("NarackaId"))
            {
                info["NarackaId"] = narackaId;
            }
            else
            {
                info.Add("NarackaId", narackaId);
            }
            stranica.Session["Korisnik"] = info;

        }
        catch (Exception ex)
        {
            OCon.Close();

        }
        stranica.Response.Redirect("Naracaj.aspx?Mod=1&A=2");

    }
    public Table vratiBarajProizvod()
    {
        tabela = new Table();
        tabela.BorderStyle = BorderStyle.Solid;
        tabela.CellPadding = 5;

        for (int i = 1; i < 3; i++)
        {
            TableRow row = new TableRow();
            for (int j = 0; j < 5; j++)
            {
                TableCell cell = new TableCell();
                row.Controls.Add(cell);

            }

            tabela.Controls.Add(row);
        }

        tabela.Rows[0].Cells[0].Text = "Производ Име:";
        tabela.Rows[1].Cells[0].Text = "Производ Група:";

        TextBox imeTexBox = new TextBox();
        imeTexBox.ID = "imeTextBox";
        tabela.Rows[0].Cells[1].Controls.Add(imeTexBox);

        try
        {
            String komanda = "Select id,ime from tipproizvod";

            OCon.Open();

            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OracleDataReader dr = OCom.ExecuteReader();
            DropDownList lista = new DropDownList();
            lista.ID = "grupaProizvodiLista";
            lista.Items.Add(new ListItem("Сите", "0"));
            while (dr.Read())
            {
                ListItem pom = new ListItem(dr["Ime"].ToString(), dr["ID"].ToString());
                lista.Items.Add(pom);
            }
            tabela.Rows[1].Cells[1].Controls.Add(lista);
            OCon.Close();
        }
        catch (Exception ex)
        {
            OCon.Close();
        }
        Button barajBtn = new Button();
        barajBtn.Text = "Барај";
        barajBtn.Click += new EventHandler(barajBtn_Click);
        tabela.Rows[1].Cells[2].Controls.Add(barajBtn);
        tabela.Rows[0].Cells[3].Text = "   ";
        tabela.Rows[1].Cells[3].Text = "   ";
        Button zatvoriNarBtn = new Button();
        zatvoriNarBtn.Text = "Затвори Нарачка";
        zatvoriNarBtn.Click += new EventHandler(zatvoriNarBtn_Click);
        tabela.Rows[1].Cells[4].Controls.Add(zatvoriNarBtn);
        Button izbrisiNarBtn = new Button();
        izbrisiNarBtn.Text = "Избриши Нарачка";
        izbrisiNarBtn.Click += new EventHandler(izbrisiNarBtn_Click);
        tabela.Rows[1].Cells[4].Controls.Add(izbrisiNarBtn);
        Button izleziNarBtn = new Button();
        izleziNarBtn.Text = "Излези";
        izleziNarBtn.Click += new EventHandler(izleziNarBtn_Click);
        tabela.Rows[1].Cells[4].Controls.Add(izleziNarBtn);
        return tabela;
    }
    void izleziNarBtn_Click(object sender, EventArgs e)
    {
        strana.Response.Redirect("Naracaj.aspx");
    }
    void izbrisiNarBtn_Click(object sender, EventArgs e)
    {
        String narackaId = info["NarackaId"].ToString();
        try
        {
            OCon.Open();
            String komanda = "Delete from listaproizvodi where listaproizvodi.naracka_id=" + narackaId.ToString().Trim();
            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OCom.ExecuteNonQuery();
            OCon.Close();
            komanda = "Delete naracka where id=" + narackaId.ToString().Trim();
            OCom = new OracleCommand(komanda, OCon);
            OCon.Open();
            OCom.ExecuteNonQuery();
            OCon.Close();
            strana.Response.Redirect("Naracaj.aspx");
        }
        catch (Exception ex)
        {
            OCon.Close();
        }
    }
    void zatvoriNarBtn_Click(object sender, EventArgs e)
    {
        String narackaId = info["NarackaId"].ToString();

        try
        {
            OCon.Open();
            String komanda = "Update naracka set ZATVORENA='D',VKUPNACENA=VratiVkupnoNaracka(" + narackaId.ToString().Trim() + "),DATUMISPORAKA=" + "to_Date('" + DateTime.Now.AddDays(1).ToShortDateString() + "','mm/dd/yyyy')" + " where id =" + narackaId.ToString().Trim();
            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OCom.ExecuteNonQuery();
            OCon.Close();
            strana.Response.Redirect("Naracaj.aspx");
        }
        catch (Exception ex)
        {
            OCon.Close();
        }
    }
    void barajBtn_Click(object sender, EventArgs e)
    {
        String[] vrednosti = new String[2];
        tabP = 0;

        podatoci.ContentTemplateContainer.Controls.Clear();
        TextBox vnesIme = ((TextBox)vnes.FindControl("imeTextBox"));
        DropDownList vnesGrupa = ((DropDownList)vnes.ContentTemplateContainer.FindControl("grupaProizvodiLista"));
        vrednosti[0] = vnesIme.Text.ToString();
        vrednosti[1] = vnesGrupa.SelectedIndex.ToString();
        stranica.Session["Vnes"] = vrednosti;
        this.nacrtajTabela();
        podatoci.Update();
    }
    public void nacrtajTabela()
    {

        String komanda;
        String ime = "";
        String grupa = "";
        tabNN = 0;
        TextBox vnesIme = ((TextBox)vnes.FindControl("imeTextBox"));
        DropDownList vnesGrupa = ((DropDownList)vnes.FindControl("grupaProizvodiLista"));
        ime = vnesIme.Text.ToString();
        grupa = vnesGrupa.Items[vnesGrupa.SelectedIndex].Value.ToString();
        try
        {
            OCon.Open();
            if (ime != null && grupa != null)
            {
                if (ime == "" || grupa == "0")
                {
                    if (ime == "" && grupa != "0")
                    {
                        komanda = "Select proizvod.id as IDP,tipproizvod.Id as IDT,danok.id as IDD, proizvod.IME as Име,tipproizvod.IME as Група,danok.IME as Данок,vratiCena(proizvod.id,"+ komitentId.ToString().Trim() + ") as Цена  from proizvod join tipproizvod on tipproizvod.id=proizvod.tipproizvod_id and tipproizvod.id=" + grupa.Trim().ToString() + " join danok on danok.id=proizvod.danok_id order by proizvod.IME";
                    }
                    else if (ime != "" && grupa == "0")
                    {
                        komanda = "Select proizvod.id as IDP,tipproizvod.Id as IDT,danok.id as IDD, proizvod.IME as Име,tipproizvod.IME as Група,danok.IME as Данок,vratiCena(proizvod.id," + komitentId.ToString().Trim() + ")  as Цена  from proizvod join tipproizvod on tipproizvod.id=proizvod.tipproizvod_id join danok on danok.id=proizvod.danok_id where proizvod.ime like '%" + ime.ToString().Trim() + "%' order by proizvod.IME";
                    }
                    else
                    {
                        komanda = "Select proizvod.id as IDP,tipproizvod.Id as IDT,danok.id as IDD, proizvod.IME as Име,tipproizvod.IME as Група,danok.IME as Данок,vratiCena(proizvod.id," + komitentId.ToString().Trim() + ")  as Цена  from proizvod join tipproizvod on tipproizvod.id=proizvod.tipproizvod_id join danok on danok.id=proizvod.danok_id order by tipproizvod.IME";
                    }
                }
                else
                {
                    komanda = "Select proizvod.id as IDP,tipproizvod.Id as IDT,danok.id as IDD, proizvod.IME as Име,tipproizvod.IME as Група,danok.IME as Данок,vratiCena(proizvod.id," + komitentId.ToString().Trim() + ")  as Цена  from proizvod join tipproizvod on tipproizvod.id=proizvod.tipproizvod_id join danok on danok.id=proizvod.danok_id where proizvod.Ime like '%" + ime.ToString().Trim() + "%' and tipproizvod.id=" + grupa.ToString().Trim() + " order by tipproizvod.IME,proizvod.Ime";
                }
            }
            else
            {
                komanda = "Select proizvod.id as IDP,tipproizvod.Id as IDT,danok.id as IDD, proizvod.IME as Име,tipproizvod.IME as Група,danok.IME as Данок,vratiCena(proizvod.id," + komitentId.ToString().Trim() + ")  as Цена  from proizvod join tipproizvod on tipproizvod.id=proizvod.tipproizvod_id join danok on danok.id=proizvod.danok_id order by tipproizvod.IME";
            }
            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OracleDataReader Odr = OCom.ExecuteReader();
            tabelaN = vratiTabelaProizvodi(Odr);
            OCon.Close();
            podatoci.ContentTemplateContainer.Controls.Add(tabelaN[tabP]);
        }
        catch (Exception ex)
        {

            OCon.Close();
        }

    }
    public Table[] vratiTabelaProizvodi(OracleDataReader drO)
    {
        int i = 1;
        int j = 0;
        int pat = 1;
        Table[] nova = new Table[100];
        TableHeaderCell head;
        TableCell cel;
        TableRow row = new TableRow();
        TableFooterRow foot;


        nova[j] = new Table();
        while (drO.Read())
        {
            if (pat == 1)
            {
                //nova[j] = new Table();
                head = new TableHeaderCell();
                head.Text = "Бр";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Производ_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Attributes.Add("class", "hide");
                head.Text = "Тип_ID";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Attributes.Add("class", "hide");
                head.Text = "Данок_ID";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Име";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Група";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Данок";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Цена";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Колинчина";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Потврди";
                row.Controls.Add(head);
                nova[j].Controls.Add(row);
                pat = 2;

            }
            if (i > 10)
            {
                i = 1;
                j++;
                row = new TableRow();
                nova[j] = new Table();
                head = new TableHeaderCell();
                head.Text = "Бр";
                head.Attributes.Add("class", "hide");
                head = new TableHeaderCell();
                head.Text = "Производ_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Тип_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Данок_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Име";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Група";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Данок";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Цена";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Колинчина";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Потврди";
                row.Controls.Add(head);
                nova[j].Controls.Add(row);
            }
            row = new TableRow();
            cel = new TableCell();
            cel.Text = i.ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDP"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDT"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDD"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Име"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Група"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Данок"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Цена"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            TextBox kolicinaTextBox = new TextBox();
            kolicinaTextBox.Text = "0";
            kolicinaTextBox.Width = Unit.Pixel(70);
            kolicinaTextBox.Style.Add("text-align", "right");
            kolicinaTextBox.Attributes.Add("onfocus", "proveriVlez(this)");
            kolicinaTextBox.Attributes.Add("onblur", "proveriIzlez(this)");
            kolicinaTextBox.ID = "kolicinaTextBox" + i.ToString();
            cel.Controls.Add(kolicinaTextBox);
            row.Controls.Add(cel);
            cel = new TableCell();
            Button insertBtn = new Button();
            insertBtn.Text = "Додади";
            insertBtn.ID = "dodadiBtn." + j.ToString() + "." + i.ToString();
            insertBtn.Click += insertKopce;
            cel.Controls.Add(insertBtn);
            row.Controls.Add(cel);
            nova[j].Controls.Add(row);
            i++;
        }
        Table[] kraj = new Table[j + 1];
        if (nova[0].Rows.Count > 0)
        {
            for (int w = 0; w <= j; w++)
            {
                foot = new TableFooterRow();
                cel = new TableCell();
                cel.ColumnSpan = 5;
                for (int q = 1; q <= j + 1; q++)
                {
                    Button kopce = new Button();
                    kopce.ID = q.ToString();
                    kopce.Click += new EventHandler(kopce_Click);
                    kopce.Text = q.ToString();
                    cel.Controls.Add(kopce);
                }
                foot.Controls.Add(cel);
                if (nova[w] != null)
                    nova[w].Rows.Add(foot);
                kraj[w] = nova[w];
            }
        }
        else
        {
            foot = new TableFooterRow();
            cel = new TableCell();
            cel.Text = " Нема такви  податоци во производи ";
            cel.BorderColor = System.Drawing.Color.White;
            cel.BorderWidth = Unit.Pixel(2);
            cel.BorderStyle = BorderStyle.Solid;
            foot.Controls.Add(cel);
            nova[0].Rows.Add(foot);
            kraj[0] = nova[0];
        }
        return kraj;
    }
    void insertBtn_Click(object sender, EventArgs e)
    {
        String[] pomString = ((Button)sender).ID.ToString().Split('.');
        int red = Convert.ToInt32(pomString[2].ToString());
        int tab = Convert.ToInt32(pomString[1].ToString());
        TextBox pom = ((TextBox)tabelaN[tab].Rows[red].FindControl("kolicinaTextBox" + (red).ToString()));
        try
        {
            String komanda;
            String narackaId = info["NarackaId"].ToString();
            OCon.Open();
            komanda = "Select Kolicina from listaproizvodi where naracka_id=" + narackaId.ToString().Trim() + " and proizvod_id=" + tabelaN[tab].Rows[red].Cells[1].Text.ToString().Trim();
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
                    komanda = "Insert into listaproizvodi(NARACKA_ID,PROIZVOD_ID,DANOK_ID,KOLICINA,CENA) values(" + narackaId.ToString().Trim() + "," + tabelaN[tab].Rows[red].Cells[1].Text.ToString().Trim() + "," + tabelaN[tab].Rows[red].Cells[3].Text.ToString().Trim() + "," + pom.Text.Trim().ToString() + "," + tabelaN[tab].Rows[red].Cells[7].Text.ToString().Trim() + ")";
                }
                else
                {
                    kol += Convert.ToInt32(pom.Text.Trim().ToString());
                    komanda = "Update listaproizvodi set kolicina =" + kol.ToString() + " where naracka_id=" + narackaId.ToString().Trim() + " and proizvod_id=" + tabelaN[tab].Rows[red].Cells[1].Text.ToString().Trim();
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
        podatociLista.Update();
        stranica.Response.Redirect("Naracaj.aspx?Mod=1&A=2");
    }
    void kopce_Click(object sender, EventArgs e)
    {
        tabP = Convert.ToInt32(((Button)sender).ID.ToString()) - 1;
        podatoci.ContentTemplateContainer.Controls.Clear();
        podatoci.ContentTemplateContainer.Controls.Add(tabelaN[Convert.ToInt32(((Button)sender).ID.ToString()) - 1]);
    }
    public void nacrtajTabelaLista()
    {
        //Table[] tabelaN;
        String komanda;

        try
        {
            String narackaId = info["NarackaId"].ToString();
            OCon.Open();
            komanda = "Select proizvod.id as IDP,tipproizvod.Id as IDT,danok.id as IDD, proizvod.IME as Име,tipproizvod.ime as Група,danok.IME as Данок,listaproizvodi.cena as Цена,listaproizvodi.kolicina as Количина from listaproizvodi join proizvod on proizvod.id=listaproizvodi.proizvod_id join naracka on naracka.id=listaproizvodi.naracka_id join danok on danok.id=listaproizvodi.danok_id join tipproizvod on tipproizvod.id=proizvod.tipproizvod_id and naracka.id=" + narackaId.ToString().Trim();
            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OracleDataReader Odr = OCom.ExecuteReader();
            tabelaListaN = vratiTabelaNaracka(Odr);
            OCon.Close();
            podatociLista.ContentTemplateContainer.Controls.Add(tabelaListaN[tabLN]);
        }
        catch (Exception ex)
        {

            OCon.Close();
        }

    }
    public Table[] vratiTabelaNaracka(OracleDataReader drO)
    {
        int i = 1;
        int j = 0;
        int pat = 1;
        int vkupno = 0;
        Table[] nova = new Table[100];
        TableHeaderCell head;
        TableCell cel;
        TableRow row = new TableRow();
        TableFooterRow foot;
        nova[j] = new Table();
        while (drO.Read())
        {
            if (pat == 1)
            {
                //nova[j] = new Table();
                head = new TableHeaderCell();
                head.Text = "Бр";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Производ_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Attributes.Add("class", "hide");
                head.Text = "Тип_ID";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Attributes.Add("class", "hide");
                head.Text = "Данок_ID";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Име";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Група";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Данок";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Цена";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Колинчина";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Потврди";
                row.Controls.Add(head);
                nova[j].Controls.Add(row);
                pat = 2;

            }
            if (i > 10)
            {
                i = 1;
                j++;
                row = new TableRow();
                nova[j] = new Table();
                head = new TableHeaderCell();
                head.Text = "Бр";
                head.Attributes.Add("class", "hide");
                head = new TableHeaderCell();
                head.Text = "Производ_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Тип_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Данок_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Име";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Група";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Данок";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Цена";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Колинчина";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Потврди";
                row.Controls.Add(head);
                nova[j].Controls.Add(row);
            }
            row = new TableRow();
            cel = new TableCell();
            cel.Text = i.ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDP"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDT"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDD"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Име"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Група"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Данок"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Цена"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Количина"].ToString();
            row.Controls.Add(cel);
            vkupno += Convert.ToInt32(drO["Количина"].ToString()) * Convert.ToInt32(drO["Цена"].ToString());
            cel = new TableCell();
            Button brisiBtn = new Button();
            brisiBtn.Text = "Избриши";
            brisiBtn.ID = "izbrisiBtn." + j.ToString() + "." + i.ToString();
           // brisiBtn.Click += new EventHandler(brisiBtn_Click);
            brisiBtn.Click += delKopce;
            cel.Controls.Add(brisiBtn);
            row.Controls.Add(cel);
            nova[j].Controls.Add(row);
            i++;
        }
        Table[] kraj = new Table[j + 1];
        if (nova[0].Rows.Count > 0)
        {
            for (int w = 0; w <= j; w++)
            {
                row = new TableRow();
                cel = new TableCell();
                cel.Text = "Вкупно-Нарачка:" + vkupno.ToString() + ".00 ден";
                cel.Attributes.Add("class", "textCellLevo textVkupno");
                row.Controls.Add(cel);
                nova[w].Controls.Add(row);
                foot = new TableFooterRow();
                cel = new TableCell();
                cel.ColumnSpan = 5;
                for (int q = 1; q <= j + 1; q++)
                {
                    Button kopce = new Button();
                    kopce.ID = "." + q.ToString();
                    kopce.Click += new EventHandler(kopceLista_Click);
                    kopce.Text = q.ToString();
                    cel.Controls.Add(kopce);
                }
                foot.Controls.Add(cel);
                if (nova[w] != null)
                    nova[w].Rows.Add(foot);
                kraj[w] = nova[w];
            }
        }
        else
        {
            foot = new TableFooterRow();
            cel = new TableCell();
            cel.Text = " Нарачката е празна ";
            cel.BorderColor = System.Drawing.Color.White;
            cel.BorderWidth = Unit.Pixel(2);
            cel.BorderStyle = BorderStyle.Solid;
            foot.Controls.Add(cel);
            nova[0].Rows.Add(foot);
            kraj[0] = nova[0];
        }
        return kraj;
    }
    void kopceLista_Click(object sender, EventArgs e)
    {
        String[] tabe = ((Button)sender).ID.ToString().Split('.');
        tabLN = Convert.ToInt32(tabe[1]) - 1;
        podatociLista.ContentTemplateContainer.Controls.Clear();
        podatociLista.ContentTemplateContainer.Controls.Add(tabelaListaN[tabLN]);
    }
    void brisiBtn_Click(object sender, EventArgs e)
    {

        String[] pomString = ((Button)sender).ID.ToString().Split('.');
        int red = Convert.ToInt32(pomString[2].ToString());
        int tab = Convert.ToInt32(pomString[1].ToString());
        try
        {
            String narackaId = info["NarackaId"].ToString();
            OCon.Open();
            String komanda = "Delete from listaproizvodi where listaproizvodi.naracka_id=" + narackaId.Trim().ToString() + " and  listaproizvodi.proizvod_id=" + tabelaListaN[tab].Rows[red].Cells[1].Text.ToString().Trim();
            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OCom.ExecuteNonQuery();
            OCon.Close();
        }
        catch (Exception ex)
        {
            OCon.Close();
        }

        stranica.Response.Redirect("Naracaj.aspx?Mod=1&A=2");

    }
    public void nacartajTabelaNarackiNapraveni()
    {
        tabP = 0;
        tabLN = 0;
        String komanda;

        try
        {
            DropDownList lok = ((DropDownList)vnes.FindControl("lokaciiLista"));
            DropDownList grad = ((DropDownList)vnes.FindControl("gradoviLista"));
            if (strana.Session["BarajNar"] != null)
            {
                int[] indexi =(int[])strana.Session["BarajNar"] ;
                lok.SelectedIndex = indexi[0];
                grad.SelectedIndex = indexi[1];
            }
            if (grad.SelectedIndex == 0)
            {
                komanda = "Select naracka.id as IDP,naracka.korisnik_id as IDK,lokacija.ime as Маркет,lokacija.adresa as Адреса,grad.ime as Град,naracka.DATUMNARACKA as Датум,VratiVkupnoNaracka(naracka.id) as Вкупно,naracka.platena as Платена,naracka.zatvorena as Затворена from naracka join komitent on komitent.id=naracka.komitent_id join korisnik on korisnik.id=naracka.korisnik_id join lokacija on lokacija.id=naracka.lokacija_id join grad on grad.id=lokacija.grad_id and naracka.korisnik_id=" + korisnikId.ToString().Trim() + "order by grad.Ime,lokacija.ime";
            }
            else
            {
                 komanda = "Select naracka.id as IDP,naracka.korisnik_id as IDK,lokacija.ime as Маркет,lokacija.adresa as Адреса,grad.ime as Град,naracka.DATUMNARACKA as Датум,VratiVkupnoNaracka(naracka.id) as Вкупно,naracka.platena as Платена,naracka.zatvorena as Затворена from naracka join komitent on komitent.id=naracka.komitent_id join korisnik on korisnik.id=naracka.korisnik_id join lokacija on lokacija.id=naracka.lokacija_id join grad on grad.id=lokacija.grad_id and naracka.korisnik_id=" + korisnikId.ToString().Trim() + "and lokacija.id =" + lok.Items[lok.SelectedIndex].Value.ToString().Trim() + " and grad.id=" + grad.Items[grad.SelectedIndex].Value.ToString().Trim() + " order by grad.Ime,lokacija.ime";
            }
            OCon.Open();
            
            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OracleDataReader Odr = OCom.ExecuteReader();
            tabelaNaracki = vratiTabelaNarackiKorisnik(Odr);
            OCon.Close();
            podatoci.ContentTemplateContainer.Controls.Add(tabelaNaracki[tabNN]);
        }
        catch (Exception ex)
        {

            OCon.Close();
        }

    }
    public Table vratiBarajNaracka()
    {
        tabela = new Table();
        tabela.BorderStyle = BorderStyle.Solid;
        tabela.CellPadding = 5;
        for (int i = 1; i < 4; i++)
        {
            TableRow row = new TableRow();
            for (int j = 0; j < 6; j++)
            {
                TableCell cell = new TableCell();
                if (i == 3 && j == 1)
                {
                    cell.ColumnSpan = 3;
                    row.Controls.Add(cell);
                    break;
                }
                row.Controls.Add(cell);

            }

            tabela.Controls.Add(row);
        }
        TableCell nova = new TableCell();
        tabela.Rows[2].Controls.Add(nova);

        tabela.Rows[0].Cells[0].Text = "Комитент:";
        tabela.Rows[0].Cells[2].Text = "Корисник:";
        tabela.Rows[1].Cells[0].Text = "Град";
        tabela.Rows[1].Cells[2].Text = "Локација";
        tabela.Rows[0].Cells[4].Text = "Адреса";
        Label labela = new Label();
        labela.ID = "komitentIDLbl";
        labela.Text = komitentIme;
        tabela.Rows[0].Cells[1].Controls.Add(labela);
        labela = new Label();
        labela.ID = "korisnikIDLbl";
        labela.Text = korisnikIme;
        tabela.Rows[0].Cells[3].Controls.Add(labela);
        try
        {
            OCon.Open();
            String komanda = "Select distinct grad.Ime,grad.Id from lokacija join grad on grad.id = lokacija.grad_id and  lokacija.komitent_id=" + komitentId.ToString();

            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OracleDataReader dr = OCom.ExecuteReader();

            DropDownList lista = new DropDownList();
            lista.ID = "gradoviLista";
            lista.AutoPostBack = true;
            lista.SelectedIndexChanged += new EventHandler(listaGradoviNaracka_SelectedIndexChanged);
            ListItem prv = new ListItem("Сите", "0");
            lista.Items.Add(prv);
            while (dr.Read())
            {
                ListItem pom = new ListItem(dr["Ime"].ToString(), dr["ID"].ToString());
                lista.Items.Add(pom);
            }
            tabela.Rows[1].Cells[1].Controls.Add(lista);
            komanda = "Select distinct lokacija.Ime,lokacija.Id,lokacija.Adresa from lokacija where lokacija.grad_id = (Select id  from grad where ime='" + lista.Items[0].Text.ToString() + "') and  lokacija.komitent_id=" + komitentId;
            OCom = new OracleCommand(komanda, OCon);
            dr = OCom.ExecuteReader();
            lista = new DropDownList();
            lista.ID = "lokaciiLista";
            lista.AutoPostBack = true;
            lista.SelectedIndexChanged += new EventHandler(listaLokaciiNaracka_SelectedIndexChanged);
            while (dr.Read())
            {
                ListItem pom = new ListItem(dr["Ime"].ToString(), dr["ID"].ToString());
                lista.Items.Add(pom);
            }
            tabela.Rows[1].Cells[3].Controls.Add(lista);
            labela = new Label();
            labela.ID = "adresaLbl";
            labela.Text = "          ";
            tabela.Rows[0].Cells[5].Controls.Add(labela);
            OCon.Close();
        }
        catch (Exception ex)
        {
            OCon.Close();
        }
        Button barajNarackaBtn = new Button();
        barajNarackaBtn.Text = "Барај";
        barajNarackaBtn.ID = "barajNarackaBtn";
        barajNarackaBtn.Click += new EventHandler(barajNarackaBtn_Click);
        tabela.Rows[1].Cells[5].Controls.Add(barajNarackaBtn);
        return tabela;
    }
    public Table[] vratiTabelaNarackiKorisnik(OracleDataReader drO)
    {
        int i = 1;
        int j = 0;
        int pat = 1;
        Table[] nova = new Table[100];
        TableHeaderCell head;
        TableCell cel;
        TableRow row = new TableRow();
        TableFooterRow foot;
        nova[j] = new Table();
        while (drO.Read())
        {

            if (pat == 1)
            {
                //nova[j] = new Table();
                head = new TableHeaderCell();
                head.Text = "Бр";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Производ_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Attributes.Add("class", "hide");
                head.Text = "Комитент_ID";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Маркет";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Адреса";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Град";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Датум";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Вкупно";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Платено";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Затворена";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Измени";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Прикажи";
                row.Controls.Add(head);
                nova[j].Controls.Add(row);
                pat = 2;

            }
            if (i > 10)
            {
                i = 1;
                j++;
                row = new TableRow();
                nova[j] = new Table();
                head = new TableHeaderCell();
                head.Text = "Бр";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Производ_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Attributes.Add("class", "hide");
                head.Text = "Комитент_ID";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Маркет";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Адреса";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Град";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Датум";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Вкупно";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Платена";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Затворена";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Измени";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Прегледај";
                row.Controls.Add(head);
                nova[j].Controls.Add(row);
            }
            row = new TableRow();
            cel = new TableCell();
            cel.Text = i.ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDP"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDK"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["МАРКЕТ"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["АДРЕСА"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["ГРАД"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["ДАТУМ"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["ВКУПНО"].ToString() +",00ден";
            cel.Attributes.Add("class", "textCellLevo");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["ПЛАТЕНА"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["ЗАТВОРЕНА"].ToString().Trim();
            row.Controls.Add(cel);
            cel = new TableCell();
            if (drO["ЗАТВОРЕНА"].ToString().Trim() !="D")
            {
                Button izmeniBtn = new Button();
                izmeniBtn.Text = "Измени";
                izmeniBtn.ID = "izmeniBtn." + j.ToString() + "." + i.ToString();
                izmeniBtn.Click += new EventHandler(izmeniBtn_Click);
                cel.Controls.Add(izmeniBtn);
               
            }
            row.Controls.Add(cel);
            cel = new TableCell();
            Button izvestajBtn = new Button();
            izvestajBtn.Text = "Прикажи";
            izvestajBtn.ID = "izvestajBtn." + j.ToString() + "." + i.ToString();
            izvestajBtn.Click += new EventHandler(izvestajBtn_Click);
            cel.Controls.Add(izvestajBtn);
            row.Controls.Add(cel);
            nova[j].Controls.Add(row);
            i++;
        }
        Table[] kraj = new Table[j + 1];
        //for (int w = 0; w <= j; w++)
        //{
        //    foot = new TableFooterRow();
        //    cel = new TableCell();
        //    cel.ColumnSpan = 5;
        //    for (int q = 1; q <= j + 1; q++)
        //    {
        //        Button kopce = new Button();
        //        kopce.ID =q.ToString();
        //        kopce.Click += new EventHandler(kopceListaNaracki_Click);
        //        kopce.Text = q.ToString();
        //        cel.Controls.Add(kopce);
        //    }
        //    foot.Controls.Add(cel);
        //    if (nova[w] != null)
        //        nova[w].Rows.Add(foot);
        //    kraj[w] = nova[w];
        //}
        if (nova[0].Rows.Count > 0)
        {
            for (int w = 0; w <= j; w++)
            {
                foot = new TableFooterRow();
                cel = new TableCell();
                cel.ColumnSpan = 5;
                for (int q = 1; q <= j + 1; q++)
                {
                    Button kopce = new Button();
                    kopce.ID =q.ToString();
                    kopce.Click += new EventHandler(kopceListaNaracki_Click);
                    kopce.Text = q.ToString();
                    cel.Controls.Add(kopce);
                }
                foot.Controls.Add(cel);
                if (nova[w] != null)
                    nova[w].Rows.Add(foot);
                kraj[w] = nova[w];
            }
        }
        else
        {
            foot = new TableFooterRow();
            cel = new TableCell();
            cel.Text = " Нема податоци - нарачки ";
            cel.BorderColor = System.Drawing.Color.White;
            cel.BorderWidth = Unit.Pixel(2);
            cel.BorderStyle = BorderStyle.Solid;
            foot.Controls.Add(cel);
            nova[0].Rows.Add(foot);
            kraj[0] = nova[0];
        }
        return kraj;
    }
    void barajNarackaBtn_Click(object sender, EventArgs e)
    {
        int[] vrednosti = new int[2];
        tabNN = 0;
        DropDownList lok = ((DropDownList)tabela.FindControl("lokaciiLista"));
        DropDownList grad = ((DropDownList)tabela.FindControl("gradoviLista"));
        try
        {
            vrednosti[0] = lok.SelectedIndex;
            vrednosti[1] = grad.SelectedIndex;
            strana.Session["BarajNar"] = vrednosti;
            podatoci.ContentTemplateContainer.Controls.Clear();
            this.nacartajTabelaNarackiNapraveni();
            podatoci.Update();
        }
        catch (Exception ex)
        {
            OCon.Close();
        }
    }
    void izmeniBtn_Click(object sender, EventArgs e)
    {
        try
        {
            String[] pomString = ((Button)sender).ID.ToString().Split('.');
            int red = Convert.ToInt32(pomString[2].ToString());
            int tab = Convert.ToInt32(pomString[1].ToString());
            info["NarackaId"]=tabelaNaracki[tab].Rows[red].Cells[1].Text.ToString().Trim();
            stranica.Session["Korisnik"] = info;
            
        }
        catch (Exception ex)
        {
            
        }
        strana.Response.Redirect("Naracaj.aspx?Mod=1&A=2");

    }
    void izvestajBtn_Click(object sender, EventArgs e)
    {
        try
        {
            String[] pomString = ((Button)sender).ID.ToString().Split('.');
            int red = Convert.ToInt32(pomString[2].ToString());
            int tab = Convert.ToInt32(pomString[1].ToString());

            if (info.ContainsKey("NarackaId"))
            {
                info["NarackaId"] = tabelaNaracki[tab].Rows[red].Cells[1].Text.ToString().Trim();
            }
            else
            {
                info.Add("NarackaId", tabelaNaracki[tab].Rows[red].Cells[1].Text.ToString().Trim());
            }
            stranica.Session["Korisnik"] = info;
        }
        catch (Exception ex)
        {
            
        }
        strana.Response.Redirect("Naracaj.aspx?Mod=2&A=2");
    }
    void kopceListaNaracki_Click(object sender, EventArgs e)
    {
        tabNN = Convert.ToInt32(((Button)sender).ID.ToString()) - 1;
        podatoci.ContentTemplateContainer.Controls.Clear();
        podatoci.ContentTemplateContainer.Controls.Add(tabelaNaracki[Convert.ToInt32(((Button)sender).ID.ToString()) - 1]);
    }
    void listaGradoviNaracka_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList lok = ((DropDownList)vnes.FindControl("lokaciiLista"));
        DropDownList grad = ((DropDownList)vnes.FindControl("gradoviLista"));
        Label pomAdresa = ((Label)tabela.FindControl("adresaLbl"));
        pomAdresa.Text = "";
        try
        {
            if (grad.Items[grad.SelectedIndex].Value.ToString() != "0")
            {
                String komanda = "Select distinct lokacija.Ime,lokacija.Id from lokacija where lokacija.grad_id =" + grad.Items[grad.SelectedIndex].Value.ToString() + " and  lokacija.komitent_id=" + komitentId;

                OCon.Open();

                OracleCommand OCom = new OracleCommand(komanda, OCon);
                OracleDataReader dr = OCom.ExecuteReader();
                lok.Items.Clear();
                while (dr.Read())
                {
                    ListItem pom = new ListItem(dr["Ime"].ToString(), dr["ID"].ToString());
                    lok.Items.Add(pom);
                }
                OCon.Close();
            }
            else
            {
                lok.Items.Clear();
                ListItem pom = new ListItem("Сите", "0");
                lok.Items.Add(pom);
                pomAdresa.Text ="          ";

            }
            this.listaLokaciiNaracka_SelectedIndexChanged(lok, new EventArgs());
        }
        catch (Exception ex)
        {
            OCon.Close();
        }


    }
    void listaLokaciiNaracka_SelectedIndexChanged(object sender, EventArgs e)
    {
        Label pom = ((Label)tabela.FindControl("adresaLbl"));
        Button kopce = (Button)vnes.FindControl("barajNarackaBtn");
        DropDownList lok = ((DropDownList)vnes.FindControl("lokaciiLista"));
        try
        {
            if (lok.Items[lok.SelectedIndex].Value != "0")
            {
                String komanda = "Select adresa from lokacija where lokacija.id=" + ((DropDownList)sender).Items[((DropDownList)sender).SelectedIndex].Value.ToString();

                OCon.Open();

                OracleCommand OCom = new OracleCommand(komanda, OCon);
                String adresa = OCom.ExecuteScalar().ToString();
                pom.Text = adresa;
                OCon.Close();
            }
        }
        catch (Exception ex)
        {
            OCon.Close();
        }
        this.barajNarackaBtn_Click(kopce, new EventArgs());
    }
    public void nacrtajListaProizvodiIzvestaj()
    {
        String komanda;

        try
        {
            String narackaId = info["NarackaId"].ToString();
            OCon.Open();
            komanda = "Select proizvod.id as IDP,tipproizvod.Id as IDT,danok.id as IDD, proizvod.IME as Име,tipproizvod.ime as Група,danok.IME as Данок,listaproizvodi.cena as Цена,listaproizvodi.kolicina as Количина from listaproizvodi join proizvod on proizvod.id=listaproizvodi.proizvod_id join naracka on naracka.id=listaproizvodi.naracka_id join danok on danok.id=listaproizvodi.danok_id join tipproizvod on tipproizvod.id=proizvod.tipproizvod_id and naracka.id=" + narackaId.ToString().Trim() +" order by tipproizvod.ime " ;
            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OracleDataReader Odr = OCom.ExecuteReader();
            tabelaListaN = vratiTabelaListaProizvodi(Odr);
            OCon.Close();
            podatociLista.ContentTemplateContainer.Controls.Add(tabelaListaN[tabLN]);
        }
        catch (Exception ex)
        {

            OCon.Close();
        }

    }
    public Table[] vratiTabelaListaProizvodi(OracleDataReader drO)
    {
        int i = 1;
        int j = 0;
        int pat = 1;
        Table[] nova = new Table[100];
        TableHeaderCell head;
        TableCell cel;
        TableRow row = new TableRow();
        TableFooterRow foot;
        nova[j] = new Table();
        int vkupno = 0;
        while (drO.Read())
        {
            if (pat == 1)
            {
                //nova[j] = new Table();
                head = new TableHeaderCell();
                head.Text = "Бр";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Производ_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Attributes.Add("class", "hide");
                head.Text = "Тип_ID";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Attributes.Add("class", "hide");
                head.Text = "Данок_ID";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Име";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Група";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Данок";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Цена";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Кол";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Вкупно";
                row.Controls.Add(head);
                nova[j].Controls.Add(row);
                pat = 2;

            }
            //if (i > 15)
            //{
            //    i = 1;
            //    j++;
            //    row = new TableRow();
            //    nova[j] = new Table();
            //    head = new TableHeaderCell();
            //    head.Text = "Бр";
            //    head.Attributes.Add("class", "hide");
            //    head = new TableHeaderCell();
            //    head.Text = "Производ_ID";
            //    head.Attributes.Add("class", "hide");
            //    row.Controls.Add(head);
            //    head = new TableHeaderCell();
            //    head.Text = "Тип_ID";
            //    head.Attributes.Add("class", "hide");
            //    row.Controls.Add(head);
            //    head = new TableHeaderCell();
            //    head.Text = "Данок_ID";
            //    head.Attributes.Add("class", "hide");
            //    row.Controls.Add(head);
            //    head = new TableHeaderCell();
            //    head.Text = "Име";
            //    row.Controls.Add(head);
            //    head = new TableHeaderCell();
            //    head.Text = "Група";
            //    row.Controls.Add(head);
            //    head = new TableHeaderCell();
            //    head.Text = "Данок";
            //    row.Controls.Add(head);
            //    head = new TableHeaderCell();
            //    head.Text = "Цена";
            //    row.Controls.Add(head);
            //    head = new TableHeaderCell();
            //    head.Text = "Кол";
            //    row.Controls.Add(head);
            //    head = new TableHeaderCell();
            //    head.Text = "Вкупно";
            //    row.Controls.Add(head);
            //    nova[j].Controls.Add(row);
            //}
            row = new TableRow();
            cel = new TableCell();
            cel.Text = i.ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDP"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDT"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDD"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Име"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Група"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Данок"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Цена"].ToString();
            cel.Attributes.Add("class", "textCellLevo");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Количина"].ToString();
            cel.Attributes.Add("class", "textCellLevo");
            row.Controls.Add(cel);
            cel = new TableCell();
            vkupno += Convert.ToInt32(drO["Количина"].ToString()) * Convert.ToInt32(drO["Цена"].ToString()); 
            cel.Text = Convert.ToString(Convert.ToInt32(drO["Количина"].ToString()) * Convert.ToInt32(drO["Цена"].ToString()));
            cel.Attributes.Add("class", "textCellLevo textVkupno");
            row.Controls.Add(cel);
            nova[j].Controls.Add(row);
            i++;
        }
        row = new TableRow();
        cel = new TableCell();
        cel.ColumnSpan = 11;
        cel.Text ="Вкупно:" + vkupno.ToString() +".00 ден";
        cel.Attributes.Add("class", "textCellLevo textVkupno");
        row.Controls.Add(cel);
        nova[j].Controls.Add(row);
        Table[] kraj = new Table[j + 1];
        if (nova[0].Rows.Count > 0)
        {
            for (int w = 0; w <= j; w++)
            {
                foot = new TableFooterRow();
                cel = new TableCell();
                cel.ColumnSpan = 5;
                for (int q = 1; q <= j + 1; q++)
                {
                    Button kopce = new Button();
                    kopce.ID = "." + q.ToString();
                    kopce.Click += new EventHandler(kopceLista_Click);
                    kopce.Text = q.ToString();
                    cel.Controls.Add(kopce);
                }
                foot.Controls.Add(cel);
                if (nova[w] != null)
                    nova[w].Rows.Add(foot);
                kraj[w] = nova[w];
            }
        }
        else
        {
            foot = new TableFooterRow();
            cel = new TableCell();
            cel.Text = " Нарачката е празна ";
            cel.BorderColor = System.Drawing.Color.White;
            cel.BorderWidth = Unit.Pixel(2);
            cel.BorderStyle = BorderStyle.Solid;
            foot.Controls.Add(cel);
            nova[0].Rows.Add(foot);
            kraj[0] = nova[0];
        }
        return kraj;
    }
    public void iscitiStaticni()
    {
        tabLN = 0;
        tabNN = 0;
        tabP = 0;
    }
    public Table[] tabelaProiz()
    {
        return tabelaN;
    }
    public String vratinarackaId()
    {
        return info["NarackaId"].ToString();
    }
    public Table[] tabelaListaNar()
    {
        return tabelaListaN;
    }
    public Table[] vratiTabelaProizvodiPopust(OracleDataReader drO)
    {
        int i = 1;
        int j = 0;
        int pat = 1;
        Table[] nova = new Table[100];
        TableHeaderCell head;
        TableCell cel;
        TableRow row = new TableRow();
        TableFooterRow foot;


        nova[j] = new Table();
        while (drO.Read())
        {
            if (pat == 1)
            {
                //nova[j] = new Table();
                head = new TableHeaderCell();
                head.Text = "Бр";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Производ_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Attributes.Add("class", "hide");
                head.Text = "Тип_ID";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Attributes.Add("class", "hide");
                head.Text = "Данок_ID";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Име";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Група";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Данок";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "ЦенаНаб";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Попуст";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Цена";
                row.Controls.Add(head);
                nova[j].Controls.Add(row);
                pat = 2;

            }
            if (i > 10)
            {
                i = 1;
                j++;
                row = new TableRow();
                nova[j] = new Table();
                head = new TableHeaderCell();
                head.Text = "Бр";
                head.Attributes.Add("class", "hide");
                head = new TableHeaderCell();
                head.Text = "Производ_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Тип_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Данок_ID";
                head.Attributes.Add("class", "hide");
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Име";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Група";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Данок";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "ЦенаНаб";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Попуст";
                row.Controls.Add(head);
                head = new TableHeaderCell();
                head.Text = "Цена";
                row.Controls.Add(head);
                nova[j].Controls.Add(row);
            }
            row = new TableRow();
            cel = new TableCell();
            cel.Text = i.ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDP"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDT"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["IDD"].ToString();
            cel.Attributes.Add("class", "hide");
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Име"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Група"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Данок"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["ЦенаНаб"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Попуст"].ToString();
            row.Controls.Add(cel);
            cel = new TableCell();
            cel.Text = drO["Цена"].ToString();
            row.Controls.Add(cel);
            nova[j].Controls.Add(row);
            i++;
        }
        Table[] kraj = new Table[j + 1];
        if (nova[0].Rows.Count > 0)
        {
            for (int w = 0; w <= j; w++)
            {
                foot = new TableFooterRow();
                cel = new TableCell();
                cel.ColumnSpan = 5;
                for (int q = 1; q <= j + 1; q++)
                {
                    Button kopce = new Button();
                    kopce.ID = q.ToString();
                    kopce.Click += new EventHandler(kopce_Click);
                    kopce.Text = q.ToString();
                    cel.Controls.Add(kopce);
                }
                foot.Controls.Add(cel);
                if (nova[w] != null)
                    nova[w].Rows.Add(foot);
                kraj[w] = nova[w];
            }
        }
        else
        {
            foot = new TableFooterRow();
            cel = new TableCell();
            cel.Text = " Нема податоци";
            cel.BorderColor = System.Drawing.Color.White;
            cel.BorderWidth = Unit.Pixel(2);
            cel.BorderStyle = BorderStyle.Solid;
            foot.Controls.Add(cel);
            nova[0].Rows.Add(foot);
            kraj[0] = nova[0];
        }
        return kraj;
    }
    public void nacrtajTabelaPopust()
    {

        String komanda;
        tabNN = 0;
        try
        {
            OCon.Open();
            komanda = "Select proizvod.id as IDP,tipproizvod.Id as IDT,danok.id as IDD, proizvod.IME as Име,tipproizvod.IME as Група,danok.IME as Данок,Round(proizvod.Cena * danok.OSNOVICA) as ЦенаНаб,tipcena.Ime as Попуст,vratiCena(proizvod.id," + komitentId.ToString().Trim() + ")  as Цена from proizvod join tipproizvod on tipproizvod.id=proizvod.tipproizvod_id join danok on danok.id=proizvod.danok_id join edinecnacena e on e.proizvod_id = proizvod.id join komitent on e.Komitent_ID=komitent.id  join tipcena on tipcena.id=e.tipcena_id and tipcena.mnozitel<1 and komitent.id=" + komitentId.ToString().Trim() + " order by tipproizvod.IME ";
            OracleCommand OCom = new OracleCommand(komanda, OCon);
            OracleDataReader Odr = OCom.ExecuteReader();
            tabelaN = vratiTabelaProizvodiPopust(Odr);
            OCon.Close();
            podatoci.ContentTemplateContainer.Controls.Add(tabelaN[tabP]);
        }
        catch (Exception ex)
        {

            OCon.Close();
        }

    }
}
