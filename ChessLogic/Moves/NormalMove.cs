namespace ChessLogic
{
    public class NormalMove:Move
    {
        public override MoveType Type => MoveType.Normal;
        public override Position FromPosition { get; }
        public override Position ToPosition { get; }

        public NormalMove (Position fromPosition, Position toPosition)
        {
            FromPosition = fromPosition;
            ToPosition = toPosition;
        }

        public override bool Execute(Board board)
        {
            Piece piece = board[FromPosition];
            bool capture = !board.isEmpty(ToPosition);
            board[ToPosition] = piece;
            board[FromPosition] = null;
            piece.HasMoved = true;

            return capture || piece.Type == PieceType.Pawn;
        }
    }
}
