namespace ChessLogic
{
    public class Castle : Move
    {
        public override MoveType Type { get; }
        public override Position FromPosition { get; }
        public override Position ToPosition { get; }

        private readonly Direction kingDir;
        private readonly Position rookFromPos;
        private readonly Position rookToPos;

        public Castle(MoveType type, Position KingPos)
        {
            Type = type;
            FromPosition = KingPos;

            if (MoveType.CastleKS == type)
            {
                kingDir = Direction.East;
                ToPosition = new Position(KingPos.Row, 6);
                rookFromPos = new Position(KingPos.Row, 7);
                rookToPos = new Position(KingPos.Row, 5);
            } else if (MoveType.CastleQS == type)
            {
                kingDir = Direction.West;
                ToPosition = new Position (KingPos.Row, 2);
                rookFromPos = new Position(KingPos.Row, 0);
                rookToPos = new Position(KingPos.Row, 3);
            }
        }

        public override bool Execute(Board board)
        {
            new NormalMove(FromPosition, ToPosition).Execute(board);
            new NormalMove (rookFromPos, rookToPos).Execute(board);

            return false;
        }

        public override bool isLegal (Board board)
        {
            Player player = board[FromPosition].Color;

            if (board.isInCheck(player))
            {
                return false;
            }

            Board copy = board.Copy();
            Position KingPosInCopy = FromPosition;

            for (int i = 0; i < 2; i++)
            {
                new NormalMove(KingPosInCopy, KingPosInCopy + kingDir).Execute(copy);
                KingPosInCopy += kingDir;

                if (copy.isInCheck(player))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
