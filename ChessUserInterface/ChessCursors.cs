using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ChessUserInterface
{
    public static class ChessCursors
    {
        public static readonly Cursor WhiteCursor = LoadCursor("Материалы/CursorW.cur");
        public static readonly Cursor BlackCursor = LoadCursor("Материалы/CursorB.cur");
        private static  Cursor LoadCursor(string filePath)
        {
            Stream stream = Application.GetResourceStream(new Uri(filePath, UriKind.Relative)).Stream;
            return new Cursor(stream, true);
        }
    }
}
