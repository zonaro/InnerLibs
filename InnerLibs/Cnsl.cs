using InnerLibs.LINQ;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InnerLibs.Console
{
    /// <summary>
    /// Métodos para manipulação de aplicações baseadas em Console (System.Console)
    /// </summary>
    public static class Cnsl
    {
        /// <summary>
        /// Toca um Beep
        /// </summary>
        /// <param name="Times">Numero de beeps</param>
        public static string Beep(this string Text, int Times = 1)
        {
            for (int index = 1, loopTo = Times.SetMinValue(1); index <= loopTo; index++) System.Console.Beep();
            return Text;
        }

        /// <summary>
        /// Toca um beep especifico
        /// </summary>
        /// <param name="Frequency">Frequencia</param>
        /// <param name="Duration">Duracao em milisegundos</param>
        public static string Beep(this string Text, int Frequency, int Duration, int Times = 1)
        {
            for (int index = 1, loopTo = Times.SetMinValue(1); index <= loopTo; index++) System.Console.Beep(Frequency.LimitRange(37, 32767), Duration);
            return Text;
        }

        /// <summary>
        /// Pula uma ou mais linhas no console
        /// </summary>
        /// <param name="BreakLines">Numero de linhas</param>
        public static string ConsoleBreakLine(this string Text, int BreakLines = 1)
        {
            while (BreakLines > 0)
            {
                System.Console.WriteLine(string.Empty);
                BreakLines = BreakLines - 1;
            }
            return Text;
        }

        /// <summary>
        /// Escreve uma data com descrição no console
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="Text"></param>
        /// <param name="BreakLines"></param>
        /// <returns></returns>
        public static string ConsoleWrite(this DateTime dateTime, string Text, int BreakLines = 0) => ConsoleWrite($"{dateTime} - {Text}", BreakLines);

        /// <summary>
        /// Escreve no console colorindo palavras especificas
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="CustomColoredWords">Lista com as palavras e suas respectivas cores</param>
        public static string ConsoleWrite(this string Text, Dictionary<string, ConsoleColor> CustomColoredWords, int BreakLines = 0) => Text.ConsoleWrite(CustomColoredWords, StringComparison.InvariantCultureIgnoreCase, BreakLines);

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
            return Text.ConsoleBreakLine(BreakLines);
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
            return Text.ConsoleBreakLine(BreakLines);
        }

        /// <summary>
        /// Escreve o texto de uma exception no console
        /// </summary>
        /// <param name="Exception">Texto</param>
        /// <param name="Message">Mensagem Adicional de erro</param>
        /// <param name="Color">Cor</param>
        public static T ConsoleWriteError<T>(this T Exception, string Message, string Separator, ConsoleColor Color = ConsoleColor.Red, int BreakLines = 1) where T : Exception => (T)new Exception(Message.IfBlank("Error"), Exception).ConsoleWriteError(Separator, Color, BreakLines);

        /// <summary>
        /// Escreve o texto de uma exception no console
        /// </summary>
        /// <param name="Exception">Texto</param>
        /// <param name="Color">Cor</param>
        public static T ConsoleWriteError<T>(this T Exception, string Separator, ConsoleColor Color = ConsoleColor.Red, int BreakLines = 1) where T : Exception
        {
            if (Exception != null)
            {
                Exception.ToFullExceptionString(Separator).ConsoleWrite(Color, BreakLines.SetMinValue(1));

            }
            return Exception;
        }

        /// <summary>
        /// Escreve o texto de uma exception no console
        /// </summary>
        public static T ConsoleWriteError<T>(this T Exception, string Message) where T : Exception => Exception.ConsoleWriteError(Message, " >> ");

        /// <summary>
        /// Escreve o texto de uma exception no console
        /// </summary>
        public static T ConsoleWriteError<T>(this T Exception) where T : Exception => Exception.ConsoleWriteError(" >> ", ConsoleColor.Red, 1);

        /// <summary>
        /// Escreve o texto de uma exception no console
        /// </summary>
        public static T ConsoleWriteError<T>(this T Exception, ConsoleColor Color, int BreakLines = 1) where T : Exception => Exception.ConsoleWriteError(" >> ", Color, BreakLines);

        /// <summary>
        /// Escreve o texto de uma <typeparamref name="T"/> no console
        /// </summary>
        public static T ConsoleWriteError<T>(this T Exception, int BreakLines) where T : Exception => Exception.ConsoleWriteError(" >> ", ConsoleColor.Red, BreakLines);

        /// <summary>
        /// Escreve uma data com descrição no console e quebra 1 ou mais linhas
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="Text"></param>
        /// <param name="BreakLines"></param>
        /// <returns></returns>
        public static string ConsoleWriteLine(this DateTime dateTime, string Text, int BreakLines = 1) => dateTime.ConsoleWrite(Text, BreakLines.SetMinValue(1));

        /// <summary>
        /// Escreve uma linha no console colorindo palavras especificas
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="CustomColoredWords">Lista com as palavras e suas respectivas cores</param>

        public static string ConsoleWriteLine(this string Text, Dictionary<string, ConsoleColor> CustomColoredWords, int BreakLines = 1) => Text.ConsoleWrite(CustomColoredWords, BreakLines.SetMinValue(1));

        /// <summary>
        /// Escreve uma linha no console usando uma cor especifica
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Color">Cor</param>
        public static string ConsoleWriteLine(this string Text, ConsoleColor Color, int BreakLines = 1) => Text.ConsoleWrite(Color, BreakLines.SetMinValue(1));

        /// <summary>
        /// Escreve uma linha no console e pula um numero de linhas
        /// </summary>
        /// <param name="Text">Texto</param>
        public static string ConsoleWriteLine(this string Text, int BreakLines = 1) => Text.ConsoleWriteLine(System.Console.ForegroundColor, BreakLines);

        /// <summary>
        /// Le o proximo caractere inserido no console pelo usuário
        /// </summary>
        /// <returns></returns>
        public static char ReadChar() => System.Console.ReadKey().KeyChar;

        /// <summary>
        /// Le a proxima tecla pressionada pelo usuário
        /// </summary>
        /// <returns></returns>
        public static ConsoleKey ReadConsoleKey() => System.Console.ReadKey().Key;


    }
}