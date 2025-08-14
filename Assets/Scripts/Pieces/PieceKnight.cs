using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static EnumDefines;

public class PieceKnight : IPieces
{

    public override List<CoordXY> GetPossibleMoves()
    {
        var moves = new List<CoordXY>();

        int x = (int)GetCurrentPosition().x;
        int y = (int)GetCurrentPosition().y;

        int[,] knightMoves = new int[,]
        {
        { 2, 1 }, { 2, -1 }, { -2, 1 }, { -2, -1 },
        { 1, 2 }, { 1, -2 }, { -1, 2 }, { -1, -2 }
        };

        for (int i = 0; i < 8; i++)
        {
            int newX = x + knightMoves[i, 0];
            int newY = y + knightMoves[i, 1];

            var pos = new CoordXY((COORD_X)newX, (COORD_Y)newY);

            if (IsCoordValid(pos) && !IsAnyAllyPiecesAt(pos)) moves.Add(pos);
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
