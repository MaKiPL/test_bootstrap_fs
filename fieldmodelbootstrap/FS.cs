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
            public uint Entry;
            public uint LengthOfUnpackedFile;
            public uint LocationInFS;
            public uint LZSS;
        }

        private const uint LastItemFSlocation = 0xFFFFFFFF;
        public Dictionary<string, FI> FSarch = new Dictionary<string, FI>();
        private List<uint> FsFileLength;
        private byte[] fsBuffer;

        private string[] filesList;

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
            fsBuffer = File.ReadAllBytes(path);
            ReadSuppliers(fiPath, flPath);
        }

        public FS(byte[] FS, byte[] FI, byte[] FL)
        {
            fsBuffer = FS;
            ReadSuppliers(FI, FL);
        }

        private void ReadSuppliers(string fiPath_, string flPath_)
        {
            filesList = File.ReadAllLines(flPath_).Select(x => x.ToLower()).ToArray();
            FsFileLength = new List<uint>();
            using (FileStream fs = new FileStream(fiPath_, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
                for (int i = 0; i < filesList.Length; i++)
                {
                    FSarch.Add(filesList[i], new FI() { Entry = (uint)i, LengthOfUnpackedFile = br.ReadUInt32(), LocationInFS = br.ReadUInt32(), LZSS = br.ReadUInt32() });
                    FsFileLength.Add(BitConverter.ToUInt32(fsBuffer, (int)FSarch.Last().Value.LocationInFS) + 4);
                }
        }

        private void ReadSuppliers(byte[] fiPath_, byte[] flPath_)
        {
            //filesList
            filesList = MakiExtended.ConvertBufferToStringArray(flPath_, Encoding.UTF8).Select(x=> x.ToLower().TrimEnd()).ToArray();

            uint fsIndex = 0;
            FsFileLength = new List<uint>();
                for (int i = 0; i < filesList.Length; i++)
                {
                if (string.IsNullOrWhiteSpace(filesList[i])) continue;
                FSarch.Add(filesList[i], new FI() { Entry = (uint)i, LengthOfUnpackedFile = BitConverter.ToUInt32(fiPath_, (int)fsIndex), LocationInFS = BitConverter.ToUInt32(fiPath_, (int)fsIndex+4), LZSS = BitConverter.ToUInt32(fiPath_, (int)fsIndex+8) });
                fsIndex += 12;
                FsFileLength.Add(BitConverter.ToUInt32(fsBuffer, (int)FSarch.Last().Value.LocationInFS) + 4);
            }
        }

        public byte[] GetFile(string filename)
        {
            FI myFI = FSarch[filename.ToLower()];


            if (myFI.LZSS == 1)
            {
                byte[] fileBuffer = new byte[FsFileLength[(int)myFI.Entry]];
                Array.Copy(fsBuffer, myFI.LocationInFS, fileBuffer, 0, fileBuffer.Length);
                return LZSS.DecompressAll(fileBuffer, (uint)fileBuffer.Length, (int)myFI.LengthOfUnpackedFile);
            }
            else
            {
                byte[] fileBuffer = new byte[myFI.LengthOfUnpackedFile];
                Array.Copy(fsBuffer, myFI.LocationInFS, fileBuffer, 0, fileBuffer.Length);
                return fileBuffer;
            }
        }

        public string FindFile(string filenamePart)
        {
            var b = (from ar in GetFileList() where ar.Contains(filenamePart) select ar);
            if (b.Count() > 0) return b.First();
            else return "ERR_ERR_ERR";
        }
        

        public string[] GetFileList()
        => filesList;

        public FS GetArchive(string filepath)
        {
            string noExt = filepath.Substring(0, filepath.Length - 2);
            return new FS(
                GetFile($"{noExt}fs"),
                GetFile($"{noExt}fi"),
                GetFile($"{noExt}fl")
                ); 
            
        }
    }
}
