using System.Collections.Generic;
using UnityEngine;
using static EnumDefines;

public class PieceRook : IPieces
{

    public override List<CoordXY> GetPossibleMoves()
    {
        List<CoordXY> possibleMovesList = new List<CoordXY>();
        possibleMovesList.AddRange(GetMovesInLine((int)COORD_X.A, (int)COORD_X.MAX, GetCurrentPosition(), true));
        possibleMovesList.AddRange(GetMovesInLine((int)COORD_Y._1, (int)COORD_Y.MAX, GetCurrentPosition(), false));
        return possibleMovesList;
    }

    private List<CoordXY> GetMovesInLine(int start, int end, CoordXY currentPos, bool isXAxis)
    {
        var moves = new List<CoordXY>();
        int current = isXAxis ? (int)currentPos.x : (int)currentPos.y;

        foreach (int step in new int[] { 1, -1 })
        {
            for (int i = current + step; ; i += step)
            {
                if (step > 0 && i >= end) break;
                if (step < 0 && i < start) break;

                var pos = isXAxis
                    ? new CoordXY((COORD_X)i, currentPos.y)
                    : new CoordXY(currentPos.x, (COORD_Y)i);

                if (!IsCoordValid(pos)) continue;

                if (IsAnyAllyPiecesAt(pos)) break;

                moves.Add(pos);

                if (IsAnyEnemyPiecesAt(pos)) break;
            }
        }

        return moves;
    }



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
