using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pop.CompilerApi
{
    public class Tokenizer<T> where T : Enum
    {
        public List<TokenDefinition<T>> TokenDefinitions { get; set; } = new List<TokenDefinition<T>>();

        public List<Token<T>> Tokenize(string source, Token<T> parent)
        {
            int index = 0;
            List<Token<T>> result = new List<Token<T>>();

            while (source.Length > 0)
            {
                Match bestMatch = null;
                TokenDefinition<T> bestMatchingDefinition = null;
                int bestMatchIndex = int.MaxValue;

                foreach (TokenDefinition<T> definition in this.TokenDefinitions)
                {
                    Match m = definition.TokenDescriptor.Match(source, index);
                    if (m.Success && m.Index < bestMatchIndex)
                    {
                        if (parent == null || parent.Value != m.Value || parent.Value.Length > m.Length)
                        {
                            bestMatch = m;
                            bestMatchIndex = m.Index;
                            bestMatchingDefinition = definition;
                        }
                    }
                }

                if (bestMatch != null)
                {
                    //source = source.Substring(bestMatch.Index + bestMatch.Length);
                    index += bestMatch.Length;

                    Token<T> token = new Token<T>
                    {
                        TokenType = bestMatchingDefinition.TokenType,
                        Value = bestMatch.Value,
                    };
                    result.Add(token);

                    if (bestMatchingDefinition.NeedsSubTokenization)
                    {
                        token.Tokens = this.Tokenize(token.Value, token);
                    }
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public List<Token<T>> Tokenize(string source)
        {
            return this.Tokenize(source, null);
        }
    }
}
