using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Pop.CompilerApi
{
    public class TokenDefinition<T> where T : Enum
    {
        public T TokenType { get; set; }
        public Regex TokenDescriptor { get; set; }
        public bool NeedsSubTokenization { get; set; }

        public TokenDefinition(T token, Regex descriptor, bool subTokenize)
        {
            this.NeedsSubTokenization = subTokenize;
            this.TokenType = token;
            this.TokenDescriptor = descriptor;
        }

        public TokenDefinition(T token, Regex descriptor) : this(token, descriptor, false) { }
    }
}
