namespace ChessLogic.Moves
{
    public class EnPeasant : Move
    {
        public override MoveType Type => MoveType.EnPeasant;
        public override Position FromPosition { get; }
        public override Position ToPosition { get; }

        private readonly Position capturePos;

        public EnPeasant (Position fromPosition, Position toPosition)
        {
            FromPosition = fromPosition;
            ToPosition = toPosition;

            capturePos = new Position (fromPosition.Row, toPosition.Column);
        }

        public override bool Execute(Board board)
        {
            new NormalMove(FromPosition, ToPosition).Execute(board);
            board[capturePos] = null;

            return true;
        }
    }
}
