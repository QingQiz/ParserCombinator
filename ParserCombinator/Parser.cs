using System;

namespace ParserCombinator
{
    public class ParserInput
    {
        private readonly string _input;
        private int _index;

        public ParserInput(string input)
        {
            _input = input ?? "";
            _index = 0;
        }

        public char this[int index] => _input[index + _index];

        public static ParserInput operator ++(ParserInput inp)
        {
            inp._index++;
            return inp;
        }

        public bool End => _index >= _input.Length;
    }

    public class ParserResult<T1> : Tuple<T1, ParserInput>
    {
        public ParserResult(T1 item1, ParserInput item2) : base(item1, item2)
        {
        }

        public T1 Result => Item1;

        public ParserInput Input => Item2;
    }

    /// <summary>
    /// Parser TParser
    /// </summary>
    /// <typeparam name="TParser"> the return type of the parser </typeparam>
    public class Parser<TParser>
    {
        private readonly Func<ParserInput, ParserResult<TParser>?> _parser;

        public Parser(Func<ParserInput, ParserResult<TParser>?> parser)
        {
            _parser = parser;
        }

        public ParserResult<TParser>? Parse(ParserInput inp)
        {
            return _parser(inp);
        }
    }
}