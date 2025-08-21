using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;
using static GeneralDefine;
using static OtherDefine;
using static PiecesDefine;

public class ChessRules
{
    private PlayerManager[] playerManager;
    private BoardLogic boardLogic;
    public ChessRules(BoardLogic boardLogic, PlayerManager[] playerManagers)
    {
        this.playerManager = playerManagers;
        this.boardLogic = boardLogic;
    }

    #region CASTLE_FUNCTION
    public bool IsAbleToCastle(TEAM_SIDE teamSide)
    {
        IPieces pieceKing = GetKingAndRooksCanCastle(teamSide).pieceKing;
        List<IPieces> pieceRooks = GetKingAndRooksCanCastle(teamSide).pieceRooks;
        if (pieceKing == null || pieceRooks == null || pieceRooks.Count <= 0) return false;
        return true;
    }

    public ((IPieces pieceKing, CoordXY castleKingPos) kingData, (IPieces pieceRook, CoordXY castleRookPos) rookData) 
        GetMoveCastlePieceBySelectedPlate(TEAM_SIDE teamSide, CoordXY selectedPlateCoord)
    {
        try
        {
            var castlePosPairs = GetCastlePosPair(teamSide);
            var castleKingPairs = castlePosPairs.pieceKing;
            var castleRookPairs = castlePosPairs.pieceRook;

            if (castleKingPairs == null || castleRookPairs == null)
                return ((null, null), (null, null));

            var kingAndRooks = GetKingAndRooksCanCastle(teamSide);
            IPieces pieceKing = kingAndRooks.pieceKing;
            List<IPieces> pieceRooks = kingAndRooks.pieceRooks;

            for (int i = 0; i < pieceRooks.Count; i++)
            {
                CastlePosPair kingPair = castleKingPairs[i];
                CastlePosPair rookPair = castleRookPairs[i];

                if (selectedPlateCoord.IsEqual(kingPair.castlePos))
                {
                    return ((pieceKing, kingPair.castlePos), (pieceRooks[i], rookPair.castlePos));
                }
            }

            return ((null, null), (null, null));
        }
        catch (Exception ex)
        {
            return ((null, null), (null, null));
        }
    }


    public (List<CastlePosPair> pieceKing, List<CastlePosPair> pieceRook) GetCastlePosPair(TEAM_SIDE teamSide)
    {
        try
        {
            List<CastlePosPair> castlePosPairKing = new List<CastlePosPair>();
            List<CastlePosPair> castlePosPairRook = new List<CastlePosPair>();
            IPieces pieceKing = GetKingAndRooksCanCastle(teamSide).pieceKing;
            List<IPieces> pieceRooks = GetKingAndRooksCanCastle(teamSide).pieceRooks;
            if (pieceKing == null || pieceRooks == null || pieceRooks.Count <= 0) return (null, null);

            foreach (IPieces pieceRook in pieceRooks)
            {
                CoordXY castlePosKing = GetCastlePosition(pieceKing, pieceRook).newKingPos;
                CoordXY castlePosRook = GetCastlePosition(pieceKing, pieceRook).newRookPos;
                castlePosPairRook.Add(new CastlePosPair(pieceRook.GetCurrentPosition(), castlePosRook));
                castlePosPairKing.Add(new CastlePosPair(pieceKing.GetCurrentPosition(), castlePosKing));
            }
            return (castlePosPairKing, castlePosPairRook);
        }
        catch (Exception ex)
        {
            return (null, null);
        }
    }

    private (IPieces pieceKing, List<IPieces> pieceRooks) GetKingAndRooksCanCastle(TEAM_SIDE teamSide)
    {
        var pieces = playerManager[(int)teamSide].GetPieceList();
        var king = pieces.FirstOrDefault(p => p.pieceType == PIECE_TYPE.KING);
        var rooks = pieces.Where(p => p.pieceType == PIECE_TYPE.ROOK).ToList();
        List<IPieces> pieceRookList = new List<IPieces>(); 

        if (king == null || rooks.Count == 0) return (king, pieceRookList);

        if (king.hasMoved || IsKingInCheck(teamSide)) return (king, pieceRookList);

        foreach (var rook in rooks)
        {
            if (rook.hasMoved) continue;
            if (IsAnyPiecesBetween(king, rook)) continue;

            var kingPath = GetKingPathForCastle(king, rook);
            var legalPath = FilterIllegalMoves(king, kingPath).legalList;

            if (kingPath.Count == legalPath.Count) pieceRookList.Add(rook);
        }

        return (king, pieceRookList);
    }


    private (CoordXY newKingPos, CoordXY newRookPos) GetCastlePosition(IPieces king, IPieces rook)
    {
        var kingPos = king.GetCurrentPosition();
        var rookPos = rook.GetCurrentPosition();

        bool isKingSideCastle = rook.GetCurrentPosition().x > king.GetCurrentPosition().x;
        int kingMoveDir = isKingSideCastle ? 1 : -1;
        CoordXY newKingPos = new CoordXY(kingPos.x + 2 * kingMoveDir, kingPos.y);
        CoordXY newRookPos = new CoordXY(newKingPos.x - 1 * kingMoveDir, kingPos.y);

        return (newKingPos, newRookPos);
    }

    private List<CoordXY> GetKingPathForCastle(IPieces king, IPieces rook)
    {
        bool isKingSideCastle = rook.GetCurrentPosition().x > king.GetCurrentPosition().x;
        var kingPos = king.GetCurrentPosition();

        List<CoordXY> path = new List<CoordXY>();
        int kingMoveDir = isKingSideCastle ? 1 : -1;
        path.Add(new CoordXY(kingPos.x + 1 * kingMoveDir, kingPos.y));
        path.Add(new CoordXY(kingPos.x + 2 * kingMoveDir, kingPos.y));
        return path;
    }

    private bool IsAnyPiecesBetween(IPieces piece1, IPieces piece2)
    {
        CoordXY pos1 = piece1.GetCurrentPosition();
        CoordXY pos2 = piece2.GetCurrentPosition();

        if (pos1.x == pos2.x)
        {
            int minY = Mathf.Min((int)pos1.y, (int)pos2.y);
            int maxY = Mathf.Max((int)pos1.y, (int)pos2.y);

            for (int y = minY + 1; y < maxY; y++)
            {
                CoordXY coord = new CoordXY(pos1.x, (COORD_Y)y);
                if (piece1.IsAnyAllyPiecesAt(coord) || piece1.IsAnyEnemyPiecesAt(coord)) return true;
            }
        }
        else if (pos1.y == pos2.y)
        {
            int minX = Mathf.Min((int)pos1.x, (int)pos2.x);
            int maxX = Mathf.Max((int)pos1.x, (int)pos2.x);

            for (int x = minX + 1; x < maxX; x++)
            {
                CoordXY coord = new CoordXY((COORD_X)x, pos1.y);
                if (piece1.IsAnyAllyPiecesAt(coord) || piece1.IsAnyEnemyPiecesAt(coord)) return true;
            }
        }
        return false;
    }
    #endregion

    #region FILTER_CHECKMATE
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
                        if (System.Math.Abs(scanX - kingX) <= 1 && System.Math.Abs(scanY - kingY) <= 1 && piece.pieceType == PIECE_TYPE.KING)
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
    #endregion
}
