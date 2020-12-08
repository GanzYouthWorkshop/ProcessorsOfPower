using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pop.CompilerApi
{
    /// <summary>
    /// Egy általános eszköz, ami <see cref="TokenDefinition{T}"/>-ök alapján tokenizáls egy bemeneti <see cref="string"/>-et.
    /// </summary>
    /// <typeparam name="T">A létrehozandó tokenek típusa egy <see cref="Enum"/>-ban tárolva.</typeparam>
    public class Tokenizer<T> where T : Enum
    {
        /// <summary>
        /// A token leírásokat tároló lista.
        /// </summary>
        public List<TokenDefinition<T>> TokenDefinitions { get; set; } = new List<TokenDefinition<T>>();

        /// <summary>
        /// Tokenizál egy bemenő szöveget.
        /// </summary>
        /// <param name="source">A bemenő szöveg <see cref="string"/>-je.</param>
        /// <param name="parent">Egy esetlges szülő-token.</param>
        /// <returns>A szövegben lévő tokenek.</returns>
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
                        if (parent == null || parent.Rank > definition.Rank)
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
                        Rank = bestMatchingDefinition.Rank,
                    };
                    result.Add(token);

                    if (bestMatchingDefinition.Rank > 0)
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

        /// <summary>
        /// Tokenizál egy bemenő szöveget.
        /// </summary>
        /// <param name="source">A bemenő szöveg <see cref="string"/>-je.</param>
        /// <returns>A szövegben lévő tokenek.</returns>
        public List<Token<T>> Tokenize(string source)
        {
            return this.Tokenize(source, null);
        }
    }
}
