using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Extensions;

namespace Extensions.Console
{
    /// <summary>
    /// Métodos para manipulação de aplicações baseadas em <see cref="System.Console"/>
    /// </summary>
    public static class Cnsl
    {
        #region Public Methods

        /// <summary>
        /// Toca um Beep
        /// </summary>
        /// <param name="Times">Numero de beeps</param>
        public static string Beep(this string Text, int Times = 1)
        {
            for (int index = 1, loopTo = Times.SetMinValue(1); index <= loopTo; index++)
            {
                System.Console.Beep();
            }

            return Text;
        }

        /// <summary>
        /// Toca um beep especifico
        /// </summary>
        /// <param name="Frequency">Frequencia</param>
        /// <param name="Duration">Duracao em milisegundos</param>
        public static string Beep(this string Text, int Frequency, int Duration, int Times = 1)
        {
            for (int index = 1, loopTo = Times.SetMinValue(1); index <= loopTo; index++)
            {
                System.Console.Beep(Frequency.LimitRange(37, 32767), Duration);
            }

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
                System.Console.WriteLine(Util.EmptyString);
                BreakLines--;
            }
            return Text;
        }

        public static string ConsoleBreakLine(this int BreakLines) => ConsoleBreakLine(Util.EmptyString, BreakLines);

        public static string ConsoleBreakLine() => ConsoleBreakLine(1);

        /// <summary>
        /// Escreve uma data com descrição no console
        /// </summary>
        /// <param name="LogDateTime"></param>
        /// <param name="Text"></param>
        /// <param name="BreakLines"></param>
        /// <returns></returns>
        public static string ConsoleLog(this string Text, DateTime? LogDateTime = null, ConsoleColor? DateColor = null, ConsoleColor? MessageColor = null, string DateFormat = default, int BreakLines = 1)
        {
            DateFormat = DateFormat.IfBlank("yyyy-MM-dd HH: mm:ss");
            LogDateTime = LogDateTime ?? DateTime.Now;
            DateColor = DateColor ?? MessageColor ?? System.Console.ForegroundColor;
            MessageColor = MessageColor ?? DateColor;
            var timestring = LogDateTime.Value.ToString(DateFormat, CultureInfo.InvariantCulture);
            ConsoleWrite($"{timestring}", DateColor.Value);
            ConsoleWriteLine($" - {Text}", MessageColor.Value, BreakLines);
            return $"{timestring} - {Text}";
        }

