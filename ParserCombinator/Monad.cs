using System;
using System.Collections.Generic; 
namespace ParserCombinator
{
    public static class Monad
    {
        #region Monad

        /// <summary>
        /// p1 >>= p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns> Parser T1 -> (T1 -> Parser T2) -> Parser T2 </returns>
        public static Parser<T2> Then<T1, T2>(this Parser<T1> p1, Func<T1, Parser<T2>> p2)
        {
            return new Parser<T2>(inp =>
            {
                var res = p1.Parse(inp);

                if (res == null) return null;

                return p2(res.Result).Parse(res.Input);
            });
        }

        /// <summary>
        /// p1 >> p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns> Parser T1 -> Parser T2 -> Parser T2 </returns>
        public static Parser<T2> ThenI<T1, T2>(this Parser<T1> p1, Parser<T2> p2)
        {
            return p1.Then(_ => p2);
        }

        /// <summary>
        /// return ret
        /// </summary>
        /// <param name="ret"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> T -> Parser T </returns>
        public static Parser<T> Return<T>(T ret)
        {
            return new Parser<T>(inp => new ParserResult<T>(ret, inp));
        }

        #endregion

        #region Applicative

        /// <summary>
        /// func &lt;$&gt; p
        /// </summary>
        /// <param name="func"></param>
        /// <param name="p"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns> (T1 -> T2) -> Parser T1 -> Parser T2 </returns>
        public static Parser<T2> Map<T1, T2>(this Func<T1, T2> func, Parser<T1> p)
        {
            return p.Then(res => Return(func(res)));
        }

        /// <summary>
        /// func &lt;$&gt; p
        /// </summary>
        /// <param name="func"></param>
        /// <param name="p"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <returns> (T1 -> T2 -> T3) -> Parser T1 -> Parser (T2 -> T3) </returns>
        public static Parser<Func<T2, T3>> Map<T1, T2, T3>(this Func<T1, T2, T3> func, Parser<T1> p)
        {
            return p.Then(t1 => Return(new Func<T2, T3>(t2 => func(t1, t2))));
        }

        /// <summary>
        /// p1 &lt;*&gt; p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns> Parser (T1 -> T2) -> Parser T1 -> Parser T2 </returns>
        public static Parser<T2> Map<T1, T2>(this Parser<Func<T1, T2>> p1, Parser<T1> p2)
        {
            return p1.Then(t1 => p2.Then(t2 => Return(t1(t2))));
        }

        /// <summary>
        /// p1 &lt;* p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns> Parser T1 -> Parser T2 -> Parser T1 </returns>
        public static Parser<T1> FollowBy<T1, T2>(Parser<T1> p1, Parser<T2> p2)
        {
            return new Func<T1, Func<T2, T1>>(a => b => a).Map(p1).Map(p2);
        }

        #endregion

        #region Alternative

        /// <summary>
        /// Empty result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Parser<List<T>> Empty<T>()
        {
            return Return(new List<T>());
        }

        /// <summary>
        /// p1 &lt;|&gt; p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Parser<T> Or<T>(this Parser<T> p1, Parser<T> p2)
        {
            return new Parser<T>(inp =>
            {
                var res = p1.Parse(inp);

                if (res != null) return res;

                return p2.Parse(inp);
            });
        }

        /// <summary>
        /// run parser zero or more times
        /// <br/>
        /// many p = ((:) &lt;$&gt; p &lt;*&gt; many p) &lt;|&gt; empty
        /// </summary>
        /// <param name="p">parser</param>
        /// <returns> parser T -> parser [T] </returns>
        public static Parser<List<T>> Many<T>(Parser<T> p)
        {
            // stackoverflow ....
            // return p.Then(x => Many(p).Then(y => Return(y.Append(x).ToList())))
            //     .Then(res => Return(res.AsEnumerable().Reverse().ToList()))
            //     .Or(Empty<T>());

            return new Parser<List<T>>(inp =>
            {
                var res  = new List<T>();
                var last = inp;

                while (true)
                {
                    var x = p.Parse(last);
                    if (x == null) break;

                    res.Add(x.Result);
                    last = x.Input;
                }

                return new ParserResult<List<T>>(res, last);
            });
        }

        #endregion
    }
}