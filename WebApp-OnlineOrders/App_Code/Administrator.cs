using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Web.UI.WebControls;

using System.Web.UI.HtmlControls;
/// <summary>
/// Summary description for Admin
/// </summary>

public class Administrator
{
    protected String ImeAdmin;
    protected String UserName;
    protected String Password;
    protected String cnString;
    protected String komanda;
    protected OracleConnection oCon;
    protected OracleCommand oCom;
    protected OracleDataReader oDr;
    protected OracleDataAdapter oDa;
    protected Int32 brojNaRedovi;
    protected DropDownList DaNeDropDownList;
    protected DropDownList TipCenaDropDownList;
    protected DropDownList TipProizvodDropDownList;
    protected bool povekeTabeli;
    public Administrator()
	{
        cnString = WebConfigurationManager.ConnectionStrings["Tunel"].ToString();
        oCon = new OracleConnection(cnString);
        oCom = new OracleCommand();
        brojNaRedovi = 10;
        povekeTabeli = false;

        DaNeDropDownList = new DropDownList();

        DaNeDropDownList.Items.Add("D");
        DaNeDropDownList.Items.Add("N");


	}
    public int DeleteDanoci(int ID)
    {
        int rez = -1;
        komanda = "Delete From Danok Where ID="+ID;

        oCom = new OracleCommand(komanda);
        oCom.Connection = oCon;

        try
        {
            oCon.Open();
            rez = oCom.ExecuteNonQuery();
            oCon.Close();
        }
        catch (Exception ex)
        {
         
        }

        return rez;
    }
    public DataSet Danoci()
    {
        DataSet nov = new DataSet();
        komanda = "Select * From Danok";
        try
        {
            oCon.Open();
            oDa = new OracleDataAdapter(komanda, oCon);
            oDa.Fill(nov);

            oCon.Close();

        }
        catch (Exception ex)
        {
            
        }

        return nov;
    }
    public DataSet Proizvodi(int id)
    {
        DataSet nov = new DataSet();
        komanda = "Select * From Proizvod where ID=" + id.ToString() +"";
        try
        {
            oCon.Open();
            oDa = new OracleDataAdapter(komanda, oCon);
            oDa.Fill(nov);

            oCon.Close();

        }
        catch (Exception ex)
        {

        }

        return nov;
    }
    public DataSet PrikaziTabela(string imeTabela)
    {
        DataSet nov = new DataSet();
        komanda = "Select * From " + imeTabela;
        try
        {
            oCon.Open();
            oDa = new OracleDataAdapter(komanda, oCon);
            oDa.Fill(nov);

            oCon.Close();

        }
        catch (Exception ex)
        {

        }

        return nov;
    }
    public LinkedList<ListItem> ListaParovi(string imeTabela)
    {
        LinkedList<ListItem> lista = new LinkedList<ListItem>();


        int br = 0;
        komanda = "Select ID,Ime From " + imeTabela;
        oCom = new OracleCommand(komanda, oCon);
        try
        {
            oCon.Open();
            oDr = oCom.ExecuteReader();
            while (oDr.Read())
            {

                ListItem item = new ListItem(oDr["Ime"].ToString(),oDr["ID"].ToString());

                lista.AddLast(item);
                br++;
            }

            oCon.Close();
        }
        catch (Exception ex)
        {

        }
        return lista;
    }
    public String Zacuvaj(string imeTabela,string []Red)
    {
        string rez;
        string ID;
        try
        {
            ID = Red[0].Trim();

            switch (imeTabela)
            {
                case "Danok":
                    {
                        komanda = "Update " + imeTabela+ " Set Ime='"+Red[1].Trim()+"',Osnovica="+Red[2].Trim().Replace(',','.')+" where ID="+ID;
                        break;
                    };
                case "Grad":
                    {
                        komanda = "Update " + imeTabela+ " Set Ime='"+Red[1].Trim()+"' where ID="+ID;
                        break;
                    };
                case "Firma":
                    {
                        komanda = "Update " + imeTabela + " Set Ime='" + Red[1].Trim() + "',TelBroj='" + Red[2].Trim() + "',Adresa='" + Red[3].Trim() + "',UserName='" + Red[4].Trim() + "',Lozinka='" + Red[5].Trim() + "' where ID=" + ID;
                        break;
                    };
               case "TipCena":
                    {
                        komanda = "Update " + imeTabela+ " Set Ime='"+Red[1].Trim()+"',Mnozitel="+Red[2].Trim().Replace(',','.')+" where ID="+ID; 
                        break;
                    };
                case "TipProizvod":
                    {
                        komanda = "Update " + imeTabela+ " Set Ime='"+Red[1].Trim()+"'  where ID="+ID; 
                        break;
                    };
                case "Komitent":
                    {
                        komanda = "Update Komitent Set TipCena_ID='"+Red[1].Trim()+"',Ime='" + Red[2].Trim() + "',TelBroj='" + Red[3].Trim() + "',ZiroSmetka='" + Red[4].Trim() + "',MaxSaldo=" + Red[5].Trim().Replace(',','.') + " where ID=" + ID;
                        break;
                    };
                case "Korisnik":
                    {
                        komanda = "Update Korisnik Set Ime='" + Red[1].Trim() + "',Prezime='" + Red[2].Trim() + "',EMB='" + Red[3].Trim() + "',UserName='" + Red[4].Trim() + "',Lozinka='" + Red[5].Trim() + "',Aktiven='"+Red[6].Trim()+"' where ID=" + ID;
                        break;
                    };
                case "Proizvod":
                    {
                        komanda = "Update Proizvod Set Ime='" + Red[1].Trim() + "',Tipproizvod_ID=" + Red[2].Trim() + ",cena=" + Red[3].Trim() + ",kolicina=" + Red[4].Trim() + ",Danok_ID=" + Red[5].Trim() + " where ID=" + ID;
                        break;
                    };
                case "Lokacija" :
                        {
                            komanda = "Update Lokacija Set Adresa='" + Red[1].Trim() + "',Ime='" + Red[2].Trim() + "' Where ID=" + ID;
                            break;
                        };
                case "Naracka":
                    {
                        komanda = "Update Naracka Set Platena='"+Red[1].Trim()+"' Where ID=" + ID;
                        break;
                    };
                case "EdinecnaCena":
                    {
                        komanda = "Update EdinecnaCena Set TipCena_Id="+Red[2].Trim()+" Where Komitent_ID="+ ID+ " and Proizvod_Id="+Red[1].Trim();
                        break;
                    };
                case "GrupnaCena":
                    {
                        komanda = "Update GrupnaCena Set TipCena_Id="+ Red[2].Trim()+ " Where Komitent_ID="+ID+ " and TipProizvod_Id="+Red[1].Trim();
                        break;
                    };
                case "ZiroSmetka":
                    {
                        komanda = "Update ZiroSmetka Set Smetka='" + Red[1].Trim() + "',Banka='" + Red[2].Trim() + "' Where Firma_ID=" + ID;
                        break;
                    }
                default:
                    break;
            }
            oCon.Open();
            oCom = new OracleCommand(komanda, oCon);
            int pom = oCom.ExecuteNonQuery();
            oCon.Close();
            if (pom == 1)
            {
                rez = "Успешно е внесено!";
            }
            else
            {
                rez = "Грешка !";
            }
    		
	    }
	    catch (Exception ex)
	    {
		    rez = ex.Message;
        }
        return rez;
    }
    public String VratiDaNeLista(string vrednost)
    {
        String Rezultat = "";

        Rezultat += "<select><option";
        if(vrednost.Trim()=="D")
        {
            Rezultat += " selected>D</option><option>N</option></select>";
        }else{
            Rezultat += ">D</option><option selected>N</option></select>";
        }

        return Rezultat;
    }
    public HtmlSelect VratiGradLista(string Vrednost)
    {

        HtmlSelect lista = new HtmlSelect();

        lista.Items.AddRange(ListaParovi("Grad").ToArray());

        lista.SelectedIndex = lista.Items.IndexOf(lista.Items.FindByText(Vrednost));

        return lista;
    }
    public HtmlSelect VratiKomitentLista(string Vrednost)
    {

        HtmlSelect lista = new HtmlSelect();

        lista.Items.AddRange(ListaParovi("Komitent").ToArray());

        lista.SelectedIndex = lista.Items.IndexOf(lista.Items.FindByText(Vrednost));

        return lista;
    }
    public HtmlSelect VratiProizvodLista(string Vrednost)
    {

        HtmlSelect lista = new HtmlSelect();

        lista.Items.AddRange(ListaParovi("Proizvod").ToArray());

        lista.SelectedIndex = lista.Items.IndexOf(lista.Items.FindByText(Vrednost));

        return lista;
    }
    public HtmlSelect VratiTipCenaLista(string Vrednost)
    {

        HtmlSelect lista = new HtmlSelect();

        lista.Items.AddRange(ListaParovi("TipCena").ToArray());

        lista.SelectedIndex = lista.Items.IndexOf(lista.Items.FindByText(Vrednost));

        return lista;
    }
    public HtmlSelect VratiTipProizvodLista(string Vrednost)
    {

        HtmlSelect lista = new HtmlSelect();

        lista.Items.AddRange(ListaParovi("TipProizvod").ToArray());

        lista.SelectedIndex = lista.Items.IndexOf(lista.Items.FindByText(Vrednost));

        return lista;
    }
    public HtmlSelect VratiDanokLista(string Vrednost)
    {

        HtmlSelect lista = new HtmlSelect();

        lista.Items.AddRange(ListaParovi("Danok").ToArray());

        lista.SelectedIndex = lista.Items.IndexOf(lista.Items.FindByText(Vrednost));

        return lista;
    }
    public HtmlInputText VratiEditText(string vrednost,bool daliEdit)
    {
        HtmlInputText textBox = new HtmlInputText("text");
        textBox.Value = vrednost;
        if (!daliEdit)
        {
            textBox.Attributes.Add("readonly", " ");
        }

        return textBox;
    }
    public String VratiID(string Ime, string imeTabela)//Vraka kluc za taa tabela !
    {
        int sifra = 0;
        switch (imeTabela)
        {
            case "Danok":
                {
                    komanda = "Select ID From " + imeTabela +" Ime='"+imeTabela.Trim()+"'";
                    break;
                };
            //case "ZiroSmetka":
            //    {
            //        //Vraka ID na firmata za dadeno ime !
            //        komanda="Select ID From Firma Where Ime='"+Ime+"'";
            //        break;
            //    };
            //case "Lokacija":
            //    {
            //        komanda = "Select ID From ";
            //        break;
            //    };
            default:
                break;
        }



        try
        {
            oCon.Open();
            oCom = new OracleCommand(komanda, oCon);


           sifra = Convert.ToInt32(oCom.ExecuteScalar());

            oCon.Close();

        }
        catch (Exception ex)
        {

        }
        return sifra.ToString();
    }
    public String UpdateDanok(string DanokID, String DanokIme, String DanokMnozitel)
    {
        komanda="Update Danok(Ime,Mnozitel) Set Ime='"+DanokIme.Trim()+"',Mnozitel='"+DanokMnozitel.Trim()+"' Where ID=";
        if (DanokID!=null)
        {
            komanda += DanokID.ToString();
        }
        else
        {
            komanda += VratiID(DanokIme, "Danok");
        }

        try
        {
            oCom = new OracleCommand(komanda, oCon);
            oCon.Open();

            int vrednost = oCom.ExecuteNonQuery();
            if (vrednost > 0)
            {

            }
            oCon.Close();
        }
        catch (Exception ex)
        {
            
            throw;
        }
        return "";
    }
    public DataSet PrikaziSredenaTabela(string imeTabela)
    {
        DataSet nov = new DataSet();

        switch (imeTabela)
        {
            case "Danok":
                {
                    komanda = "Select ID as \"Реден Број\",Ime as \"Вид на Данок\",Osnovica as \"Даночна стапка\" From " + imeTabela;
                    break;
                };
            case "Grad":
                {
                    komanda = "Select ID as \"Реден Број\",Ime as \"Име на Град\" From " + imeTabela;
                    break;
                };
            case "Firma":
                {
                    komanda = "Select ID as \"Реден Број\",Ime as \"Име на Фирма\",TelBroj as \"Тел.Број\",Adresa as \"Адреса\",UserName as \"UserName\",Lozinka as \"Лозинка\" From " + imeTabela;
                    break;
                };
            case "Komitent":
                {
                    komanda = "Select k.ID as \"Реден Број\",k.Ime as \"Фирма\",k.TelBroj as \"Тел.Број\",k.ZiroSmetka as \"Жиро Сметка\",k.MaxSaldo as \"Дозволена Сума\",k.Saldo as \"Тековно Салдо\",t.Ime as \"Цена\" From Komitent k inner join TipCena t on k.TipCena_Id=t.id";
                    break;
                };
            case "Korisnik":
                {
                    komanda = "Select korisnik.ID as \"Реден Број\",komitent.ime as \"Фирма\",korisnik.IME as \"Име\",korisnik.PREZIME as \"Презиме\",korisnik.EMB as \"Матичен Број\",korisnik.USERNAME as \"UserName\",korisnik.LOZINKA as \"Лозинка\",korisnik.AKTIVEN as \"Активен\" from komitent join korisnik on komitent.id=korisnik.komitent_id";
                    break;
                };
            case "Lokacija":
                {
                    komanda = "Select lokacija.ID as \"Реден Број\",komitent.ime as \"Фирма\",grad.ime as \"Град\",lokacija.adresa as \"Адреса\",lokacija.ime as \"Име на Локација\"from lokacija join komitent on komitent.id=lokacija.komitent_id join grad on grad.id=lokacija.grad_id order by komitent.ime";
                    break;
                };
            case "Proizvod":
                {
                    komanda = "Select p.id as \"Реден Број\",p.Ime as \"Производ\",t.Ime as \"Тип Производ\",p.Cena as \"Цена\",p.Kolicina as \"Количина\",d.Ime as \"Данок\" From Proizvod p ";
                    komanda+="Inner Join TipProizvod t on p.TipProizvod_Id=t.Id ";
                    komanda+="Inner Join Danok d on p.Danok_ID=d.id ";
                    break;
                };
            case "TipCena":
                {
                    komanda = "Select ID as \"Реден Број\",Ime as \"Име\",Mnozitel as \"Множител\" From " + imeTabela;
                    break;
                };
            case "TipProizvod":
                {
                    komanda = "Select ID as \"Реден Број\",Ime as \"Име\"  From " + imeTabela;
                    break;
                };
            case "Naracka":
                {
                    komanda = "Select n.id as \"Реден Број\",k.ime as \"Фирма\",kor.ime || ' ' ||kor.prezime as \"Нарачал\",";
                    komanda+="l.ime as \"Локација\",l.adresa as \"Адреса\",g.ime as \"Град\",n.vkupnacena as \"Цена\",n.platena as \"Платена\",";
                    komanda+="n.zatvorena as \"Затворена\" from naracka n ";
                    komanda+="join komitent k on k.id=n.komitent_id ";
                    komanda+="join korisnik kor on kor.id=n.korisnik_id ";
                    komanda+="join lokacija l on l.id=n.lokacija_id ";
                    komanda+="join grad g on g.id=l.grad_id";
                    break;
                };
            case "EdinecnaCena":
                {
                    komanda = "Select k.id as \"Kom.ID\",p.id as \"Proiz.id\",k.Ime as \"Фирма\",p.Ime as \"Производ\",t.Ime as \"Вид Цена\"  From EdinecnaCena e ";
                    komanda+="Inner Join Komitent k on e.Komitent_Id=k.ID ";
                    komanda+="Inner Join TipCena t on e.TipCena_Id=t.ID ";
                    komanda+="Inner Join Proizvod p on e.Proizvod_Id=p.ID ";
                    break;
                };
            case "GrupnaCena":
                {

                    komanda = "Select k.id as \"Kom.ID\",t.id as \"Grupa.ID\",k.Ime as \"Фирма\",t.ime as \"Производ\",C.Ime as \"Цена\"  From GrupnaCena g ";
                    komanda+="Inner Join TipProizvod t on g.TipProizvod_ID=t.id ";
                    komanda+="Join Komitent k on g.Komitent_Id=k.Id ";
                    komanda += "Join TipCena c on g.TipCena_ID=c.ID ";
                    break;
                };
            case "ZiroSmetka":
                {
                    komanda = "Select f.Ime as \"Фирма\",z.Smetka as \"Жиро Сметка\",z.Banka as \"Банка\" From ZiroSmetka z ";
                    komanda += "Inner Join Firma f on z.Firma_Id=f.id";
                    break;
                };
            case "ListaProizvodi":
                {
                    komanda = "Select * From " + imeTabela;
                    break;
                };
            default:
                break;
        }



        try
        {
            oCon.Open();
            oDa = new OracleDataAdapter(komanda, oCon);
            oDa.Fill(nov);

            oCon.Close();

        }
        catch (Exception ex)
        {

        }

        return nov;
    }
    public String PrikaziSredenaTabelaString(string imeTabela,bool daliBrisi)
    {
        String Rezultat = "";
        String header = "";
        DataSet rezDataSet = this.PrikaziSredenaTabela(imeTabela);

        int brojac=0;

        bool kreiranaTabela = false;

        if (rezDataSet.Tables[0].Rows.Count <= brojNaRedovi)
        {
            povekeTabeli = false;
        }
        else
        {
            povekeTabeli = true;
        }
        header = "<tr><th></th>";
        for (int i = 0; i < rezDataSet.Tables[0].Columns.Count; i++)
        {
            header += "<th>" + rezDataSet.Tables[0].Columns[i].ColumnName.ToString().Trim()+ "</th>";
        }
        header += "</tr>";
        for (int i = 0; i < rezDataSet.Tables[0].Rows.Count; i++)
        {
            if (povekeTabeli == true)
            {
                if (i != 0)
                {
                    if (i % brojNaRedovi  == 0)//Znaci deka treba nova tabela da se crta !
                    {
                        brojac++;
                        Rezultat += "</table><table class=\""+ brojac + "\">";
                        Rezultat += header;
                    }
                }
                else
                {
                    Rezultat += "<table class=\"" + brojac + "\">";
                    Rezultat += header;
                }
            }
            else
            {
                if (!kreiranaTabela)
                {
                    brojac = 1;
                    Rezultat += "<table class=\"tbr" + brojac + "\">";
                    Rezultat += header;
                    kreiranaTabela = true;
                }
            }
            Rezultat += "<tr>";
            if (daliBrisi == false)
            {
                Rezultat += "<td><a href=\"#\" class=\"red\">Измени</a></td>";
            }
            else
            {
                Rezultat += "<td><a href=\"#\" class=\"red\">Избриши</a></td>";
            }
            for (int j = 0; j < rezDataSet.Tables[0].Columns.Count; j++)
            {
                if (j == 0)
                {
                    Rezultat += "<td class=\"kluc\">";

                }
                else
                {
                    Rezultat += "<td>";
                }
                Rezultat += rezDataSet.Tables[0].Rows[i][j].ToString();
                Rezultat += "</td>";

            }
            Rezultat += "</tr>";
        }
        Rezultat += "</table>";
        Rezultat += "<div id=\"TabelaFooter\"></div>";
        
        return Rezultat;
    }
    public HtmlTable EditVrednost(string imeTabela,string kluc,string kluc2,bool nov,bool brisi)
    {
        String Rezultat = "";
        String komanda = "";
        DataSet rezDataSet = new DataSet();
        HtmlTable  tabela = new HtmlTable();
        HtmlTableRow red = new HtmlTableRow();
        HtmlTableCell kelija = new HtmlTableCell();

        kelija.InnerText = "Табела :" + imeTabela;
        red.Cells.Add(kelija);
        tabela.Rows.Add(red);
        if (nov)
        {
            rezDataSet = PrikaziSredenaTabela(imeTabela);
        }
        else
        {
            //Se raboti za izmena i togas treba da se sql query da bide so Where
            try
            {
                switch (imeTabela)
                {
                    case "Danok":
                        {
                            komanda = "Select ID as \"Реден Број\",Ime as \"Вид на Данок\",Osnovica as \"Даночна стапка\" From " + imeTabela + " where ID=" + kluc;
                            break;
                        };
                    case "Grad":
                        {
                            komanda = "Select ID as \"Реден Број\",Ime as \"Име на Град\" From " + imeTabela + " where ID=" + kluc; ;
                            break;
                        };
                    case "Firma":
                        {
                            komanda = "Select ID as \"Реден Број\",Ime as \"Име на Фирма\",TelBroj as \"Тел.Број\",Adresa as \"Адреса\",UserName as \"UserName\",Lozinka as \"Лозинка\" From " + imeTabela + " where ID=" + kluc; ;
                            break;
                        };
                    case "Komitent":
                        {
                            komanda = "Select k.ID as \"Реден Број\",k.Ime as \"Фирма\",k.TelBroj as \"Тел.Број\",k.ZiroSmetka as \"Жиро Сметка\",k.MaxSaldo as \"Дозволена Сума\",k.Saldo as \"Тековно Салдо\",t.Ime as \"Вид на цена\" From Komitent k Inner Join TipCena t on k.TipCena_Id=t.ID" + " where k.ID=" + kluc; ;
                            break;
                        };
                    case "Korisnik":
                        {
                            komanda = "Select korisnik.ID as \"Реден Број\",komitent.ime as \"Фирма\",korisnik.IME as \"Име\",korisnik.PREZIME as \"Презиме\",korisnik.EMB as \"Матичен Број\",korisnik.USERNAME as \"UserName\",korisnik.LOZINKA as \"Лозинка\",korisnik.AKTIVEN as \"Активен\" from komitent join korisnik on komitent.id=korisnik.komitent_id " + " where korisnik.ID=" + kluc; ;

                            break;
                        };
                    case "Lokacija":
                        {
                            komanda = "Select lokacija.ID as \"Реден Број\",komitent.ime as \"Фирма\",grad.ime as \"Град\",lokacija.adresa as \"Адреса\",lokacija.ime as \"Име на Локација\"from lokacija join komitent on komitent.id=lokacija.komitent_id join grad on grad.id=lokacija.grad_id  where Lokacija.ID= " + kluc;
                            break;
                        };
                    case "Proizvod":
                        {
                            komanda = "Select p.id as \"Реден Број\",p.Ime as \"Производ\",t.Ime as \"Тип Производ\",p.Cena as \"Цена\",p.Kolicina as \"Количина\",d.Ime as \"Данок\" From Proizvod p ";
                            komanda += "Inner Join TipProizvod t on p.TipProizvod_Id=t.Id ";
                            komanda += "Inner Join Danok d on p.Danok_ID=d.id " + " where p.id=" + kluc;
                            break;
                        };
                    case "TipCena":
                        {
                            komanda = "Select ID as \"Реден Број\",Ime as \"Име\",Mnozitel as \"Множител\" From " + imeTabela + " where ID=" + kluc; ;
                            break;
                        };
                    case "TipProizvod":
                        {
                            komanda = "Select ID as \"Реден Број\",Ime as \"Име\"  From " + imeTabela + " where ID=" + kluc; ;
                            break;
                        };
                    case "Naracka":
                        {
                            komanda = "Select n.id as \"Реден Број\",k.ime as \"Фирма\",kor.ime || ' ' ||kor.prezime as \"Нарачал\",";
                            komanda += "l.ime as \"Локација\",l.adresa as \"Адреса\",g.ime as \"Град\",n.vkupnacena as \"Цена\",n.platena as \"Платена\",";
                            komanda += "n.zatvorena as \"Затворена\" from naracka n ";
                            komanda += "join komitent k on k.id=n.komitent_id ";
                            komanda += "join korisnik kor on kor.id=n.korisnik_id ";
                            komanda += "join lokacija l on l.id=n.lokacija_id ";
                            komanda += "join grad g on g.id=l.grad_id " + " where n.id=" + kluc; ;
                            break;
                        };
                    case "EdinecnaCena":
                        {
                            komanda = "Select k.id as \"Kom.ID\",p.id as \"Proiz.id\",k.Ime as \"Фирма\",p.Ime as \"Производ\",t.Ime as \"Вид Цена\"  From EdinecnaCena e ";
                            komanda += "Inner Join Komitent k on e.Komitent_Id=k.ID ";
                            komanda += "Inner Join TipCena t on e.TipCena_Id=t.ID ";
                            komanda += "Inner Join Proizvod p on e.Proizvod_Id=p.ID " + " where k.id=" + kluc + " and p.id=" + kluc2;
                            break;
                        };
                    case "GrupnaCena":
                        {

                            komanda = "Select k.id as \"Kom.ID\",t.id as \"Grupa.ID\", k.Ime as \"Фирма\",t.ime as \"Производ\",C.Ime as \"Цена\"  From GrupnaCena g ";
                            komanda += "Inner Join TipProizvod t on g.TipProizvod_ID=t.id ";
                            komanda += "Join Komitent k on g.Komitent_Id=k.Id ";
                            komanda += "Join TipCena c on g.TipCena_ID=c.ID " + " where k.id=" + kluc + " and t.id=" + kluc2;
                            break;
                        };
                    case "ZiroSmetka":
                        {
                            komanda = "Select f.Ime as \"Фирма\",z.Smetka as \"Жиро Сметка\",z.Banka as \"Банка\" From ZiroSmetka z ";
                            komanda += "Inner Join Firma f on z.Firma_Id=f.id" + " where ID=" + kluc; ;
                            break;
                        };
                    case "ListaProizvodi":
                        {
                            komanda = "Select * From " + imeTabela + " where ID=" + kluc; ;
                            break;
                        };
                    default:
                        break;
                }


                oCon.Open();
                oDa = new OracleDataAdapter(komanda, oCon);
                oDa.Fill(rezDataSet);

                oCon.Close();
            }
            catch (Exception ex)
            {
                Rezultat = ex.Message;
                HtmlTableRow redg = new HtmlTableRow();
                HtmlTableCell kelijag = new HtmlTableCell();
                kelijag.InnerText = Rezultat;
                redg.Cells.Add(kelijag);
                tabela.Rows.Add(redg);
                return tabela;
            }

        }
        for (int i = 0; i < rezDataSet.Tables[0].Columns.Count; i++)
        {

            red = new HtmlTableRow();

            HtmlTableCell header = new HtmlTableCell();
            kelija = new HtmlTableCell();
            header.InnerText = rezDataSet.Tables[0].Columns[i].ColumnName.ToString();
            red.Cells.Add(header);

            switch (imeTabela)
            {
                case "Korisnik":
                    {
                        if (nov)
                        {
                            if (i == 0)
                            {
                                kelija.Controls.Add(VratiEditText("", false));
                            }else if (i == 1)
                            {
                                kelija.Controls.Add(VratiKomitentLista(""));
                            }else if (i == 7)
                            {
                                kelija.InnerHtml = VratiDaNeLista("D");
                            }
                            else
                            {
                                kelija.Controls.Add(VratiEditText("", true));
                            }

                            red.Cells.Add(kelija);
                        }
                        else
                        {
                            if (i == 7)
                            {
                                kelija.InnerHtml = VratiDaNeLista(rezDataSet.Tables[0].Rows[0][i].ToString());
                            }
                            else if (i == 0 || i == 1)
                            {
                                kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), false));
                            }
                            else
                            {
                                kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), true)); 
                            }
                            red.Cells.Add(kelija);
                        }
                        break;
                    };
                case "Proizvod":
                    {
                        if (nov)
                        {
                            if (i == 0)
                            {
                                kelija.Controls.Add(VratiEditText("", false));
                            }else if (i == 5)
                            {
                                kelija.Controls.Add(this.VratiDanokLista(""));
                            }
                            else if (i == 2)
                            {
                                kelija.Controls.Add(this.VratiTipProizvodLista(""));
                            }
                            else
                            {
                                 kelija.Controls.Add(VratiEditText("",true));
                            }
                            red.Cells.Add(kelija);
                        }
                        else
                        {
                            if (i == 5)
                            {
                                kelija.Controls.Add(this.VratiDanokLista(rezDataSet.Tables[0].Rows[0][i].ToString().Trim()));
                            }
                            else if (i == 0)
                            {
                                kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), false));
                            }
                            else if (i == 2)
                            {
                                kelija.Controls.Add(this.VratiTipProizvodLista(rezDataSet.Tables[0].Rows[0][i].ToString().Trim()));
                            }
                            else
                            {
                                kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), true));

                            }
                            red.Cells.Add(kelija);
                        }
                        break;
                    };
                case "Lokacija":
                    {
                        if (nov)
                        {
                            if (i == 0)
                            {
                                kelija.Controls.Add(VratiEditText("", false));
                            }else if (i == 1)
                            {
                                kelija.Controls.Add(VratiKomitentLista(""));
                            }
                            else if (i == 2)
                            {
                                kelija.Controls.Add(VratiGradLista(""));
                            }
                            else
                            {
                                kelija.Controls.Add(VratiEditText("", true));
                            }
                        }
                        else
                        {
                            if (i == 0 || i == 1)
                            {
                                kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), false));
                            }
                            else
                            {
                                kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), true));
                            }
                        }
                        red.Cells.Add(kelija);
                        break;
                    };
                case "Naracka":
                        {
                            if (nov)
                            {
                                kelija.InnerHtml = "Нема дозвола !";
                            }
                            else
                            {
                                if (i == 7)
                                {
                                    kelija.InnerHtml = VratiDaNeLista(rezDataSet.Tables[0].Rows[0][i].ToString());
                                }
                                else
                                {
                                    kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), false));

                                }
                            }
                            red.Cells.Add(kelija);
                            break;
                        };
                case "EdinecnaCena":
                    {
                        if (nov)
                        {
                            if(i== 0 || i==1)
                            {
                                kelija.Controls.Add(VratiEditText("", false));
                            }else if (i == 2)
                            {
                                kelija.Controls.Add(VratiKomitentLista(""));
                            }
                            else if (i == 3)
                            {
                                kelija.Controls.Add(VratiProizvodLista(""));
                            }
                            else if (i == 4)
                            {
                                kelija.Controls.Add(VratiTipCenaLista(""));
                            }else
                            {
                                kelija.Controls.Add(VratiEditText("", true));
                            }
                        }
                        else
                        {
                            if (i == 4)
                            {
                                kelija.Controls.Add(VratiTipCenaLista(rezDataSet.Tables[0].Rows[0][i].ToString()));
                            }
                            else
                            {
                                kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), false));
                                
                            }
                        }
                        red.Cells.Add(kelija);
                        break;
                    };
                case "GrupnaCena":
                    {
                        if (nov)
                        {
                            if (i == 0 || i == 1)
                            {
                                kelija.Controls.Add(VratiEditText("", false));
                            }
                            else if (i == 2)
                            {
                                kelija.Controls.Add(VratiKomitentLista(""));
                            }
                            else if (i == 3)
                            {
                                kelija.Controls.Add(VratiTipProizvodLista(""));
                            }
                            else if (i == 4)
                            {
                                kelija.Controls.Add(VratiTipCenaLista(""));
                            }
                            else
                            {
                                kelija.Controls.Add(VratiEditText("", true));
                            }
                        }
                        else
                        {
                            if (i == 4)
                            {
                                kelija.Controls.Add(VratiTipCenaLista(rezDataSet.Tables[0].Rows[0][i].ToString()));
                            }
                            else
                            {
                                kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), false));
                            }
                        }
                        red.Cells.Add(kelija);
                        break;
                    };
                case "Komitent":
                    {
                        if (nov)
                        {
                            if (i == 0)
                            {
                                kelija.Controls.Add(VratiEditText("", false));
                            }else if(i==6)
                            {
                                kelija.Controls.Add(VratiTipCenaLista(""));
                            }
                            else
                            {
                                kelija.Controls.Add(VratiEditText("", true));
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), false));
                            }
                            else if (i == 6)
                            {
                                kelija.Controls.Add(VratiTipCenaLista(rezDataSet.Tables[0].Rows[0][i].ToString()));
                            }
                            else
                            {
                                kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), true));
                            }
                        }
                        red.Cells.Add(kelija);
                        break;
                    };
                default:
                    {
                        if (nov)
                        {
                            if (i == 0)
                            {
                                kelija.Controls.Add(VratiEditText("", false));
                            }
                            else
                            {
                                kelija.Controls.Add(VratiEditText("", true));
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), false));
                            }
                            else
                            {
                                kelija.Controls.Add(VratiEditText(rezDataSet.Tables[0].Rows[0][i].ToString(), true));
                            }
                        }
                        red.Cells.Add(kelija);
                    break;
                    };
            }
            tabela.Rows.Add(red);

        }
        if (nov)//Dodavame nekoi novi redovi koga se raboti za nov 
        {
            switch (imeTabela)
            {
                default:
                    break;
            }
        }

        String hiddenVrednost = "<input type=\"hidden\" id=\"vrednost\" name=\"vrednost\"";
        
        HtmlButton submit = new HtmlButton();
        if (nov)
        {
            submit.InnerText = "Додади Нов";
            submit.Attributes.Add("type", "submit");
            submit.Attributes.Add("method", "post");
            submit.Attributes.Add("id", "dodadi_kopce");
        }
        else
        {
            if (brisi)//Togas treba da se pojavi kopce za brisenje
            {
                submit.InnerText = "Избриши";
                submit.Attributes.Add("type", "submit");
                submit.Attributes.Add("method", "post");
                submit.Attributes.Add("id", "brisi_kopce");
            }
            else
            {
                submit.InnerText = "Зачувај";
                submit.Attributes.Add("type", "submit");
                submit.Attributes.Add("method", "post");
                submit.Attributes.Add("id", "submit_kopce");
            }
        }



        
        red = new HtmlTableRow();
        kelija = new HtmlTableCell();

        kelija.Controls.Add(submit);
        red.Cells.Add(kelija);

       kelija = new HtmlTableCell();
        kelija.InnerHtml = hiddenVrednost;
        red.Cells.Add(kelija);
        tabela.Rows.Add(red);
        if (brisi==true)//Site text box gi stavame na disable  , samo za proverka pred brisenje.
        {
            foreach (HtmlTableRow  redica in tabela.Rows)
            {
                foreach (HtmlTableCell kel in redica.Cells)
                {
                    for (int i = 0; i < kel.Controls.Count ; i++)
                    {
                        HtmlInputControl input = kel.Controls[i] as HtmlInputControl;
                        if (input != null)
                        {
                            input.Attributes.Add("readonly", "");
                        }
                        else
                        {
                            HtmlSelect select = kel.Controls[i] as HtmlSelect;
                            if (select != null)
                            {
                                select.Attributes.Add("disabled", "");
                            }
                        }

                    }
                }

            }
        }
        return tabela;
    }
    public String[] KoloniVoTabela(string imeTabela)
    {
        LinkedList<String> lista = new LinkedList<string>();
        DataSet nov = new DataSet();
        komanda = "Select * From " + imeTabela;
        oCom = new OracleCommand(komanda, oCon);
        try
        {
            oCon.Open();
            oDa = new OracleDataAdapter(komanda, oCon);
            oDa.Fill(nov);

            oCon.Close();
        }
        catch (Exception ex)
        {

        }

        for(int i=0;i<nov.Tables[0].Columns.Count;i++)
        {
            lista.AddLast(nov.Tables[0].Columns[i].ColumnName.ToString());
        }
        return lista.ToArray<string>();
    }
    public String InsertVrednost(string imeTabela, String[] Red)
    {
        String[] koloni = KoloniVoTabela(imeTabela);
        String Rezultat = "";
        String vrednost = "";
        Boolean proverka = true;
        int pocetok = 1;
        int rez=-1;
        try
        {
            switch (imeTabela)
            {
                case "GrupnaCena":
                case "EdinecnaCena":
                    {
                        pocetok = 0;
                        proverka= true;
                        break;
                    };
                case "Proizvod":
                    {
                        pocetok = 1;
                        proverka = true;
                        vrednost = Red[3].Trim();
                        break;
                    };
                default:
                    {
                        pocetok = 1;
                        proverka = true;
                        vrednost = Red[1].Trim();
                        break;
                    };
                   
            }
            if (proverka)
            {
                if (ProveriVrednost(imeTabela, Red) == true)
                {
                    Rezultat = "Таков податок веќе постои !";
                    return Rezultat;
                }
            }
            komanda = "Insert Into " + imeTabela.Trim() + "(";

            for (int i = pocetok; i < koloni.Length; i++)
            {
                if (i == koloni.Length - 1)
                {
                    komanda += koloni[i].ToString().Trim() + ")";
                }
                else
                {
                    komanda += koloni[i].ToString().Trim() + ",";
                }
            }

            komanda += " Values (";
            for (int i = pocetok; i < koloni.Length; i++)
            {
                if (i == koloni.Length - 1)
                {
                    komanda += "'" + Red[i].Trim() + "')";
                }
                else
                {
                    komanda += "'" + Red[i].Trim() + "',";
                }
            }


            oCon.Open();
            oCom = new OracleCommand(komanda, oCon);

            rez =oCom.ExecuteNonQuery();

            oCon.Close();
        }
        catch (Exception ex)
        {
            return "Грешка :" + ex.Message;
        }
        if (rez > 0)
        {
            Rezultat = "Додавањето беше успешно!";
        }
        return Rezultat;
    }
    public Boolean ProveriVrednost(string imeTabela, string[] vrednost)
    {
        Boolean ima = false;
        int rez = -1;

        try
        {
            switch (imeTabela)
            {
                case "EdinecnaCena":
                    {
                        komanda = "Select Count(*) From EdinecnaCena Where KOMITENT_ID=" + vrednost[0] + " and PROIZVOD_ID=" + vrednost[2];
                        break;
                    };
                case "GrupnaCena":
                    {
                        komanda = "Select Count(*) From GrupnaCena Where KOMITENT_ID=" + vrednost[1] + " and TipPROIZVOD_ID=" + vrednost[0];
                        break;
                    };
                default:
                    {
                        komanda = "Select Count(*) From " + imeTabela + " where Ime='" + vrednost[1] + "'";
                        break;
                    };
            }
                



            oCon.Open();

            oCom = new OracleCommand(komanda, oCon);
            rez = Convert.ToInt32(oCom.ExecuteScalar().ToString());
            oCon.Close();
        }
        catch (Exception ex)
        {

        }
        if (rez > 0)
        {
            ima = true;
        }
        else
        {
            ima = false;
        }
        return ima;
    }
    public String DeleteVrednost(string imeTabela, string kluc, string kluc2)
    {
        bool daliBrisi = true;
        String Rezultat = "";
        int rez = -2;

        switch (imeTabela)
        {

            case "EdinecnaCena":
                {
                    komanda = "Delete From EdinecnaCena Where Komitent_ID=" + kluc + " and Proizvod_ID="+kluc2;
                    break;
                };
            case "GrupnaCena":
                {
                    komanda = "Delete From GrupnaCena Where Komitent_ID=" + kluc + " and TipProizvod_ID=" + kluc2;
                    break;
                };
            case "ZiroSmetka":
                {
                    daliBrisi = false;
                    break;
                };
            default :
                {
                    komanda = "Delete From " + imeTabela + " Where ID=" + kluc;
                    break;
                };
        }

        try
        {
            if (daliBrisi == true)
            {
                oCon.Open();

                oCom = new OracleCommand(komanda, oCon);
                rez = oCom.ExecuteNonQuery();
                oCon.Close();
            }
        }
        catch (Exception ex)
        {
            rez = -1;
        }


        if (rez == 1)
        {
            Rezultat = "Бришењето на податокот беше успешно.!";
        }
        else if (rez == -1)
        {
            Rezultat = "Настана Грешка.Податокот не може да биде избришан";
        }
        else
        {
            Rezultat = "Грешка во бришењето на податоците или се избришани повеќе податоци.";
        }
        return Rezultat ;

    }
}
