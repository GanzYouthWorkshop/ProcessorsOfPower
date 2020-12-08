using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Pop.CompilerApi
{
    /// <summary>
    /// Leír egy forráskód nyelvi tokent.
    /// </summary>
    /// <typeparam name="T">A tokenek típusa.</typeparam>
    [DebuggerDisplay("{TokenType} - {Value} - {Tokens.Count} subs")]
    public class Token<T> where T : Enum
    {
        /// <summary>
        /// A token típusa <see cref="T"/> enum szerint.
        /// </summary>
        public T TokenType { get; set; }

        /// <summary>
        /// A token értéke.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// A token rangja. Egy 0-nál nagyobb rangú token összetett lehet.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// A token al-tokenjei.
        /// </summary>
        public List<Token<T>> Tokens { get; set; }
    }
}
