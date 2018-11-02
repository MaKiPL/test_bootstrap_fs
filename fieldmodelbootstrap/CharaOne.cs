using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace fieldmodelbootstrap
{
    class CharaOne
    {
        private int characterEntries;

        struct CharD
        {
            public char[] Name;
            public uint Unk;
            public uint Unk2;
        }

        private struct CharPO
        {
            public uint Unk;
            public char[] Name;
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

        public string[] GetNames;

        public byte[] buffer;


        unsafe public CharaOne(byte[] buffer)
        {
            this.buffer = buffer;
            int bitIndex = 0;
            characterEntries = BitConverter.ToUInt16(buffer, 0);
            if (buffer[3] != 0 || buffer[2] != 0) return;
            if (buffer[0] == 0) return;
            bitIndex += 4;
            charEntries = new CharEntry[characterEntries];

            for(int i = 0; i<characterEntries; i++)
            {
                charEntries[i].Header = new CharHeader()
                {
                    Offset = BitConverter.ToUInt32(buffer, bitIndex),
                    Length = BitConverter.ToUInt32(buffer, bitIndex+4),
                    Length2 = BitConverter.ToUInt32(buffer, bitIndex + 8),
                    CharacterID = BitConverter.ToUInt16(buffer, bitIndex + 12),
                    CharacterFlag = BitConverter.ToUInt16(buffer, bitIndex + 14),
                    TypeMark = BitConverter.ToUInt32(buffer, bitIndex + 16)
                };
                bitIndex += sizeof(CharHeader);
                int typeMark = (int)charEntries[i].Header.TypeMark;
                if (typeMark == -1)
                {
                    //is pet
                    charEntries[i].Char = new CharPO()
                    {
                        Unk = BitConverter.ToUInt32(buffer, bitIndex),
                        Name = MakiExtended.GetStringRawBuffer(buffer, bitIndex + 4, 4).ToCharArray(),
                        Unk2 = BitConverter.ToUInt32(buffer, bitIndex + 8),
                        Unk3 = BitConverter.ToUInt32(buffer, bitIndex + 12)
                    };
                    bitIndex += 16;
                }
                else if(typeMark == 0)
                {
                    charEntries[i].Char = new CharD()
                    {
                        Name = MakiExtended.GetStringRawBuffer(buffer, bitIndex, 4).ToCharArray(),
                        Unk = BitConverter.ToUInt32(buffer, bitIndex + 8),
                        Unk2 = BitConverter.ToUInt32(buffer, bitIndex + 12)
                    };
                    bitIndex += 12;
                }
                else
                {
                    //is pet
                    charEntries[i].Char = new CharPO()
                    {
                        Unk = BitConverter.ToUInt32(buffer, bitIndex+4),
                        Name = MakiExtended.GetStringRawBuffer(buffer, bitIndex + 8, 4).ToCharArray(),
                        Unk2 = BitConverter.ToUInt32(buffer, bitIndex + 12),
                        Unk3 = BitConverter.ToUInt32(buffer, bitIndex + 16)
                    };
                    bitIndex += 24;
                }
            }

            string[] Dcollection = charEntries.Where(x => x.Char.GetType() == typeof(CharD)).Select(x => new string(((CharD)x.Char).Name)).OrderBy(x => x).ToArray();//.OrderBy(x => x.Name).ToArray();
            string[] POcollection = charEntries.Where(x => x.Char.GetType() == typeof(CharPO)).Select(x => new string(((CharPO)x.Char).Name)).OrderBy(x => x).ToArray();//.OrderBy(x => x.Name).ToArray();
            GetNames = Dcollection.Concat(POcollection).ToArray();
        }
    }
}
