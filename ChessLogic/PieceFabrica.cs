namespace ChessLogic
{
    public class PieceFabrica
    {
        public Piece fromFenChar(char fenChar)
        {
            switch (fenChar)
            {
                case 'p':
                    return new Pawn(Player.Black);
                case 'P':
                    return new Pawn(Player.White);


                case 'r':
                    return new Rook(Player.Black);
                case 'R':
                    return new Rook(Player.White);


                case 'n':
                    return new Khight(Player.Black);
                case 'N':
                    return new Khight(Player.White);


                case 'b':
                    return new Bishop(Player.Black);
                case 'B':
                    return new Bishop(Player.White);


                case 'q':
                    return new Queen(Player.Black);
                case 'Q':
                    return new Queen(Player.White);


                case 'k':
                    return new King(Player.Black);
                case 'K':
                    return new King(Player.White);

                default: return null;
            }
        }
    }
}
