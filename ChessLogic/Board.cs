using ChessLogic.Moves;

namespace ChessLogic
{
    public class Board
    {
        PieceFabrica pieceFabrica = new PieceFabrica();
        public Player Color { get; private set; }
        public int moveNumber { get; private set; }

        private readonly Piece[,] pieces = new Piece[8, 8];

        private readonly Dictionary<Player, Position> pawnSkipPositions = new Dictionary<Player, Position>
        {
            {Player.White, null },
            {Player.Black, null}
        };

        public Piece this[int row, int column]
        {
            get { return pieces[row, column]; }
            set { pieces[row, column] = value; }
        }

        public Piece this[Position pos]
        {
            get { return this[pos.Row, pos.Column]; }
            set { this[pos.Row, pos.Column] = value; }
        }

        public Position GetPawnSkipPosition (Player player)
        {
            return pawnSkipPositions[player];
        }

        public void setPawnSkipPosition (Player player, Position pos)
        {
            pawnSkipPositions[player] = pos;
        }

        public static Board Initial(String fen)
        {
            Board board = new Board();
            board.AddStartFENPosition(fen);
            return board;
        }

        private void AddStartFENPosition(string fen)
        {
            string[] parts = fen.Split(" ");

            if (parts.Length != 6) return;

            string PiecePosition = parts[0];
            string[] fenRows = PiecePosition.Split("/");
            string EnMove = parts[3];
            int rows, columns;
            Player player = new Player();

            if (parts[1] == "b" || parts[1] == "B")
            {
                player = Player.Black;
            }  
            else
            {
                player = Player.White;
            }

            if (EnMove != "-")
            {
                columns = EnMove[0] - 'a';
                rows = 8 - (EnMove[1] - '0');
                Position pos = new Position(rows, columns);
                setPawnSkipPosition(player.Opponent(), pos);
            }

            for (int i = 0; i < fenRows.Length; i++)
            {
                String figuresRow = fenRows[i];
                int row = i + 1;
                int columnIndex = 0;
                for (int j = 0; j < figuresRow.Length; j++)
                {
                    char ch = figuresRow[j];

                    if (Char.IsNumber(ch))
                    {
                        columnIndex += Convert.ToInt32(Char.GetNumericValue(ch));
                    }
                    else
                    {
                        this[row - 1, columnIndex] = pieceFabrica.fromFenChar(ch);
                        columnIndex++;
                    }
                }
            }
            Color = (parts[1] == "b") ? Player.Black : Player.White;
            moveNumber = int.Parse(parts[5]);
        }

        public static bool isInside(Position pos)
        {
            return pos.Row >= 0 && pos.Row < 8 && pos.Column >= 0 && pos.Column < 8;
        }

        public bool isEmpty (Position pos)
        {
            return this[pos] == null;
        }

        public IEnumerable<Position> PiecePositions()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Position pos = new Position(i, j);
                    if (!isEmpty(pos))
                    {
                        yield return pos;
                    }
                }
            }
        }

        public IEnumerable<Position> PiecePositionsFor (Player player)
        {
            return PiecePositions().Where(pos => this[pos].Color == player);
        }

        public bool isInCheck(Player player)
        {
            return PiecePositionsFor(player.Opponent()).Any(pos =>
            {
                Piece piece = this[pos];
                return piece.CanCaptureOpponentKing(pos,this);
            });
        }

        public Board Copy()
        {
            Board copy = new Board();

            foreach (Position pos in PiecePositions())
            {
                copy[pos] = this[pos].Copy();
            }

            return copy;
        }

        public Counting countPieces()
        {
            Counting counting = new Counting();

            foreach (Position pos in PiecePositions())
            {
                Piece piece = this[pos];
                counting.Increment(piece.Color, piece.Type);
            }

            return counting;
        }

        public bool InsufficientMaterial()
        {
            Counting counting = countPieces();

            return isKingVKing(counting) || isKingBishopVKing(counting) || isKingKnightVKing(counting) || isKingBishopVKingBishop(counting);
        }

        private static bool isKingVKing (Counting counting)
        {
            return counting.TotalCount == 2;
        }

        private static bool isKingBishopVKing (Counting counting)
        {
            return counting.TotalCount == 3 && (counting.White(PieceType.Bishop) == 1 || counting.Black(PieceType.Bishop) == 1);
        }

        private static bool isKingKnightVKing(Counting counting)
        {
            return counting.TotalCount == 3 && (counting.White(PieceType.Knight) == 1 || counting.Black(PieceType.Knight) == 1);
        }

        private bool isKingBishopVKingBishop (Counting counting)
        {
            if (counting.TotalCount != 4)
            {
                return false;
            }

            if (counting.White(PieceType.Bishop) != 1 || counting.Black(PieceType.Bishop) != 1)
            {
                return false;
            }

            Position wBishopPos = FindPiece(Player.White, PieceType.Bishop);
            Position bBishopPos = FindPiece(Player.Black, PieceType.Bishop);

            return wBishopPos.squareColor() == bBishopPos.squareColor();
        }

        private Position FindPiece(Player color, PieceType type)
        {
            return PiecePositionsFor(color).First(pos => this[pos].Type == type);
        }

        private bool isUnmovedKingAndRook(Position KingPos, Position RookPos)
        {
            if (isEmpty(KingPos) || isEmpty(RookPos))
            {
                return false;
            }

            Piece King = this[KingPos];
            Piece Rook = this[RookPos];

            return King.Type == PieceType.King && Rook.Type == PieceType.Rook && !King.HasMoved && !Rook.HasMoved;
        }

        public bool CastleRightKS (Player player)
        {
            return player switch
            {
                Player.White => isUnmovedKingAndRook(new Position (7, 4), new Position (7, 7)),
                Player.Black => isUnmovedKingAndRook(new Position (0, 4), new Position (0, 7)),
                _ => false
            };
        }

        public bool CastleRightQS(Player player)
        {
            return player switch
            {
                Player.White => isUnmovedKingAndRook(new Position(7, 4), new Position(7, 0)),
                Player.Black => isUnmovedKingAndRook(new Position(0, 4), new Position(0, 0)),
                _ => false
            };
        }

        private bool HasPawnInPosition(Player player, Position[] pawnPositions, Position skipPos)
        {
            foreach (Position pos in pawnPositions.Where(isInside))
            {
                Piece piece = this[pos];
                if (piece == null || piece.Color!=player || piece.Type != PieceType.Pawn) 
                {
                    continue;
                }

                EnPeasant move = new EnPeasant(pos, skipPos);
                if (move.isLegal(this))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanCaptureEnPeasant(Player player)
        {
            Position skipPos = GetPawnSkipPosition(player.Opponent());

            if (skipPos == null) 
            {
                return false;
            }

            Position[] pawnPositions = player switch
            {
                Player.White => new Position[] { skipPos + Direction.SouthWest, skipPos + Direction.SouthEast },
                Player.Black => new Position[] { skipPos + Direction.NorthWest, skipPos + Direction.NorthEast },
                _ => Array.Empty<Position>()
            };

            return HasPawnInPosition(player, pawnPositions, skipPos);
        }
    }
}
