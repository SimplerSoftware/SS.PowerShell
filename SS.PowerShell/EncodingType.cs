using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.PowerShell
{
    /// <summary>
    /// <para type="description">Types of text encoding to use when reading content.</para>
    /// </summary>
    public enum EncodingType : int
    {
        Default = 0,
        ASCII,
        BigEndianUnicode,
        Unicode,
        UTF32,
        UTF7,
        UTF8
    }
}
