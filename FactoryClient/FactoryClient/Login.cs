using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace FactoryClient
{
    public partial class Login : Form
    {
        Request req = new Request();
        JArray org; Dictionary<string, string> name_to_id = new Dictionary<string, string>();
        string org_id = "1";
        public Login() { InitializeComponent(); }

        private void login_btn_Click(object sender, EventArgs e)
        {
            try { req.Login(id.Text, password.Text, org_id); }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            Form1 form = new Form1(req);
            form.Show();
            Hide();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            org = req.Get_Organization();
            foreach (JObject item in org)
            {
                organization.Items.Add(item["name"].ToObject<string>());
                name_to_id[item["name"].ToObject<string>()] = item["id"].ToObject<string>();
            }
        }

        private void organization_SelectedIndexChanged(object sender, EventArgs e) { org_id = name_to_id[organization.Text]; }
    }
}
