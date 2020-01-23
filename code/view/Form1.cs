using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace view
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void fileButton_Click(object sender, EventArgs e)
        {
            string fileName = GetFileFromDialog();
            if (fileName != null)
            {
                this.readsBox.Text = fileName;
            }
        }

        private static string GetFileFromDialog()
        {
            OpenFileDialog selectFileDialog = new OpenFileDialog();

            if (selectFileDialog.ShowDialog() == DialogResult.OK)
            {
                return selectFileDialog.FileName;
            }
            return null;
        }

        private void contigsFileButton_Click(object sender, EventArgs e)
        {

        }
    }
}
