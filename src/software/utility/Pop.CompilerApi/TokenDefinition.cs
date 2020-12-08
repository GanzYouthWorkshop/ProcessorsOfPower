using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Pop.CompilerApi
{
    /// <summary>
    /// Egy token-típust definiáló leírás.
    /// </summary>
    /// <typeparam name="T">A token-típusok gyűjtő-enumja.</typeparam>
    public class TokenDefinition<T> where T : Enum
    {
        /// <summary>
        /// A token típusa.
        /// </summary>
        public T TokenType { get; set; }

        /// <summary>
        /// A token-típust leíró reguláris kifejezés.
        /// </summary>
        public Regex TokenDescriptor { get; set; }

        /// <summary>
        /// A token-típus rangja. Egy n-rangú token csak n-1 rangú al-tokeneket tartalmazhat.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Egy új token-leírást hoz létre.
        /// </summary>
        /// <param name="token">A token típusa.</param>
        /// <param name="descriptor">A token-típust leíró reguláris kifejezés.</param>
        /// <param name="rank">A token-típus rangja. Egy n-rangú token csak n-1 rangú al-tokeneket tartalmazhat.</param>
        public TokenDefinition(T token, Regex descriptor, int rank)
        {
            this.TokenType = token;
            this.TokenDescriptor = descriptor;
            this.Rank = rank;
        }

        /// <summary>
        /// Egy új token-leírást hoz létre 0 ranggal.
        /// </summary>
        /// <param name="token">A token típusa.</param>
        /// <param name="descriptor">A token-típust leíró reguláris kifejezés.</param>
        public TokenDefinition(T token, Regex descriptor) : this(token, descriptor, 0) { }
    }
}
