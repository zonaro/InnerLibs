using System;
using System.Collections.Generic;
using System.Linq;

namespace InnerLibs.Console
{

    /// <summary>
    /// Métodos para manipulação de aplicações baseadas em Console (System.Console)
    /// </summary>
    public static class ConsoleExtensions
    {

        /// <summary>
        /// Escreve uma data com descrição no console
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="Text"></param>
        /// <param name="BreakLines"></param>
        /// <returns></returns>
        public static string ConsoleWrite(this DateTime dateTime, string Text,int BreakLines = 0) => ConsoleWrite($"{dateTime} - {Text}",BreakLines);

        /// <summary>
        /// Escreve uma data com descrição no console e quebra 1 ou mais linhas
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="Text"></param>
        /// <param name="BreakLines"></param>
        /// <returns></returns>
        public static string ConsoleWriteLine(this DateTime dateTime, string Text,int BreakLines = 1) => dateTime.ConsoleWrite(Text,BreakLines.SetMinValue(1));

        /// <summary>
        /// Escreve no console colorindo palavras especificas
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="CustomColoredWords">Lista com as palavras e suas respectivas cores</param>
        public static string ConsoleWrite(this string Text, Dictionary<string, ConsoleColor> CustomColoredWords, int Lines = 0) => Text.ConsoleWrite(CustomColoredWords, StringComparison.InvariantCultureIgnoreCase, Lines);

        /// <summary>
        /// Escreve no console colorindo palavras especificas
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="CustomColoredWords">Lista com as palavras e suas respectivas cores</param>
        /// <param name="Comparison">Tipo de comparação</param>
        public static string ConsoleWrite(this string Text, Dictionary<string, ConsoleColor> CustomColoredWords, StringComparison Comparison, int BreakLines = 0)
        {
            CustomColoredWords = CustomColoredWords ?? new Dictionary<string, ConsoleColor>();
            CustomColoredWords = CustomColoredWords.SelectMany(x => x.Key.Split(" ").Distinct().ToDictionary(y => y, y => x.Value)).ToDictionary();
            var lastcolor = System.Console.ForegroundColor;
            var maincolor = CustomColoredWords.GetValueOr("", lastcolor);
            if (Text.IsNotBlank())
            {
                var substrings = Text.Split(" ");
                foreach (string substring in substrings)
                {
                    System.Console.ForegroundColor = maincolor;
                    System.Console.ForegroundColor = CustomColoredWords.Where(x => x.Key.Equals(substring, Comparison)).Select(x => x.Value).FirstOr(maincolor);
                    System.Console.Write(substring + " ");
                }
            }

            System.Console.ForegroundColor = lastcolor;
            ConsoleBreakLine(BreakLines);
            return Text;
        }

        /// <summary>
        /// Escreve no console usando uma cor especifica
        /// </summary>
        /// <param name="Text">Texto</param>
        public static string ConsoleWrite(this string Text, int BreakLines = 0) => Text.ConsoleWrite(System.Console.ForegroundColor, BreakLines);

        /// <summary>
        /// Escreve no console usando uma cor especifica
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Color">Cor</param>
        public static string ConsoleWrite(this string Text, ConsoleColor Color, int BreakLines = 0)
        {
            var lastcolor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = Color;
            System.Console.Write(Text ?? "");
            System.Console.ForegroundColor = lastcolor;
            ConsoleBreakLine(BreakLines);
            return Text;
        }

        /// <summary>
        /// Escreve uma linha no console colorindo palavras especificas
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="CustomColoredWords">Lista com as palavras e suas respectivas cores</param>

        public static string ConsoleWriteLine(this string Text, Dictionary<string, ConsoleColor> CustomColoredWords, int Lines = 1)
        {
            Lines = Lines.SetMinValue(1);
            Text.ConsoleWrite(CustomColoredWords, Lines);
            return Text;
        }

        /// <summary>
        /// Escreve uma linha no console usando uma cor especifica
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Color">Cor</param>

        public static string ConsoleWriteLine(this string Text, ConsoleColor Color, int Lines = 1)
        {
            Lines = Lines.SetMinValue(1);
            Text.ConsoleWrite(Color, Lines);
            return Text;
        }

