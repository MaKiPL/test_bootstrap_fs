using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fieldmodelbootstrap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openFieldfsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FS fs = new FS();
            if(fs.FSarch.Count < 1)
            {
                toolStripStatusLabel1.Text = "Bad field.fs";
                return;
            }
            FS mapData = fs.GetArchive(fs.FindFile("mapdata.fs"));
            listBox1.DataSource = MakiExtended.ConvertBufferToStringArray(mapData.GetFile(mapData.FindFile("maplist")),Encoding.UTF8);
            toolStripStatusLabel1.Text = $"FS ready at : {listBox1.Items.Count} entries";

        }
    }
}