        public static string ConsoleLog(this DateTime LogDateTime, string Text, ConsoleColor? DateColor = null, ConsoleColor? MessageColor = null, string DateFormat = default, int BreakLines = 1) => ConsoleLog(Text, LogDateTime, DateColor, MessageColor, DateFormat, BreakLines);

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
            var maincolor = CustomColoredWords.GetValueOr(string.Empty, lastcolor);
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
            System.Console.Write(Text ?? string.Empty);
            System.Console.ForegroundColor = lastcolor;
            return Text.ConsoleBreakLine(BreakLines);
        }

        /// <summary>
        /// Escreve o Texto de uma exception no console
        /// </summary>
        /// <param name="Exception">Texto</param>
        /// <param name="Message">Mensagem Adicional de erro</param>
        /// <param name="Color">Cor</param>
        public static T ConsoleWriteError<T>(this T Exception, string Message, string Separator, ConsoleColor Color = ConsoleColor.Red, int BreakLines = 1) where T : Exception => (T)new Exception(Message.IfBlank("Error"), Exception).ConsoleWriteError(Separator, Color, BreakLines);

        /// <summary>
        /// Escreve o Texto de uma exception no console
        /// </summary>
        /// <param name="Exception">Texto</param>
        /// <param name="Color">Cor</param>
        public static T ConsoleWriteError<T>(this T Exception, string Separator, ConsoleColor Color = ConsoleColor.Red, int BreakLines = 1) where T : Exception
        {
            Exception?.ToFullExceptionString(Separator).ConsoleWrite(Color, BreakLines.SetMinValue(1));
            return Exception;
        }

        /// <summary>
        /// Escreve o Texto de uma exception no console
        /// </summary>
        public static T ConsoleWriteError<T>(this T Exception, string Message) where T : Exception => Exception.ConsoleWriteError(Message, " => ");

        /// <summary>
        /// Escreve o Texto de uma exception no console
        /// </summary>
        public static T ConsoleWriteError<T>(this T Exception) where T : Exception => Exception.ConsoleWriteError(" >> ", ConsoleColor.Red, 1);

        /// <summary>
        /// Escreve o Texto de uma exception no console
        /// </summary>
        public static T ConsoleWriteError<T>(this T Exception, ConsoleColor Color, int BreakLines = 1) where T : Exception => Exception.ConsoleWriteError(" >> ", Color, BreakLines);

        /// <summary>
        /// Escreve o Texto de uma <typeparamref name="T"/> no console
        /// </summary>
        public static T ConsoleWriteError<T>(this T Exception, int BreakLines) where T : Exception => Exception.ConsoleWriteError(" >> ", ConsoleColor.Red, BreakLines);

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

        public static string ConsoleWriteSeparator(char Separator = '-', ConsoleColor? Color = null, int BreakLines = 1) => ConsoleWriteSeparator(Util.EmptyString, Separator, Color, BreakLines);

        /// <summary>
        /// Escreve um separador no console. Este separador pode conter um Texto
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Separator"></param>
        /// <param name="Color"></param>
        /// <param name="BreakLines"></param>
        /// <returns></returns>
        public static string ConsoleWriteSeparator(this string Text, char Separator = '-', ConsoleColor? Color = null, int BreakLines = 1)
        {
            if (Text.IsNotBlank())
            {
                Text = Text.Wrap(" ");
            }
            Color = Color ?? System.Console.ForegroundColor;
            try
            {
                Text = Text.Pad(System.Console.BufferWidth, Separator);
            }
            catch
            {
            }
            return ConsoleWriteLine(Text, Color.Value, BreakLines);
        }

        public static string ConsoleWriteTitle(this string Text, ConsoleColor? Color = null, int BreakLines = 1) => ConsoleWriteSeparator(Text, ' ', Color, BreakLines);

        public static string ConsoleWriteTitleBar(this string Text, ConsoleColor? Color = null, int BreakLines = 1, char BarChar = '-')
        {
            ConsoleWriteSeparator(BarChar, Color);
            var str = ConsoleWriteTitle(Text, Color);
            ConsoleWriteSeparator(BarChar, Color, BreakLines);
            return str;
        }

        /// <summary>
        /// Retorna o valor de um argumento de uma linha de comando
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ArgName"></param>
        /// <param name="ValueIfNull"></param>
        /// <returns></returns>
        public static T GetArgumentValue<T>(this string[] args, string ArgName, T ValueIfNull = default)
        {
            if (ArgName.IsBlank())
            {
                return ValueIfNull;
            }

            if (args.Contains(ArgName, StringComparer.InvariantCultureIgnoreCase))
            {
                var index = args.Select(x => x.ToUpperInvariant()).GetIndexOf(ArgName?.ToUpperInvariant());
                if (index > -1)
                {
                    var s = args.IfBlankOrNoIndex(index + 1, default);
                    return s.IsBlank() ? ValueIfNull : s.ChangeType<T>();
                }
            }
            return ValueIfNull;
        }



        /// <summary>
        /// Retorna o valor de um argumento de uma linha de comando
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ArgName"></param>
        /// <param name="ValueIfNull"></param>
        /// <returns></returns>
        public static string GetArgumentValue(this string[] args, string ArgName, string ValueIfNull = default) => GetArgumentValue<string>(args, ArgName, ValueIfNull);

        /// <summary>
        /// Le o proximo caractere inserido no console pelo usuário
        /// </summary>
        /// <returns></returns>
        public static char ReadChar(this ref char c)
        {
            c = System.Console.ReadKey().KeyChar;
            return c;
        }

        /// <summary>
        /// Le a proxima tecla pressionada pelo usuário
        /// </summary>
        /// <returns></returns>
        public static ConsoleKey ReadConsoleKey(this ref ConsoleKey Key)
        {
            Key = System.Console.ReadKey().Key;
            return Key;
        }

        #endregion Public Methods
    }
}