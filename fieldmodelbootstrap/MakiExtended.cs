using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fieldmodelbootstrap
{
    class MakiExtended
    {
        /// <summary>
        /// Converts byte array containing text buffer to string array of choosen encoding and splitting by \0\n\r. 
        /// </summary>
        /// <param name="buffer">byte array containg text buffer</param>
        /// <param name="enc">encoding parameter. We recommend UTF8</param>
        /// <returns></returns>
        public static string[] ConvertBufferToStringArray(byte[] buffer, Encoding enc)
=> enc.GetString(buffer).Split(new char[] { '\0', '\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);

        public static string getFilename_dirty(string s, char delimiter = '\\')=>
            s.Substring(s.LastIndexOf(delimiter)+1);

        public static string getFilename_dirty_withoutExtension(string s, char delimiter = '\\') 
        {
            string delimited = getFilename_dirty(s, delimiter);
            if (delimited.Contains('.'))
                return delimited.Substring(0, delimited.LastIndexOf('.'));
            else return delimited;
        }
    }
}
