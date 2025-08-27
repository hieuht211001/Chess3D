using System;
using UnityEngine;
using UnityEngine.UIElements;
using static GeneralDefine;
using static PiecesDefine;

public class Util
{
    public static CoordXY ConvertWorldVectorToCoord(Vector2 pos)
    {
        int iIndexX = (int)(((int)COORD_X.MAX - 1) - (pos.x / BoardLogic.SCALE_X));
        int iIndexY = (int)(((int)COORD_Y.MAX - 1) - (pos.y / BoardLogic.SCALE_Y));
        return new CoordXY((COORD_X)iIndexX, (COORD_Y)iIndexY);
    }

    public static Vector2 ConvertCoordToWorldVector(CoordXY coord)
    {
        float posX = ((int)COORD_X.MAX - 1 - (int)coord.x) * BoardLogic.SCALE_X;
        float posY = ((int)COORD_Y.MAX - 1 - (int)coord.y) * BoardLogic.SCALE_Y;
        return new Vector2(posX, posY);
    }

    public static CoordXY GetNextCoordByIndex(CoordXY originCoord, int indexX, int indexY)
    {
        int originX = (int)originCoord.x;
        int originY = (int)originCoord.y;

        if ((originX + indexX) < (int)COORD_X.A && (originX + indexX) >= (int)COORD_X.MAX)
        {
            return null;
        }
        if ((originY + indexY) < (int)COORD_Y._1 && (originY + indexY) >= (int)COORD_Y.MAX)
        {
            return null;
        }
        return new CoordXY((COORD_X)(originX + indexX), (COORD_Y)(originY + indexY));
    }

    public static int GetMoveDirection(TEAM_SIDE side)
    {
        if (side == TEAM_SIDE.ALLY) return 1;
        else return -1;
    }

    public static Component GetComponent(GameObject obj)
    {
        if (obj == null) return null;

        var piece = obj.GetComponent<IPieces>();
        if (piece != null) return piece;

        var plate = obj.GetComponent<Plate>();
        if (plate != null) return plate;

        return null;
    }
}

public static class FenUtils
{
    public static char CoordXToFileChar(COORD_X x) => (char)('a' + (int)x);
    public static char CoordYToRankChar(COORD_Y y) => (char)('1' + (int)y);
    public static COORD_X FileCharToCoordX(char file) => (COORD_X)(file - 'a');
    public static COORD_Y RankCharToCoordY(char rank) => (COORD_Y)(rank - '1');


    public static (CoordXY from, CoordXY to) UciToCoordXY(string uci)
    {
        if (uci.Length < 4) throw new ArgumentException("Invalid UCI move");
        var from = new CoordXY(FileCharToCoordX(uci[0]), RankCharToCoordY(uci[1]));
        var to = new CoordXY(FileCharToCoordX(uci[2]), RankCharToCoordY(uci[3]));
        return (from, to);
    }

    public static string CoordXYToUci(CoordXY from, CoordXY to)
    {
        return $"{CoordXToFileChar(from.x)}{CoordYToRankChar(from.y)}" +
               $"{CoordXToFileChar(to.x)}{CoordYToRankChar(to.y)}";
    }

    public static char PieceToFenChar(PIECE_TYPE type, TEAM_SIDE side)
    {
        char c = type switch
        {
            PIECE_TYPE.PAWN => 'p',
            PIECE_TYPE.KNIGHT => 'n',
            PIECE_TYPE.BISHOP => 'b',
            PIECE_TYPE.ROOK => 'r',
            PIECE_TYPE.QUEEN => 'q',
            PIECE_TYPE.KING => 'k',
            _ => throw new Exception("Unknown piece type")
        };
        return side == TEAM_SIDE.ALLY ? char.ToUpper(c) : c;
    }

    public static string BoardToFen(TEAM_SIDE teamSide, Player[] players)
    {
        char[,] board = new char[8, 8];
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
                board[y, x] = ' ';

        foreach (var side in new TEAM_SIDE[] { TEAM_SIDE.ALLY, TEAM_SIDE.ENEMY })
        {
            var pm = players[(int)side];
            foreach (var piece in pm.GetPieceList())
            {
                if (!piece.isActive) continue;
                var pos = piece.GetCurrentPosition();
                board[7 - (int)pos.y, (int)pos.x] = PieceToFenChar(piece.pieceType, piece.teamSide);
            }
        }

        string fen = "";
        for (int y = 0; y < 8; y++)
        {
            int empty = 0;
            for (int x = 0; x < 8; x++)
            {
                char c = board[y, x];
                if (c == ' ')
                    empty++;
                else
                {
                    if (empty > 0)
                    {
                        fen += empty.ToString();
                        empty = 0;
                    }
                    fen += c;
                }
            }
            if (empty > 0) fen += empty.ToString();
            if (y < 7) fen += '/';
        }
        return fen;
    }

    public static string GetFullFen(TEAM_SIDE teamSide, Player[] players,
        string castling = "KQkq", string enPassant = "-", int halfmove = 0, int fullmove = 1)
    {
        string boardFen = BoardToFen(teamSide, players);
        string turn = (teamSide == TEAM_SIDE.ALLY) ? "w" : "b";
        return $"{boardFen} {turn} {castling} {enPassant} {halfmove} {fullmove}";
    }
}


