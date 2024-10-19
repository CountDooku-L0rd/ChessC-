using System.Text;

namespace ChessLogic
{
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }

        public Result Result { get; private set; } = null;
        public String Fen {  get; set; }

        private int fiftyMovesRule = 0;
        private int MoveCounter = 1;

        private readonly Dictionary<string, int> stateHistory = new Dictionary<string, int>();

        public GameState(Player player, Board board) 
        {
            CurrentPlayer = player;
            Board = board;

            Fen = GetFen();
            stateHistory[Fen] = 1;
        }

        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if (Board.isEmpty(pos) || Board[pos].Color != CurrentPlayer)
            {
                return Enumerable.Empty<Move>();
            }

            Piece piece = Board[pos];
            IEnumerable<Move> moveCandidates = piece.GetMoves(pos, Board);
            return moveCandidates.Where(move => move.isLegal(Board));
        }

        public void MakeMove (Move move)
        {
            MoveCounter++;
            Board.setPawnSkipPosition(CurrentPlayer, null);
            bool captureOrPawn = move.Execute(Board);

            if (captureOrPawn) 
            {
                fiftyMovesRule = 0;
                stateHistory.Clear();
            }
            else
            {
                fiftyMovesRule++;
            }
            UpdateFen();
            CurrentPlayer = CurrentPlayer.Opponent();
            CheckForGameOver();
        }

        public IEnumerable<Move> AllLegalMovesFor(Player player)
        {
            IEnumerable<Move> moveCandidates = Board.PiecePositionsFor(player).SelectMany(pos =>
            {
                Piece piece = Board[pos];
                return piece.GetMoves(pos, Board);
            });

            return moveCandidates.Where(move => move.isLegal(Board));
        }

        private void CheckForGameOver()
        {
            if (!AllLegalMovesFor(CurrentPlayer).Any())
            {
                if (Board.isInCheck(CurrentPlayer))
                {
                    Result = Result.Win(CurrentPlayer.Opponent());
                }
                else
                {
                    Result = Result.Draw(EndReason.Stalemate);
                }
            }
            else if (Board.InsufficientMaterial())
            {
                Result = Result.Draw(EndReason.NonMaterial);
            }
            else if (FyftyMovesRule())
            {
                Result = Result.Draw(EndReason.FyftyMoveRule);
            }
            else if (ThreefoldRepetition())
            {
                Result = Result.Draw(EndReason.ThreefoldRepetition);
            }
        }

        public bool isGameOver()
        {
            return Result != null;
        }

        private bool FyftyMovesRule()
        {
            int fullMoves = fiftyMovesRule / 2;
            return fullMoves == 50;
        }

        public string GetFen()
        {
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row <= 7; row++)
            {
                int emptyFiles = 0;
                for (int column = 0; column <= 7; column++)
                {
                    Piece piece = Board[row, column];
                    if (piece == null)
                    {
                        emptyFiles++;
                    }
                    else
                    {
                        if (emptyFiles > 0)
                        {
                            sb.Append(emptyFiles);
                            emptyFiles = 0;
                        }
                        sb.Append(Piece.GetFenCharacter(piece));
                    }
                }
                if (emptyFiles > 0)
                {
                    sb.Append(emptyFiles);
                }
                if (row >= 0 && row < 7)
                {
                    sb.Append('/');
                }
            }
            sb.Append(' ');
            sb.Append(CurrentPlayer == Player.White ? 'w' : 'b');
            sb.Append(' ');
            bool anyCastlingRights = false;

            Position wKingPos = new Position (7, 4);
            Position bKingPos = new Position (0, 4);
            King wKing = new King(Player.White);
            King bKing = new King(Player.Black);

            if ((CurrentPlayer == Player.White && Board.CastleRightKS(CurrentPlayer)) || (CurrentPlayer.Opponent() == Player.White && Board.CastleRightKS(CurrentPlayer.Opponent())))
            {
                sb.Append('K');
                anyCastlingRights = true;
            }
            if ((CurrentPlayer == Player.White && Board.CastleRightQS(CurrentPlayer)) || (CurrentPlayer.Opponent() == Player.White && Board.CastleRightQS(CurrentPlayer.Opponent())))
            {
                sb.Append('Q');
                anyCastlingRights = true;
            }
            if ((CurrentPlayer == Player.Black && Board.CastleRightKS(CurrentPlayer)) || (CurrentPlayer.Opponent() == Player.Black && Board.CastleRightKS(CurrentPlayer.Opponent())))
            {
                sb.Append('k');
                anyCastlingRights = true;
            }
            if ((CurrentPlayer == Player.Black && Board.CastleRightQS(CurrentPlayer)) || (CurrentPlayer.Opponent() == Player.Black && Board.CastleRightQS(CurrentPlayer.Opponent())))
            {
                sb.Append('q');
                anyCastlingRights = true;
            }
            if (!anyCastlingRights)
            {
                sb.Append('-');
            }
            sb.Append(' ');

            if (!Board.CanCaptureEnPeasant(CurrentPlayer))
            {
                sb.Append('-');
            } 
            else
            {
                Position pos = Board.GetPawnSkipPosition(CurrentPlayer.Opponent());
                char file = (char)('a' + pos.Column);
                int rank = 8 - pos.Row;
                sb.Append(file);
                sb.Append(rank);
            }

            return sb.ToString();
        }

        public String GetFinalFen()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GetFen());
            sb.Append(' ');
            sb.Append(fiftyMovesRule);
            sb.Append(' ');
            sb.Append(MoveCounter / 2);

            return sb.ToString();
        }

        private void UpdateFen()
        {
            Fen = this.GetFen();

            if (!stateHistory.ContainsKey(Fen))
            {
                stateHistory[Fen] = 1;
            }
            else
            {
                stateHistory[Fen]++;
            }
        }

        private bool ThreefoldRepetition()
        {
            return stateHistory[Fen] == 3;
        }
    }
}
