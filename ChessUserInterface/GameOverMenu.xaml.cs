using ChessLogic;
using System.Windows;
using System.Windows.Controls;

namespace ChessUserInterface
{
    public partial class GameOverMenu : UserControl
    {
        public event Action<Options> OptionSelected;
        public GameOverMenu(GameState gameState)
        {
            InitializeComponent();

            Result result = gameState.Result;
            MenuText.Text = GetWinnerText(result.Winner);
            ReasonText.Text = GetReasonText(result.Reason, gameState.CurrentPlayer);
        }

        private static string GetWinnerText (Player player)
        {
            return player switch
            {
                Player.White => "Победили белые",
                Player.Black => "Победили чёрные",
                _ => "Ничья"
            };
        }

        private static string PlayerString (Player player)
        {
            return player switch
            { 
              Player.White => "Белые",
              Player.Black => "Чёрные",
              _ => ""
            };
        }

        private static string GetReasonText (EndReason reason, Player currentPlayer)
        {
            return reason switch
            {
                EndReason.Stalemate => $"Ничья - {PlayerString(currentPlayer)} не могут сделать ход",
                EndReason.Checkmate => $"Мат - {PlayerString(currentPlayer)} не могут сделать ход",
                EndReason.FyftyMoveRule => $"Правило 50-ти ходов",
                EndReason.NonMaterial => $"Нехватка материала",
                EndReason.ThreefoldRepetition => $"Троекратное повторение позиции",
                _ => ""
            };
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Options.Exit);
        }
    }
}