        /// <summary>
        /// Escreve uma linha no console usando uma cor especifica
        /// </summary>
        /// <param name="Text">Texto</param>
        public static string ConsoleWriteLine(this string Text, int Lines = 1) => Text.ConsoleWriteLine(System.Console.ForegroundColor, Lines);

        /// <summary>
        /// Escreve o texto de uma exception no console
        /// </summary>
        /// <param name="Exception">Texto</param>
        /// <param name="Message">Mensagem Adicional de erro</param>
        /// <param name="Color">Cor</param>
        public static T ConsoleWriteError<T>(this T Exception, string Message, string Separator, ConsoleColor Color = ConsoleColor.Red, int Lines = 1) where T : Exception
        {
            var ex = new Exception(Message.IfBlank("Error"), Exception);
            return (T)ex.ConsoleWriteError(Separator, Color, Lines);
        }

        /// <summary>
        /// Escreve o texto de uma exception no console
        /// </summary>
        /// <param name="Exception">Texto</param>
        /// <param name="Color">Cor</param>
        public static T ConsoleWriteError<T>(this T Exception, string Separator, ConsoleColor Color = ConsoleColor.Red, int Lines = 1) where T : Exception
        {
            Lines = Lines.SetMinValue(1);
            Exception.ToFullExceptionString(Separator).ConsoleWrite(Color, Lines);
            return Exception;
        }

        /// <summary>
        /// Escreve o texto de uma exception no console
        /// </summary>
        public static T ConsoleWriteError<T>(this T Exception, string Message) where T : Exception
        {
            return Exception.ConsoleWriteError(Message, " >> ");
        }

        /// <summary>
        /// Escreve o texto de uma exception no console
        /// </summary>
        public static T ConsoleWriteError<T>(this T Exception) where T : Exception
        {
            return Exception.ConsoleWriteError(" >> ", ConsoleColor.Red, 1);
        }

        public static T ConsoleWriteError<T>(this T Exception, ConsoleColor Color, int Lines = 1) where T : Exception
        {
            return Exception.ConsoleWriteError(" >> ", Color, Lines);
        }

        public static T ConsoleWriteError<T>(this T Exception, int Lines) where T : Exception
        {
            return Exception.ConsoleWriteError(" >> ", ConsoleColor.Red, Lines);
        }

        /// <summary>
        /// Pula uma ou mais linhas no console
        /// </summary>
        /// <param name="Lines">Numero de linhas</param>
        public static void ConsoleBreakLine(int Lines = 1)
        {
            while (Lines > 0)
            {
                System.Console.WriteLine(string.Empty);
                Lines = Lines - 1;
            }
        }

        /// <summary>
        /// Pula uma ou mais linhas no console e retorna a mesma string (usada como chaining)
        /// </summary>
        /// <param name="Lines">Numero de linhas</param>
        public static string ConsoleBreakLine(this string Text, int Lines = 1)
        {
            ConsoleBreakLine(Lines);
            return Text;
        }

        /// <summary>
        /// Le a proxima linha inserida no console pelo usuário
        /// </summary>
        /// <returns></returns>
        public static string ReadLine()
        {
            return System.Console.ReadLine();
        }

        /// <summary>
        /// Le o proximo caractere inserido no console pelo usuário
        /// </summary>
        /// <returns></returns>
        public static char ReadChar()
        {
            return System.Console.ReadKey().KeyChar;
        }

        /// <summary>
        /// Le a proxima tecla pressionada pelo usuário
        /// </summary>
        /// <returns></returns>
        public static ConsoleKey ReadKey()
        {
            return System.Console.ReadKey().Key;
        }

        /// <summary>
        /// Toca um Beep
        /// </summary>
        /// <param name="Times">Numero de beeps</param>
        public static void Beep(int Times = 1)
        {
            for (int index = 1, loopTo = Times.SetMinValue(1); index <= loopTo; index++)
                System.Console.Beep();
        }

        /// <summary>
        /// Toca um beep especifico
        /// </summary>
        /// <param name="Frequency">Frequencia</param>
        /// <param name="Duration">Duracao em milisegundos</param>
        public static void Beep(int Frequency, int Duration, int Times = 1)
        {
            for (int index = 1, loopTo = Times.SetMinValue(1); index <= loopTo; index++)
                System.Console.Beep(Frequency.LimitRange(37, 32767), Duration);
        }
    }
}