using System;
using System.Collections.Generic;
using System.Text;

namespace Pop.CompilerApi
{
    /// <summary>
    /// A bájtsorrendet ("endianness"-t) írja le.
    /// </summary>
    public enum ByteOrder
    {
        /// <summary>
        /// Little endian, a legkisebb bájt van elől - ilyenek az Intel processzorok is.
        /// </summary>
        LittleEndian,

        /// <summary>
        /// Big endian, a legnagyobb bájt van elől,az általános számábrázolás szerint.
        /// </summary>
        BigEndian
    }
}
