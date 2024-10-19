using System.Text.RegularExpressions;

namespace ChessUserInterface
{
    public class FenValidation
    {
        private static readonly string PIECE_POSITIONS_REGEX = "^([rnbqkpRNBQKP1-8]+\\/){7}[rnbqkpRNBQKP1-8]+$";
        private static readonly string SIDE_TO_MOVE_REGEX = "^[wb]$";
        private static readonly string CASTLING_REGEX = "^(K?Q?k?q?|\\-)$";
        private static readonly string EN_PASSANT_REGEX = "^([a-h][36]|\\-)$";
        private static readonly string HALF_MOVE_REGEX = "^\\d+$";
        private static readonly string FULL_MOVE_REGEX = "^\\d+$";

        public static bool IsValidFEN(string fen)
        {
            string[] parts = fen.Split(' ');
            if (parts.Length != 6)
            {
                return false;
            }

            return IsValidPiecePlacement(parts[0]) &&
                   IsValidSideToMove(parts[1]) &&
                   IsValidCastling(parts[2]) &&
                   IsValidEnPassant(parts[3]) &&
                   IsValidHalfMove(parts[4]) &&
                   IsValidFullMove(parts[5]);
        }


        private static bool IsValidPiecePlacement(string piecePlacement)
        {
            if (!Regex.IsMatch(piecePlacement, PIECE_POSITIONS_REGEX))
            {
                return false;
            }

            string[] rows = piecePlacement.Split('/');
            foreach (string row in rows)
            {
                int count = 0;
                foreach (char c in row)
                {
                    if (char.IsDigit(c))
                    {
                        count += c - '0';
                    }
                    else
                    {
                        count++;
                    }
                }
                if (count != 8)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsValidSideToMove(string sideToMove)
        {
            return Regex.IsMatch(sideToMove, SIDE_TO_MOVE_REGEX);
        }

        private static bool IsValidCastling(string castling)
        {
            return Regex.IsMatch(castling, CASTLING_REGEX);
        }

        private static bool IsValidEnPassant(string enPassant)
        {
            return Regex.IsMatch(enPassant, EN_PASSANT_REGEX);
        }

        private static bool IsValidHalfMove(string halfMove)
        {
            return Regex.IsMatch(halfMove, HALF_MOVE_REGEX);
        }

        private static bool IsValidFullMove(string fullMove)
        {
            return Regex.IsMatch(fullMove, FULL_MOVE_REGEX);
        }

    }
}
