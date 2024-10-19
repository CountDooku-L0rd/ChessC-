using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChessLogic;

namespace ChessUserInterface
{
    public static class Images
    {
        private static readonly Dictionary<PieceType, ImageSource> WhiteSource = new()
        {
            {PieceType.Pawn, LoadImage("Материалы/PawnW.png") },
            {PieceType.Bishop, LoadImage("Материалы/BishopW.png") },
            {PieceType.Knight, LoadImage("Материалы/KnightW.png") },
            {PieceType.Rook, LoadImage("Материалы/RookW.png") },
            {PieceType.Queen, LoadImage("Материалы/QueenW.png") },
            {PieceType.King, LoadImage("Материалы/KingW.png") }
        };
        private static readonly Dictionary<PieceType, ImageSource> BlackSource = new()
        {
            {PieceType.Pawn, LoadImage("Материалы/PawnB.png") },
            {PieceType.Bishop, LoadImage("Материалы/BishopB.png") },
            {PieceType.Knight, LoadImage("Материалы/KnightB.png") },
            {PieceType.Rook, LoadImage("Материалы/RookB.png") },
            {PieceType.Queen, LoadImage("Материалы/QueenB.png") },
            {PieceType.King, LoadImage("Материалы/KingB.png") }
        };

        private static ImageSource LoadImage(string filePath)
        {
            return new BitmapImage(new Uri(filePath, UriKind.Relative));
        }

        public static ImageSource GetImage(Player color, PieceType type)
        {
            return color switch
            {
                Player.White => WhiteSource[type],
                Player.Black => BlackSource[type],
                _ => null
            };
        }

        public static ImageSource GetImage(Piece piece)
        {
            if (piece == null) return null;
            return GetImage(piece.Color, piece.Type);
        }
    }
}
