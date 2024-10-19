using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ChessLogic;
using ChessLogic.Moves;
using Stockfish.NET;

namespace ChessUserInterface
{
    public partial class MainWindow : Window
    {
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highLights = new Rectangle[8, 8];
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();

        private GameState gameState;
        private Position selectedPos = null;
        Player player;

        List<string> fenStrings = new List<string>();
        static string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
        static string rootDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(projectDirectory, "..", "..", "..", ".."));
        string filePath = System.IO.Path.Combine(rootDirectory, "..", "fen.txt");

        private readonly int BotSearchTimeMs = 1000;
        public static string stockfishPath = System.IO.Path.Combine(rootDirectory, "Stockfish", "stockfish_20090216_x64_ssse.exe");
        private IStockfish stockfish = new Stockfish.NET.Stockfish(stockfishPath);


        public MainWindow(string fenStr, int Depth, int color)
        {
            InitializeComponent();
            InitializeBoard();
            String fen = fenStr;
            fenStrings.Add(fen);
            string[] parts = fen.Split(" ");
            String moveTime = parts[1];
            Player toMove;

            if (moveTime == "w" || moveTime == "W")
            {
                toMove = Player.White;
            } else if (moveTime == "b" || moveTime == "B")
            {
                toMove = Player.Black;
            } else
            {
                Application.Current.Shutdown();
                return;
            }


            if (color == 1) {
                player = Player.White;
            } else if (color == 2)
            {
                player = Player.Black;
            }
            
            gameState = new GameState(toMove, Board.Initial(fen));
            gameState.Fen = fen;
            drawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);

            stockfish.Depth = Depth;

