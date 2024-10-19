namespace ChessLogic
{
    public class Khight : Piece
    {
        public override PieceType Type => PieceType.Knight;
        public override Player Color { get; }

        public Khight(Player color)
        {
            Color = color;
        }
        public override Piece Copy()
        {
            Khight copy = new Khight(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private static IEnumerable<Position> PotencialtoPositions(Position from)
        {
            foreach (Direction vDir in new Direction[] { Direction.North, Direction.South })
            {
                foreach (Direction hDir in new Direction[] { Direction.East, Direction.West })
                {
                    yield return from + 2 * vDir + hDir;
                    yield return from + 2 * hDir + vDir;
                }
            }
        }

        private IEnumerable<Position> MovePositions(Position from, Board board)
        {
            return PotencialtoPositions(from).Where(pos => Board.isInside(pos) 
            && (board.isEmpty(pos) || board[pos].Color != Color));
        }

        public override IEnumerable<Move> GetMoves (Position from, Board board)
        {
            return MovePositions(from,board).Select(to => new NormalMove(from, to));
        }
    }
}
