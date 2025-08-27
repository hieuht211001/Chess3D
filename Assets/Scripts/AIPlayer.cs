using UnityEngine;
using static GeneralDefine;

public class AIPlayer : IPlayerController
{
    private IAIChess chessAIEngine;
    public AIPlayer(TEAM_SIDE teamSide) : base(teamSide) 
    {
        chessAIEngine = new StockFishEngine();
        chessAIEngine.StartEngine();
    }

    public async override void OnTurnSwitch(TEAM_SIDE teamSide)
    {
        if (teamSide != this.teamSide) return;
        string fenData = FenUtils.GetFullFen(teamSide, player);
        string moveResult = await chessAIEngine.GetBestMove(fenData, 3000);
        CoordXY originPos = FenUtils.UciToCoordXY(moveResult).from;
        CoordXY destPos = FenUtils.UciToCoordXY(moveResult).to;
        IPieces selectedPiece = boardLogic.GetPieceAt(originPos);
        player[(int)teamSide].SetSelectPiece(selectedPiece);
        player[(int)teamSide].MoveSelectedPiece(destPos);
        Debug.Log(moveResult);
        turnManager.SwitchTurn();
    }
}
