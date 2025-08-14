using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;
using static EnumDefines;

public class ChessRules
{
    private PlayerManager[] playerManager;
    private BoardLogic boardLogic;
    public ChessRules(BoardLogic boardLogic, PlayerManager[] playerManagers)
    {
        this.playerManager = playerManagers;
        this.boardLogic = boardLogic;
    }

    public (List<CoordXY> legalList, List<CoordXY> illegalList) FilterIllegalMoves(IPieces piece, List<CoordXY> posList)
    {
        TEAM_SIDE teamSide = piece.teamSide;
        List<CoordXY> illegal = new List<CoordXY>();
        for (int i = posList.Count - 1; i >= 0; i--)
        {
            SimulateCommand simulateMovement = new SimulateCommand(piece, posList[i]);
            simulateMovement.Execute();
            if (IsKingInCheck(teamSide))
            {
                illegal.Add(posList[i]);
                posList.Remove(posList[i]);
            }
            simulateMovement.Undo();
        }
        return (posList, illegal);
    }

    public bool IsKingInCheck(TEAM_SIDE teamSide)
    {
        IPieces pieceKing = playerManager[(int)teamSide].GetAssignedPieces(PIECE_TYPE.KING)[0];
        if (IsKingInCheckByLine(teamSide, (PieceKing)pieceKing) != null) return true;
        if (IsKingInCheckByKnight(teamSide, (PieceKing)pieceKing) != null) return true;
        if (IsKingInCheckByPawn(teamSide, (PieceKing)pieceKing) != null) return true;
        return false;
    }

    private IPieces IsKingInCheckByLine(TEAM_SIDE teamSide, PieceKing pieceKing)
    {
        (int dx, int dy)[] directions = new (int, int)[]
        {
            (1, 0), (-1, 0), (0, 1), (0, -1),
            (1, 1), (-1, 1), (1, -1), (-1, -1)
        };

        int kingX = (int)pieceKing.GetCurrentPosition().x;
        int kingY = (int)pieceKing.GetCurrentPosition().y;
        foreach (var (dx, dy) in directions)
        {
            int scanX = kingX + dx;
            int scanY = kingY + dy;

            while (scanX >= (int)COORD_X.A && scanX < (int)COORD_X.MAX &&
                   scanY >= (int)COORD_Y._1 && scanY < (int)COORD_Y.MAX)
            {
                var pos = new CoordXY((COORD_X)scanX, (COORD_Y)scanY);

                if (pieceKing.IsAnyAllyPiecesAt(pos)) break; // bị chắn bởi quân mình

                if (pieceKing.IsAnyEnemyPiecesAt(pos))
                {
                    var piece = boardLogic.GetPieceAt(pos);
                    if (piece != null)
                    {
                        if ((dx == 0 || dy == 0) && (piece.pieceType == PIECE_TYPE.ROOK || piece.pieceType == PIECE_TYPE.QUEEN))
                            return piece;
                        if ((dx != 0 && dy != 0) && (piece.pieceType == PIECE_TYPE.BISHOP || piece.pieceType == PIECE_TYPE.QUEEN))
                            return piece;
                        if (Math.Abs(scanX - kingX) <= 1 && Math.Abs(scanY - kingY) <= 1 && piece.pieceType == PIECE_TYPE.KING)
                            return piece;
                    }
                    break;
                }

                scanX += dx;
                scanY += dy;
            }
        }
        return null;
    }

    private IPieces IsKingInCheckByKnight(TEAM_SIDE teamSide, PieceKing pieceKing)
    {
        (int dx, int dy)[] knightMoves = new (int, int)[]
        {
            (2, 1), (1, 2), (-1, 2), (-2, 1),
            (-2, -1), (-1, -2), (1, -2), (2, -1)
        };

        foreach (var (dx, dy) in knightMoves)
        {
            int scanX = (int)pieceKing.GetCurrentPosition().x + dx;
            int scanY = (int)pieceKing.GetCurrentPosition().y + dy;
            if (scanX >= (int)COORD_X.A && scanX < (int)COORD_X.MAX &&
                scanY >= (int)COORD_Y._1 && scanY < (int)COORD_Y.MAX)
            {
                var pos = new CoordXY((COORD_X)scanX, (COORD_Y)scanY);
                var piece = boardLogic.GetPieceAt(pos);
                if (piece != null && pieceKing.IsAnyEnemyPiecesAt(pos) && piece.pieceType == PIECE_TYPE.KNIGHT) return piece;
            }
        }
        return null;
    }

    private IPieces IsKingInCheckByPawn(TEAM_SIDE teamSide, PieceKing pieceKing)
    {
        int pawnDir = 1;
        if (teamSide == TEAM_SIDE.ALLY) pawnDir *= 1;
        else pawnDir *= (-1);
        (int dx, int dy)[] pawnAttacks = new (int, int)[]
        {
            (1, pawnDir), (-1, pawnDir)
        };
        foreach (var (dx, dy) in pawnAttacks)
        {
            int x = (int)pieceKing.GetCurrentPosition().x + dx;
            int y = (int)pieceKing.GetCurrentPosition().y + dy;
            if (x >= (int)COORD_X.A && x < (int)COORD_X.MAX &&
                y >= (int)COORD_Y._1 && y < (int)COORD_Y.MAX)
            {
                var pos = new CoordXY((COORD_X)x, (COORD_Y)y);
                var piece = boardLogic.GetPieceAt(pos);
                if (piece != null && pieceKing.IsAnyEnemyPiecesAt(pos) && piece.pieceType == PIECE_TYPE.PAWN) return piece;
            }
        }
        return null;
    }

}
