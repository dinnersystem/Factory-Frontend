﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FactoryClient
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            Request req;
            try { req = new Request(id.Text, password.Text); }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            Form1 form = new Form1(req);
            form.Show();
            Hide();
        }

        private void SaveMe_Click(object sender, EventArgs e)
        {
            this.Height += (this.Height == 225 ? 100 : 0 );
        }

        private void Saver_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://forms.gle/5vF8q7LUXUtXySAeA");
        }
    }
}
