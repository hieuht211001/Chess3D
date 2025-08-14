using System.Collections.Generic;
using UnityEngine;
using static EnumDefines;

public class PieceQueen : IPieces
{

    public override List<CoordXY> GetPossibleMoves()
    {
        CoordXY currentPos = GetCurrentPosition();
        var moves = new List<CoordXY>();
        int currentX = (int)currentPos.x;
        int currentY = (int)currentPos.y;

        (int dx, int dy)[] directions = new (int, int)[]
        {
            (1, 0),   
            (-1, 0), 
            (0, 1), 
            (0, -1), 
            (1, 1),   
            (-1, 1),
            (1, -1), 
            (-1, -1) 
        };

        foreach (var (dx, dy) in directions)
        {
            int x = currentX + dx;
            int y = currentY + dy;

            while (x >= (int)COORD_X.A && x < (int)COORD_X.MAX &&
                   y >= (int)COORD_Y._1 && y < (int)COORD_Y.MAX)
            {
                var pos = new CoordXY((COORD_X)x, (COORD_Y)y);

                if (IsAnyAllyPiecesAt(pos)) break;

                moves.Add(pos);

                if (IsAnyEnemyPiecesAt(pos)) break;

                x += dx;
                y += dy;
            }
        }

        return moves;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
