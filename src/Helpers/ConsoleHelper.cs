using System;

namespace YetAnotherFlickrUploader.Helpers
{
    public static class ConsoleHelper
    {
        private static int? _cursorPosY;
        private static int? _cursorPosX;

        public static void WriteDebug(string message)
        {
            Write(ConsoleColor.Gray, message);
        }

        public static void WriteDebug(string format, params object[] args)
        {
            Write(ConsoleColor.Gray, format, args);
        }

        public static void WriteInfo(string message)
        {
            Write(ConsoleColor.White, message);
        }

        public static void WriteDebugLine(string message)
        {
            WriteLine(ConsoleColor.Gray, message);
        }

        public static void WriteDebugLine(string format, params object[] args)
        {
            WriteLine(ConsoleColor.Gray, format, args);
        }

        public static void WriteInfoLine(string message)
        {
            WriteLine(ConsoleColor.White, message);
        }

        public static void WriteInfoLine(string format, params object[] args)
        {
            WriteLine(ConsoleColor.White, format, args);
        }

        public static void WriteWarningLine(string message)
        {
            WriteLine(ConsoleColor.DarkYellow, message);
        }

        public static void WriteWarningLine(string format, params object[] args)
        {
            WriteLine(ConsoleColor.DarkYellow, format, args);
        }

        public static void WriteErrorLine(string message)
        {
            WriteLine(ConsoleColor.Red, message);
        }

        public static void WriteErrorLine(string format, params object[] args)
        {
            WriteLine(ConsoleColor.Red, format, args);
        }

        public static void WriteException(Exception e)
        {
            var fc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Exception details:");
            Console.WriteLine(e.Message);
            Console.WriteLine(e.Source);
            if (e.InnerException != null)
            {
                Console.WriteLine("Inner exception:");
                Console.WriteLine(e.InnerException.Message);
                Console.WriteLine(e.InnerException.Source);
                Console.WriteLine("Stack:");
                Console.WriteLine(e.InnerException.StackTrace);
            }
            else
            {
                Console.WriteLine("Stack:");
                Console.WriteLine(e.StackTrace);
            }
            Console.ForegroundColor = fc;
        }

        public static void SaveCursorPosition()
        {
            _cursorPosY = Console.CursorTop;
            _cursorPosX = Console.CursorLeft;
        }

        public static void RestoreCursorPosition()
        {
            SetCursorPosition(_cursorPosX, _cursorPosY);
        }

        public static void SetCursorPosition(int? left, int? top = null)
        {
            if (top.HasValue)
                Console.CursorTop = top.Value;
            if (left.HasValue)
                Console.CursorLeft = left.Value;
        }

        #region Private methods

        private static void Write(ConsoleColor color, string message)
        {
            var fc = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = fc;
        }

        private static void Write(ConsoleColor color, string format, params object[] args)
        {
            var fc = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(format, args);
            Console.ForegroundColor = fc;
        }

        private static void WriteLine(ConsoleColor color, string message)
        {
            var fc = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = fc;
        }

        private static void WriteLine(ConsoleColor color, string format, params object[] args)
        {
            var fc = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(format, args);
            Console.ForegroundColor = fc;
        }

        #endregion
    }
}
