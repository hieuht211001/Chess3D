using System.Collections.Generic;
using UnityEngine;
using static GeneralDefine;
using static OtherDefine;
using static PiecesDefine;

public class HumanController : IPlayerController
{
    public HumanController(TEAM_SIDE teamSide) : base(teamSide) { }

    private void OnSameTurnPieceSelected(IPieces piece)
    {
        if (piece == null) return;
        List<CoordXY> possibleMoves = piece.GetPossibleMoves();
        List<CoordXY> legalMoves = new List<CoordXY>();
        List<CoordXY> captureMoves = new List<CoordXY>();
        List<CoordXY> illegalMoves = new List<CoordXY>();
        List<CastlePosPair> castleMoves = new List<CastlePosPair>();

        player[(int)piece.teamSide].SetSelectPiece(piece);
        player[(int)(piece.teamSide == TEAM_SIDE.ALLY ? TEAM_SIDE.ENEMY : TEAM_SIDE.ALLY)].DeSelectAllPiece();

        (List<CoordXY> legalList, List<CoordXY> illegalList) temp = chessRules.FilterIllegalMoves(piece, possibleMoves);
        foreach (var move in temp.legalList)
        {
            if (boardLogic.IsAnyEnermyPiecesAt(move, piece.teamSide)) captureMoves.Add(move);
            else legalMoves.Add(move);
        }
        illegalMoves = temp.illegalList;

        if (piece.pieceType == PIECE_TYPE.KING)
        {
            if (chessRules.IsAbleToCastle(piece.teamSide))
            {
                castleMoves.AddRange(chessRules.GetCastlePosPair(piece.teamSide).pieceKing);
                legalMoves.Remove(piece.GetCurrentPosition());
            }
        }

        plate.DestroyAllPlates();
        if (legalMoves != null && legalMoves.Count > 0)
        {
            plate.ShowPlateAt(PLATE_TYPE.LEGAL, legalMoves, piece.GetCurrentPosition());
        }
        if (captureMoves != null && captureMoves.Count > 0)
        {
            plate.ShowPlateAt(PLATE_TYPE.CAPTURE, captureMoves, piece.GetCurrentPosition());
        }
        if (illegalMoves != null && illegalMoves.Count > 0)
        {
            plate.ShowPlateAt(PLATE_TYPE.ILLEGAL, illegalMoves, piece.GetCurrentPosition());
        }
        if (castleMoves != null && castleMoves.Count > 0)
        {
            List<CoordXY> castleCoord = new List<CoordXY>();

            foreach (CastlePosPair pair in castleMoves)
            {
                castleCoord.Add(pair.castlePos);
            }
            plate.ShowPlateAt(PLATE_TYPE.SPECIAL, castleCoord, piece.GetCurrentPosition());
        }
    }

    private void OnDiffTurnPieceSelected(IPieces piece)
    {
        Plate selectedCapturePlate = piece.GetTriggeredPlate();
        if (selectedCapturePlate != null
            && selectedCapturePlate.GetPlateType() == PLATE_TYPE.CAPTURE
            && selectedCapturePlate.GetPos().IsEqual(piece.GetCurrentPosition()))
        {
            OnMovePlateSelected(selectedCapturePlate);
        }
    }

    private void OnMovePlateSelected(Plate movePlate)
    {
        if (movePlate == null || movePlate.GetPlateType() == PLATE_TYPE.ILLEGAL) return;
        if (movePlate.GetPlateType() == PLATE_TYPE.LEGAL || movePlate.GetPlateType() == PLATE_TYPE.CAPTURE)
        {
            CoordXY selectedPos = movePlate.GetPos();
            player[(int)turnManager.GetCurrentTurn()].MoveSelectedPiece(selectedPos);
        }
        else if (movePlate.GetPlateType() == PLATE_TYPE.SPECIAL)
        {
            CoordXY selectedPos = movePlate.GetPos();
            var castleData = chessRules.GetMoveCastlePieceBySelectedPlate(TEAM_SIDE.ALLY, selectedPos);
            player[(int)turnManager.GetCurrentTurn()].MoveCastle
                (castleData.kingData.pieceKing, castleData.rookData.pieceRook,
                castleData.kingData.castleKingPos, castleData.rookData.castleRookPos);
        }
        movePlate.DestroyAllPlates();
        turnManager.SwitchTurn();
    }

    public override void OnClickEvent(Component component)
    {
        if (component is IPieces)
        {
            if (turnManager.IsTurnOf(((IPieces)component).teamSide))
                OnSameTurnPieceSelected((IPieces)component);
            else OnDiffTurnPieceSelected((IPieces)component);
        }
        if (component is Plate) OnMovePlateSelected((Plate)component);
    }

    DragPieceCommand dragPieceCmd;
    public override void OnHoldStartEvent(Component component)
    {
        if (component is IPieces && turnManager.IsTurnOf(((IPieces)component).teamSide))
        {
            ((IPieces)component).ForceSetPieceHeight(PIECE_LIFT_HEIGHT);
            OnSameTurnPieceSelected((IPieces)component);
            dragPieceCmd = new DragPieceCommand((IPieces)component);
        }
    }

    public override void OnHoldDragEvent(Component component, Vector2 position)
    {
        if (component is IPieces && turnManager.IsTurnOf(((IPieces)component).teamSide))
        {
            dragPieceCmd.Execute(position);
        }
    }

    public override void OnHoldEndEvent(Component component, Vector2 position)
    {
        if (component is IPieces && turnManager.IsTurnOf(((IPieces)component).teamSide))
        {
            Plate selectedPlate = ((IPieces)component).GetTriggeredPlate();
            if (selectedPlate != null
                && selectedPlate.GetPlateType() != PLATE_TYPE.ILLEGAL
                && !selectedPlate.GetPos().IsEqual(((IPieces)component).GetCurrentPosition()))
                OnMovePlateSelected(selectedPlate);
            else dragPieceCmd.Undo();
        }
    }
}
