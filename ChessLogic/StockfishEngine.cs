using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class StockfishEngine
    {
        private readonly Process stockfishProcess;

        private readonly string stockfishPath = @"C:\Users\Артём Плешаков\Downloads\stockfish-windows-x86-64-sse41-popcnt\stockfish\stockfish-windows-x86-64-sse41-popcnt.exe";


        public StockfishEngine(string stockfishPath)
        {
            this.stockfishPath = stockfishPath;
            stockfishProcess = new Process();
        }

        public async Task<string> GetBestMoveAsync(string fen, int searchTimeMs)
        {
            // Запускаем процесс Stockfish
            stockfishProcess.StartInfo.FileName = stockfishPath;
            stockfishProcess.StartInfo.UseShellExecute = false;
            stockfishProcess.StartInfo.RedirectStandardInput = true;
            stockfishProcess.StartInfo.RedirectStandardOutput = true;
            stockfishProcess.StartInfo.CreateNoWindow = true;
            stockfishProcess.Start();

            // Отправляем текущую позицию на доске в Stockfish
            stockfishProcess.StandardInput.WriteLine("position fen " + fen);

            // Запрашиваем лучший ход с ограничением по времени
            stockfishProcess.StandardInput.WriteLine("go movetime " + searchTimeMs);

            // Ждем ответа от Stockfish
            string bestMove = await Task.Run(() =>
            {
                string output = stockfishProcess.StandardOutput.ReadToEnd();
                return ParseBestMove(output);
            });

            // Останавливаем процесс Stockfish
            stockfishProcess.StandardInput.WriteLine("quit");
            stockfishProcess.WaitForExit();

            return bestMove;
        }

        private string ParseBestMove(string output)
        {
            string[] lines = output.Split('\n');
            foreach (string line in lines)
            {
                if (line.StartsWith("bestmove"))
                {
                    string[] parts = line.Split(' ');
                    if (parts.Length >= 2)
                    {
                        return parts[1];
                    }
                }
            }
            return null;
        }
    }
}