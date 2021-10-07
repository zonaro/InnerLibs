using System;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs
{
    public class JSMin
    {
        private const int EOF = -1;
        private StringReader sr;
        private string sw = "";
        private int theA;
        private int theB;
        private int theLookahead = EOF;

        public string Minify(string src)
        {
            using (var sr = new StringReader(src))
            {
                jsmin(sr);
                return sw;
            }
        }

        private void jsmin(StringReader sr)
        {
            theA = Strings.AscW(Constants.vbLf);
            action(3, sr);
            while (theA != EOF)
            {
                switch (theA)
                {
                    case var @case when @case == Strings.AscW(" "):
                        {
                            if (IsAlphanum(theB))
                            {
                                action(1, sr);
                            }
                            else
                            {
                                action(2, sr);
                            }

                            break;
                        }

                    case var case1 when case1 == Strings.AscW(Constants.vbLf):
                        {
                            switch (theB)
                            {
                                case var case2 when case2 == Strings.AscW("{"):
                                case var case3 when case3 == Strings.AscW("["):
                                case var case4 when case4 == Strings.AscW("("):
                                case var case5 when case5 == Strings.AscW("+"):
                                case var case6 when case6 == Strings.AscW("-"):
                                    {
                                        action(1, sr);
                                        break;
                                    }

                                case var case7 when case7 == Strings.AscW(" "):
                                    {
                                        action(3, sr);
                                        break;
                                    }

                                default:
                                    {
                                        if (IsAlphanum(theB))
                                        {
                                            action(1, sr);
                                        }
                                        else
                                        {
                                            action(2, sr);
                                        }

                                        break;
                                    }
                            }

                            break;
                        }

                    default:
                        {
                            switch (theB)
                            {
                                case var case8 when case8 == Strings.AscW(" "):
                                    {
                                        if (IsAlphanum(theA))
                                        {
                                            action(1, sr);
                                            break;
                                        }

                                        action(3, sr);
                                        break;
                                    }

                                case var case9 when case9 == Strings.AscW(Constants.vbLf):
                                    {
                                        switch (theA)
                                        {
                                            case var case10 when case10 == Strings.AscW("}"):
                                            case var case11 when case11 == Strings.AscW("]"):
                                            case var case12 when case12 == Strings.AscW(")"):
                                            case var case13 when case13 == Strings.AscW("+"):
                                            case var case14 when case14 == Strings.AscW("-"):
                                            case var case15 when case15 == Strings.AscW("\""):
                                            case var case16 when case16 == Strings.AscW("'"):
                                                {
                                                    action(1, sr);
                                                    break;
                                                }

                                            default:
                                                {
                                                    if (IsAlphanum(theA))
                                                    {
                                                        action(1, sr);
                                                    }
                                                    else
                                                    {
                                                        action(3, sr);
                                                    }

                                                    break;
                                                }
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        action(1, sr);
                                        break;
                                    }
                            }

                            break;
                        }
                }
            }
        }

        private void action(int d, StringReader sr)
        {
            if (d <= 1)
            {
                put(theA);
            }

            if (d <= 2)
            {
                theA = theB;
                if (theA == Strings.AscW("'") || theA == Strings.AscW("\""))
                {
                    while (true)
                    {
                        put(theA);
                        theA = get(sr);
                        if (theA == theB)
                        {
                            break;
                        }

                        if (theA <= Strings.AscW(Constants.vbLf))
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}" + Constants.vbLf, theA));
                        }

                        if (theA == Strings.AscW(@"\"))
                        {
                            put(theA);
                            theA = get(sr);
                        }
                    }
                }
            }

            if (d <= 3)
            {
                theB = next(sr);
                if (theB == Strings.AscW("/") && (theA == Strings.AscW("(") || theA == Strings.AscW(",") || theA == Strings.AscW("=") || theA == Strings.AscW("[") || theA == Strings.AscW("!") || theA == Strings.AscW(":") || theA == Strings.AscW("&") || theA == Strings.AscW("|") || theA == Strings.AscW("?") || theA == Strings.AscW("{") || theA == Strings.AscW("}") || theA == Strings.AscW(";") || theA == Strings.AscW(Constants.vbLf)))
                {
                    put(theA);
                    put(theB);
                    while (true)
                    {
                        theA = get(sr);
                        if (theA == Strings.AscW("/"))
                        {
                            break;
                        }
                        else if (theA == Strings.AscW(@"\"))
                        {
                            put(theA);
                            theA = get(sr);
                        }
                        else if (theA <= Strings.AscW(Constants.vbLf))
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}." + Constants.vbLf, theA));
                        }

                        put(theA);
                    }

                    theB = next(sr);
                }
            }
        }

        private int next(StringReader sr)
        {
            int c = get(sr);
            if (c == Strings.AscW("/"))
            {
                switch (peek(sr))
                {
                    case var @case when @case == Strings.AscW("/"):
                        {
                            while (true)
                            {
                                c = get(sr);
                                if (c <= Strings.AscW(Constants.vbLf))
                                {
                                    return c;
                                }
                            }

                            break;
                        }

                    case var case1 when case1 == Strings.AscW("*"):
                        {
                            get(sr);
                            while (true)
                            {
                                switch (get(sr))
                                {
                                    case var case2 when case2 == Strings.AscW("*"):
                                        {
                                            if (peek(sr) == Strings.AscW("/"))
                                            {
                                                get(sr);
                                                return Strings.AscW(" ");
                                            }

                                            break;
                                        }

                                    case EOF:
                                        {
                                            throw new Exception("Error: JSMIN Unterminated comment." + Constants.vbLf);
                                            break;
                                        }
                                }
                            }

                            break;
                        }

                    default:
                        {
                            return c;
                        }
                }
            }

            return c;
        }

        private int peek(StringReader sr)
        {
            theLookahead = get(sr);
            return theLookahead;
        }

        private int get(StringReader sr)
        {
            int c = theLookahead;
            theLookahead = EOF;
            if (c == EOF)
            {
                c = sr.Read();
            }

            if (c >= Strings.AscW(" ") || c == Strings.AscW(Constants.vbLf) || c == EOF)
            {
                return c;
            }

            if (c == Strings.AscW(Constants.vbCr))
            {
                return Strings.AscW(Constants.vbLf);
            }

            return Strings.AscW(" ");
        }

        private void put(int c)
        {
            sw += Conversions.ToString(c);
        }

        public bool IsAlphanum(int c)
        {
            return c >= Strings.AscW("a") && c <= Strings.AscW("z") || c >= Strings.AscW("0") && c <= Strings.AscW("9") || c >= Strings.AscW("A") && c <= Strings.AscW("Z") || c == Strings.AscW("_") || c == Strings.AscW("$") || c == Strings.AscW(@"\") || c == Strings.AscW("+") || c > 126;
        }
    }
}