            if (player != toMove)
            {
                HandleNextMoveAsync();
            }
        }

        private void InitializeBoard() 
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Image image = new Image();
                    pieceImages[i, j] = image;
                    PieceGrid.Children.Add(image);

                    Rectangle highLight = new Rectangle();
                    highLights[i, j] = highLight;
                    HighLightGrid.Children.Add(highLight);
                }
            }
        }

        private void drawBoard(Board board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece piece = board[i, j];
                    pieceImages[i, j].Source = Images.GetImage(piece);
                }
            }
        }

        private void CacheMoves(IEnumerable<Move> moves)
        {
            moveCache.Clear();

            foreach(Move move in moves)
            {
                moveCache[move.ToPosition] = move;
            }
        }

        private void ShowHightLights()
        {
            Color color = Color.FromArgb(150, 125, 255, 125);

            foreach (Position to in moveCache.Keys)
            {
                highLights[to.Row,to.Column].Fill = new SolidColorBrush(color);
            }
        }

        private void HideHightLights()
        {
            foreach (Position to in moveCache.Keys)
            {
                highLights[to.Row, to.Column].Fill = Brushes.Transparent;
            }
        }

        private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (isMenuOnScreen())
            {
                return;
            }

            if (player != gameState.CurrentPlayer)
            {
                return;
            }

            Point point = e.GetPosition(BoardGrid);
            Position pos = ToSquarePostion(point);

            if (selectedPos == null)
            {
                OnFromPositionSelected(pos);
            }
            else
            {
                OnToPositionSelected(pos);
            }
        }

        private void OnFromPositionSelected(Position pos)
        {
            IEnumerable<Move> moves = gameState.LegalMovesForPiece(pos);

            if (moves.Any())
            {
                selectedPos = pos;
                CacheMoves(moves);
                ShowHightLights();
            }
        }

        private void OnToPositionSelected(Position pos)
        {
            selectedPos = null;
            HideHightLights();

            if (moveCache.TryGetValue(pos, out Move move))
            {
                if (move.Type  == MoveType.PawnPromotion)
                {
                    HandlePromotion(move.FromPosition, move.ToPosition);
                }
                else
                {
                    HandleMove(move);
                }
            }
        }

        private void HandlePromotion (Position From, Position To)
        {
            pieceImages[To.Row, To.Column].Source = Images.GetImage(gameState.CurrentPlayer, PieceType.Pawn);
            pieceImages[From.Row, From.Column].Source = null;

            PromotionMenu promotionMenu = new PromotionMenu(gameState.CurrentPlayer);
            MenuContainer.Content = promotionMenu;

            promotionMenu.PieceSelected += type =>
            {
                MenuContainer.Content = null;
                Move promMove = new PawnPromotion(From, To, type);
                HandleMove(promMove);
            };
        }

        private void HandleMove (Move move)
        {
            gameState.MakeMove(move);
            drawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
            fenStrings.Add(gameState.GetFinalFen());

            if (gameState.isGameOver())
            {
                ShowGameOver();
                WriteFenStringsToFile(fenStrings, filePath);
            }
            
            _ = HandleNextMoveAsync();
        }

        private async Task HandleNextMoveAsync()
        {
            await Task.Delay(100);
            if (gameState.CurrentPlayer == player.Opponent())
            {
                await BotMove();
            }
        }

        private Position ToSquarePostion (Point point)
        {
            double squareSize = BoardGrid.ActualWidth / 8;
            int row = (int)(point.Y / squareSize);
            int column = (int)(point.X / squareSize);
            return new Position(row, column);
        }

        private void SetCursor(Player player)
        {
            if (player == Player.White)
            {
                Cursor = ChessCursors.WhiteCursor;
            }
            else
            {
                Cursor = ChessCursors.BlackCursor;
            }
        }

        public bool isMenuOnScreen()
        {
            return MenuContainer.Content != null;
        }

        private void ShowGameOver()
        {
            GameOverMenu gameOverMenu = new GameOverMenu(gameState);
            MenuContainer.Content = gameOverMenu;

            gameOverMenu.OptionSelected += Options =>
            {
                MainMenu mainMenu = new MainMenu();
                mainMenu.Show();
                this.Close();
            };
        }

        static void WriteFenStringsToFile(List<string> fenStrings, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (string fen in fenStrings)
                {
                    writer.WriteLine(fen);
                }
            }
        }

        private async Task BotMove()
        {
            string fen = gameState.GetFinalFen();
            stockfish.SetFenPosition(fen);
            String bestMoveResponse = stockfish.GetBestMove();
            if (bestMoveResponse != null)
            {
                if (bestMoveResponse.Length == 4)
                {
                    try
                    {
                        Position from = new Position(8 - (bestMoveResponse[1] - '0'), bestMoveResponse[0] - 'a');
                        Position to = new Position(8 - (bestMoveResponse[3] - '0'), bestMoveResponse[2] - 'a');

                        if (bestMoveResponse == "e1g1" || bestMoveResponse == "e8g8")
                        {
                            Move move = new Castle(MoveType.CastleKS, from);
                            HandleMove(move);
                        }
                        else if (bestMoveResponse == "e1c1" || bestMoveResponse == "e8c8")
                        {
                            Move move = new Castle(MoveType.CastleQS, from);
                            HandleMove(move);
                        }
                        else if (gameState.Board.GetPawnSkipPosition(gameState.CurrentPlayer.Opponent()) == to)
                        {
                            Move move = new EnPeasant(from, to);
                            HandleMove(move);
                        }
                        else if (gameState.Board[from].Type == PieceType.Pawn && (from.Row - to.Row == 2 || to.Row - from.Row == 2))
                        {
                            Move move = new DoublePawn(from, to);
                            HandleMove(move);
                        }
                        else
                        {
                            Move move = new NormalMove(from, to);
                            HandleMove(move);
                        }

                        
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        MessageBox.Show("Ошибка конвертации позиции: " + ex.Message);
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        MessageBox.Show("Ошибка доступа к массиву: " + ex.Message);
                    }
                } 
                else if (bestMoveResponse.Length == 5)
                {
                    Position from = new Position(8 - (bestMoveResponse[1] - '0'), bestMoveResponse[0] - 'a');
                    Position to = new Position(8 - (bestMoveResponse[3] - '0'), bestMoveResponse[2] - 'a');
                    //MessageBox.Show("Бот сделал ход: " + bestMoveResponse.ToString());
                    Char pieceChar = bestMoveResponse[4];

                    Move move = new PawnPromotion(from, to, GetPieceType(pieceChar));
                    HandleMove(move);
                }
            }
            else if (bestMoveResponse == null)
            {
                if (gameState.isGameOver())
                {
                    ShowGameOver();
                    WriteFenStringsToFile(fenStrings, filePath);
                }
            }
        }

        private PieceType GetPieceType(char pieceChar)
        {
            return pieceChar switch
            {
                'q' => PieceType.Queen,
                'Q' => PieceType.Queen,
                'r' => PieceType.Rook,
                'R' => PieceType.Rook,
                'b' => PieceType.Bishop,
                'B' => PieceType.Bishop,
                'n' => PieceType.Knight,
                'N' => PieceType.Knight,
                _ => PieceType.Queen,
            };
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isMenuOnScreen() && e.Key == Key.Escape)
            {
                ShowPauseMenu();
            }
        }

        private void ShowPauseMenu()
        {
            PauseMenu pauseMenu = new PauseMenu();
            MenuContainer.Content = pauseMenu;

            pauseMenu.OptionSelected += option =>
            {
                MenuContainer.Content = null;

                if (option == Options.Exit)
                {
                    MainMenu mainMenu = new MainMenu();
                    mainMenu.Show();
                    this.Close();
                }
            };
        }
    }
}