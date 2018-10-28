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
            //string[] FsFiles = fs.GetFileList().Select(x => MakiExtended.getFilename_dirty(x).EndsWith(".fs",StringComparison.CurrentCulture)).ToArray();
            List<string> FsFiles = (from a in fs.GetFileList() where a.EndsWith(".fs") select MakiExtended.getFilename_dirty_withoutExtension(a)).ToList();
            FS mapData = fs.GetArchive(fs.FindFile("mapdata.fs"));
            string[] maplistFileList = MakiExtended.ConvertBufferToStringArray(mapData.GetFile(mapData.FindFile("maplist")),Encoding.UTF8);
            foreach (string s in FsFiles) //change to for, because System.InvalidOperationException: Collection was modified; enumeration operation may not execute. after modify of list. Seems logical
                if (!maplistFileList.Contains(s)) ;
                    FsFiles.RemoveAt(FsFiles.IndexOf(s)); //mono crashes here, why?
            listBox1.DataSource = FsFiles;
            toolStripStatusLabel1.Text = $"FS ready at : {listBox1.Items.Count} entries";
        }
    }
}
