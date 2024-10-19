namespace ChessLogic
{
    public abstract class Piece
    {
        public abstract PieceType Type { get; }
        public abstract Player Color { get; }
        public bool HasMoved { get; set; } = false;
        public abstract Piece Copy();
        public abstract IEnumerable<Move> GetMoves(Position from, Board board);
        protected IEnumerable<Position> MovePositionInDir(Position from, Board board, Direction dir)
        {
            for (Position pos = from + dir; Board.isInside(pos); pos += dir)
            {
                if (board.isEmpty(pos))
                {
                    yield return pos;
                    continue;
                }

                Piece piece = board[pos];

                if (piece.Color != Color)
                {
                    yield return pos;
                }

                yield break;
            }
        }
        protected IEnumerable<Position> MovePositionInDirs (Position from, Board board, Direction[] dirs)
        {
            return dirs.SelectMany(dir => MovePositionInDir(from, board, dir));
        }

        public virtual bool CanCaptureOpponentKing(Position from, Board board)
        {
            return GetMoves(from, board).Any(move =>
            {
                Piece piece = board[move.ToPosition];
                return piece !=null && piece.Type == PieceType.King;
            });
        }

        public static string GetFenCharacter(Piece piece)
        {
            if (piece.Color == Player.White)
            {
                if (piece.Type == PieceType.Pawn)
                {
                    return "P";
                }
                else if (piece.Type == PieceType.Queen)
                {
                    return "Q";
                }
                else if (piece.Type == PieceType.Rook)
                {
                    return "R";
                }
                else if (piece.Type == PieceType.Knight)
                {
                    return "N";
                }
                else if (piece.Type == PieceType.Bishop)
                {
                    return "B";
                }
                else if (piece.Type == PieceType.King)
                {
                    return "K";
                }
                else return " ";
            } 
            else if (piece.Color == Player.Black)
            {
                if (piece.Type == PieceType.Pawn)
                {
                    return "p";
                }
                else if (piece.Type == PieceType.Queen)
                {
                    return "q";
                }
                else if (piece.Type == PieceType.Rook)
                {
                    return "r";
                }
                else if (piece.Type == PieceType.Knight)
                {
                    return "n";
                }
                else if (piece.Type == PieceType.Bishop)
                {
                    return "b";
                }
                else if (piece.Type == PieceType.King)
                {
                    return "k";
                }
                else return " ";
            } 
            else return " ";
        }
    }
}
