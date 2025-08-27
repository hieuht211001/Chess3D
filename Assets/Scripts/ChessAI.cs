using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IAIChess
{
    protected Process _proc;
    public virtual void InitProc() { }
    public void StartEngine()
    {
        InitProc();
        _proc.Start();

        Send("uci");
        Send("isready");
        ReadUntil("readyok");
    }

    public async Task<string> GetBestMove(string fen, int movetimeMs = 1000)
    {
        Send($"position fen {fen}");
        Send($"go movetime {movetimeMs}");

        string bestMove = null;

        await Task.Run(() =>
        {
            string line;
            while ((line = _proc.StandardOutput.ReadLine()) != null)
            {
                if (line.StartsWith("bestmove"))
                {
                    bestMove = line.Split(' ')[1];
                    break;
                }
            }
        });

        return bestMove;
    }

    private void Send(string cmd)
    {
        _proc.StandardInput.WriteLine(cmd);
        _proc.StandardInput.Flush();
    }

    private void ReadUntil(string token)
    {
        string line;
        while ((line = _proc.StandardOutput.ReadLine()) != null)
        {
            if (line.Contains(token)) break;
        }
    }

    public void Stop()
    {
        try
        {
            Send("quit");
            _proc?.Kill();
        }
        catch { }
    }
}

public class StockFishEngine : IAIChess
{
    public override void InitProc()
    {
        _proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Application.streamingAssetsPath + "/stockfish-windows-x86-64-avx2.exe",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
    }
}
