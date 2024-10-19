namespace ChessLogic
{
    public abstract class Move
    {
        public abstract MoveType Type { get; }
        public abstract Position FromPosition { get; }
        public abstract Position ToPosition { get; }
        public abstract bool Execute(Board board);
        public virtual bool isLegal(Board board)
        {
            Player player = board[FromPosition].Color;
            Board boardcopy = board.Copy();
            Execute(boardcopy);
            return !boardcopy.isInCheck(player);
        }
    }
}
