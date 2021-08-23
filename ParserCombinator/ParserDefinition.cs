using System;
using System.Collections.Generic;

namespace ParserCombinator
{
    /// <summary>
    /// some predefined parser
    /// </summary>
    public static class ParserDefinition
    {
        /// <summary>
        /// parse a blank character
        /// </summary>
        public static readonly Parser<char> Space = Satisfy(char.IsWhiteSpace);

        /// <summary>
        /// parse zero or more blank character
        /// </summary>
        public static readonly Parser<List<char>> ManySpace = Monad.Many(Space);

        /// <summary>
        /// parse a char equal to <paramref name="c"/>
        /// </summary>
        /// <param name="c"></param>
        /// <returns>Parser char</returns>
        public static Parser<char> Char(char c)
        {
            return Satisfy(x => x == c);
        }

        /// <summary>
        /// parse a char equal to <paramref name="c"/> (ignore case)
        /// </summary>
        /// <param name="c"></param>
        /// <returns>Parser char</returns>
        public static Parser<char> CharI(char c)
        {
            return Satisfy(x => char.ToUpper(x) == char.ToUpper(c));
        }

        /// <summary>
        /// parse a string equal to <paramref name="s"/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns>Parser string</returns>
        public static Parser<string> String(string s)
        {
            // stackoverflow ....
            // if (string.IsNullOrEmpty(s)) return Monad.Return("");
            // return Char(s[0]).ThenI(String(s[1..])).ThenI(Monad.Return(s));
            return new Parser<string>(inp =>
            {
                foreach (var c in s)
                {
                    if (inp.End || c != inp[0]) return null;

                    inp++;
                }

                return new ParserResult<string>(s, inp);
            });
        }

        /// <summary>
        /// parse a string equal to <paramref name="s"/> (ignore case)
        /// </summary>
        /// <param name="s"></param>
        /// <returns>Parser string</returns>
        public static Parser<string> StringI(string s)
        {
            // stackoverflow ....
            // if (string.IsNullOrEmpty(s)) return Monad.Return("");
            // return CharI(s[0]).ThenI(StringI(s[1..])).ThenI(Monad.Return(s));
            return new Parser<string>(inp =>
            {
                foreach (var c in s)
                {
                    if (inp.End || char.ToUpper(c) != char.ToUpper(inp[0])) return null;

                    inp++;
                }

                return new ParserResult<string>(s, inp);
            });
        }

        /// <summary>
        /// parse a character which satisfies the function <paramref name="func"/>
        /// </summary>
        /// <param name="func"></param>
        /// <returns>Parser char</returns>
        public static Parser<char> Satisfy(Func<char, bool> func)
        {
            return new Parser<char>(inp =>
            {
                if (inp.End || !func(inp[0])) return null;

                return new ParserResult<char>(inp[0], ++inp);
            });
        }
    }
}