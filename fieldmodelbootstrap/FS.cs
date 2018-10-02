using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fieldmodelbootstrap
{
    internal class FS
    {
        public struct FI
        {
            public uint LengthOfUnpackedFile;
            public uint LocationInFS;
            public uint LZSS;
        }

        Dictionary<string, FI> FSarch = new Dictionary<string, FI>();

        public FS(string path = null)
        {
            if(path==null)
                using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Final Fantasy VIII field.fs file|field.fs", Multiselect = false, CheckFileExists = true })
                    if (ofd.ShowDialog() == DialogResult.OK) path = ofd.FileName;
                    else return;
            if (!File.Exists(path)) throw new FileNotFoundException();
            string fiPath = Path.Combine(Path.GetDirectoryName(path),$"{Path.GetFileNameWithoutExtension(Path.GetFullPath(path))}.fi");
            string flPath = Path.Combine(Path.GetDirectoryName(path), $"{Path.GetFileNameWithoutExtension(Path.GetFullPath(path))}.fl");
            if (!File.Exists(fiPath) || !File.Exists(flPath)) throw new FileNotFoundException();
            ReadSuppliers(fiPath, flPath);
        }

        private void ReadSuppliers(string fiPath_, string flPath_)
        {
            string[] filesList = File.ReadAllLines(flPath_).Select(x => x.ToLower()).ToArray();
            using (FileStream fs = new FileStream(fiPath_, FileMode.Open, FileAccess.Read))
                using (BinaryReader br = new BinaryReader(fs))
                    for (int i = 0; i < filesList.Length; i++)
                        FSarch.Add(filesList[i], new FI() { LengthOfUnpackedFile = br.ReadUInt32(), LocationInFS = br.ReadUInt32(), LZSS = br.ReadUInt32() });

        }

    }
}
