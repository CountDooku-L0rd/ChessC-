namespace ChessLogic
{
    public class PawnPromotion : Move 
    {
        public override MoveType Type => MoveType.PawnPromotion;
        public override Position FromPosition { get; }
        public override Position ToPosition { get; }

        private readonly PieceType new_type;

        public PawnPromotion (Position fromPosition, Position toPosition, PieceType new_type)
        {
            FromPosition = fromPosition;
            ToPosition = toPosition;
            this.new_type = new_type;
        }

        private Piece CreatePromotionPiece(Player color)
        {
            return new_type switch
            {
                PieceType.Knight => new Khight(color),
                PieceType.Bishop => new Bishop(color),
                PieceType.Rook => new Rook(color),
                _ => new Queen(color)
            };
        }

        public override bool Execute(Board board)
        {
            Piece pawn = board[FromPosition];
            board[FromPosition] = null;

            Piece promotionPiece = CreatePromotionPiece(pawn.Color);
            promotionPiece.HasMoved = true;
            board[ToPosition] = promotionPiece;

            return true;
        }
    }
}
