using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Http.Headers;

namespace EDnevnik
{
    public partial class Ocena : Form
    {
        public Ocena()
        {
            InitializeComponent();
        }

        private void Ocena_Load(object sender, EventArgs e)
        {
            cmb_GodinaPopulate();
            cmb_Predmet.Enabled = false;
            cmb_Odeljenje.Enabled = false;
            cmb_Ucenik.Enabled = false;
            cmb_Ocena.Items.Add(1);
            cmb_Ocena.Items.Add(2);
            cmb_Ocena.Items.Add(3);
            cmb_Ocena.Items.Add(4);
            cmb_Ocena.Items.Add(5);
            //cmb_Ocena.Enabled = false;
            cmb_ProfesorPopulate();
        }

        private void cmb_GodinaPopulate()
        {
            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM skolska_godina", veza);
            DataTable dt_godina = new DataTable();
            adapter.Fill(dt_godina);
            cmb_Godina.DataSource = dt_godina;
            cmb_Godina.ValueMember = "id";
            cmb_Godina.DisplayMember = "naziv";
            cmb_Godina.SelectedValue = 2;
        }
        private void cmb_Godina_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmb_Godina.IsHandleCreated && cmb_Godina.Focused)
            {
                cmb_ProfesorPopulate();
            }
        }
        private void cmb_ProfesorPopulate()
        {
            SqlConnection veza = Konekcija.Connect();
            StringBuilder naredba = new StringBuilder("SELECT DISTINCT osoba.id AS ID, ime + ' ' + prezime AS naziv FROM osoba");
            naredba.Append(" JOIN raspodela ON osoba.id = nastavnik_id ");
            naredba.Append(" WHERE godina_id = " + cmb_Godina.SelectedValue.ToString());
            //textBox2.Text = naredba.ToString();
            SqlDataAdapter adapter = new SqlDataAdapter(naredba.ToString(), veza);
            DataTable dt_profesor = new DataTable();
            adapter.Fill(dt_profesor);
            cmb_Profesor.DataSource = dt_profesor;
            cmb_Profesor.ValueMember = "id";
            cmb_Profesor.DisplayMember = "naziv";
            cmb_Profesor.SelectedIndex = -1;
        }

        private void cmb_Profesor_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmb_Profesor.IsHandleCreated && cmb_Profesor.Focused)
            {
                cmb_PredmetPopulate();
                cmb_Predmet.Enabled = true;
            }
        }

        private void cmb_PredmetPopulate()
        {
            SqlConnection veza = Konekcija.Connect();
            StringBuilder naredba = new StringBuilder("SELECT DISTINCT predmet.id AS id, naziv FROM predmet");
            naredba.Append(" JOIN raspodela ON predmet.id = predmet_id ");
            naredba.Append(" WHERE godina_id = " + cmb_Godina.SelectedValue.ToString());
            naredba.Append(" AND nastavnik_id = " + cmb_Profesor.SelectedValue.ToString());
            //textBox2.Text = naredba.ToString();
            SqlDataAdapter adapter = new SqlDataAdapter(naredba.ToString(), veza);
            DataTable dt_predmet = new DataTable();
            adapter.Fill(dt_predmet);
            cmb_Predmet.DataSource = dt_predmet;
            cmb_Predmet.ValueMember = "id";
            cmb_Predmet.DisplayMember = "naziv";
            cmb_Predmet.SelectedIndex = -1;
        }

        private void cmb_Predmet_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmb_Predmet.IsHandleCreated && cmb_Predmet.Focused)
            {
                cmb_OdeljenjePopulate();
                cmb_Odeljenje.Enabled = true;
            }
        }
        private void cmb_OdeljenjePopulate()
        {
            StringBuilder naredba = new StringBuilder("SELECT DISTINCT odeljenje.id AS id, STR(razred) + '-' + indeks AS naziv FROM odeljenje ");
            naredba.Append(" JOIN raspodela ON odeljenje.id = odeljenje_id ");
            naredba.Append(" WHERE raspodela.godina_id = " + cmb_Godina.SelectedValue.ToString());
            naredba.Append(" AND nastavnik_id = " + cmb_Profesor.SelectedValue.ToString());
            naredba.Append(" AND predmet_id = " + cmb_Predmet.SelectedValue.ToString());
            //textBox2.Text = naredba.ToString();
            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter = new SqlDataAdapter(naredba.ToString(), veza);
            DataTable dt_Odeljenje = new DataTable();
            adapter.Fill(dt_Odeljenje);
            cmb_Odeljenje.DataSource = dt_Odeljenje;
            cmb_Odeljenje.ValueMember = "id";
            cmb_Odeljenje.DisplayMember = "naziv";
            cmb_Odeljenje.SelectedIndex = -1;
        }

        private void cmb_Odeljenje_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmb_Odeljenje.IsHandleCreated && cmb_Odeljenje.Focused)
            {
                cmb_UcenikPopulate();
                cmb_Ucenik.Enabled = true;
                GridPopulate();
            }

        }
        private void cmb_UcenikPopulate()
        {
            StringBuilder naredba = new StringBuilder("SELECT osoba.id AS id, ime + ' ' + prezime AS naziv FROM osoba ");
            naredba.Append(" JOIN upisnica ON osoba.id = osoba_id ");
            naredba.Append(" WHERE upisnica.odeljenje_id = " + cmb_Odeljenje.SelectedValue.ToString());
            //textBox2.Text = naredba.ToString();
            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter = new SqlDataAdapter(naredba.ToString(), veza);
            DataTable dt_Ucenik = new DataTable();
            adapter.Fill(dt_Ucenik);
            cmb_Ucenik.DataSource = dt_Ucenik;
            cmb_Ucenik.ValueMember = "id";
            cmb_Ucenik.DisplayMember = "naziv";
            cmb_Ucenik.SelectedIndex = -1;
        }

        private void GridPopulate()
        {
            StringBuilder naredba = new StringBuilder("SELECT ocena.id, ime + ' ' + prezime AS naziv, ocena, ucenik_id, datum FROM osoba ");
            naredba.Append(" JOIN ocena ON osoba.id = ucenik_id ");
            naredba.Append(" JOIN raspodela ON raspodela_id = raspodela.id ");
            naredba.Append(" WHERE raspodela_id = ");
            naredba.Append(" (SELECT id FROM raspodela ");
            naredba.Append(" WHERE godina_id = " + cmb_Godina.SelectedValue.ToString());
            naredba.Append(" AND nastavnik_id = " + cmb_Profesor.SelectedValue.ToString());
            naredba.Append(" AND predmet_id = " + cmb_Predmet.SelectedValue.ToString());
            naredba.Append(" AND odeljenje_id = " + cmb_Odeljenje.SelectedValue.ToString() + ")");
            //textBox2.Text = naredba.ToString();
            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter = new SqlDataAdapter(naredba.ToString(), veza);
            DataTable dt_ocena = new DataTable();
            adapter.Fill(dt_ocena);
            Grid_Ocene.DataSource = dt_ocena;
            Grid_Ocene.AllowUserToAddRows = false;
            Grid_Ocene.Columns["ucenik_id"].Visible = false;
        }

    }
}
