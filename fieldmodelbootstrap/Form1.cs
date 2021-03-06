﻿using System;
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
        FS fs;
        FS map;

        public Form1()
        {
            InitializeComponent();
        }

        private void openFieldfsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fs = new FS();
            if(fs.FSarch.Count < 1)
            {
                toolStripStatusLabel1.Text = "Bad field.fs";
                return;
            }
            //string[] FsFiles = fs.GetFileList().Select(x => MakiExtended.getFilename_dirty(x).EndsWith(".fs",StringComparison.CurrentCulture)).ToArray();
            List<string> FsFiles = (from a in fs.GetFileList() where a.EndsWith(".fs") select MakiExtended.getFilename_dirty_withoutExtension(a)).ToList();
            FS mapData = fs.GetArchive(fs.FindFile("mapdata.fs"));
            string[] maplistFileList = MakiExtended.ConvertBufferToStringArray(mapData.GetFile(mapData.FindFile("maplist")),Encoding.UTF8);
            for (int i = 0; i < FsFiles.Count; i++)
                if (!maplistFileList.Contains(FsFiles[i]))
                {
                    FsFiles.RemoveAt(FsFiles.IndexOf(FsFiles[i]));
                    i--;
                }
            listBox1.DataSource = FsFiles;
            toolStripStatusLabel1.Text = $"FS ready at : {listBox1.Items.Count} entries";
        }

        void ListBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            
            ListBox send = sender as ListBox;
            if (send.Items.Count < 1) return;
            string s = send.SelectedValue as string;
            string path = fs.FindFile(s);
            if (string.IsNullOrWhiteSpace(path)) 
                return;
            map = fs.GetArchive(path);

            GC.Collect(); //GC have to be here
            GC.WaitForPendingFinalizers();

            string charaFile = map.FindFile("chara.one");
            if(charaFile == "ERR_ERR_ERR") return;
            CharaOne charaOne = new CharaOne(map.GetFile(charaFile));
            listBox2.DataSource = charaOne.GetNames;
            if (listBox2.Items.Count == 0)
            {
                listBox2.Enabled = false;
                listBox2.DataSource = new string[] { "BROKEN FILE. ;_;" };
            }
            else listBox2.Enabled = true;

            System.IO.File.WriteAllBytes(@"D:/chara.one", charaOne.buffer);
        }
    }
}
