using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fieldmodelbootstrap
{
    class CharaOne
    {
        private int characterEntries;

        unsafe struct CharD
        {
            public CharHeader Header;
            public fixed char Name[4];
            public uint Unk;
            public uint Unk2;
        }

        unsafe struct CharP
        {
            public CharHeader Header;
            public uint Unk;
            public fixed char Name[4];
            public uint Unk2;
            public uint Unk3;
        }

        private struct CharHeader
        {
            public uint Offset;
            public uint Length;
            public uint Length2;
            public ushort CharacterID;
            public ushort CharacterFlag; //0x10d0 means chara
            public uint TypeMark; //0x00000000 or -1
        };

        private struct CharEntry
        {
            public CharHeader Header;
            public object Char; //CharP or CharD
        }

        private CharEntry[] charEntries;


        public CharaOne(byte[] buffer)
        {
            int bitIndex = 0;
            characterEntries = BitConverter.ToInt32(buffer, 0);
            bitIndex += 4;
            charEntries = new CharEntry[characterEntries];
            //Process with chara.one for n entries by reading the header and creating CharEntry depending on type, then fullfil data and continue
        }
    }
}
