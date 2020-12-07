using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Pop.CompilerApi
{
    [DebuggerDisplay("{TokenType} - {Value} - {Tokens.Count} subs")]
    public class Token<T>
    {
        public T TokenType { get; set; }
        public string Value { get; set; }

        public List<Token<T>> Tokens { get; set; }
    }
}
