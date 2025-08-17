using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static GeneralDefine;

public class PiecePawn : IPieces
{
    public override List<CoordXY> GetPossibleMoves()
    {
        var possibleMoves = new List<CoordXY>();
        int direction = Util.GetMoveDirection(teamSide);

        var steps = new List<(int dx, int dy, bool isCaptureMove)>
        {
            (0, 1 * direction, false),
            (0, 2 * direction, false), 
            (1, 1 * direction, true),
            (-1, 1 * direction, true),
        };

        var posStep1 = Util.GetNextCoordByIndex(GetCurrentPosition(), steps[0].dx, steps[0].dy);
        bool isStep1Blocked = !IsCoordValidToMove(posStep1);

        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            var pos = Util.GetNextCoordByIndex(GetCurrentPosition(), step.dx, step.dy);

            if (i == 1 && !IsPawnAtStartPos()) continue;

            if (!IsCoordValid(pos)) continue; 

            if (step.isCaptureMove)
            {
                if (IsCoordValidToOccupy(pos)) possibleMoves.Add(pos);
            }
            else
            {
                if (i == 0)
                {
                    if (IsCoordValidToMove(pos)) possibleMoves.Add(pos);
                }
                else if (i == 1)
                {
                    if (IsPawnAtStartPos() && !isStep1Blocked && IsCoordValidToMove(pos)) possibleMoves.Add(pos);
                }
            }
        }

        return possibleMoves;
    }

    private bool IsPawnAtStartPos()
    {
        if (this.teamSide == TEAM_SIDE.ALLY)
        {
            return GetCurrentPosition().y == COORD_Y._2;
        }
        else return GetCurrentPosition().y == COORD_Y._7;
    }
}
