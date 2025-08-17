using UnityEngine;

public class GeneralDefine
{
    public class CoordXY
    {
        public COORD_X x;
        public COORD_Y y;
        public CoordXY(COORD_X x, COORD_Y y)
        {
            this.x = x;
            this.y = y;
        }
        public CoordXY() { }

        public bool IsEqual(object obj)
        {
            if (obj is CoordXY other)
            {
                return this.x == other.x && this.y == other.y;
            }
            return false;
        }
    }

    public enum TEAM_SIDE
    {
        ALLY,
        ENEMY,
        MAX,
    }

    public enum START_PIECE_LAYOUT
    {
        PAWN_1, PAWN_2, PAWN_3, PAWN_4, PAWN_5, PAWN_6, PAWN_7, PAWN_8,
        ROOK_1, KNIGHT_1, BISHOP_1,QUEEN, KING, BISHOP_2, KNIGHT_2, ROOK_2,
        MAX
    }

    public float REAL_SIZE_X = 1.25f;
    public float REAL_SIZE_Y = 1.25f;
    
    public enum COORD_X
    {
        A, B, C, D, E, F, G, H = 7, MAX
    }

    public enum COORD_Y
    {
        _1,_2, _3, _4, _5, _6, _7, _8, MAX
    }

    public enum TAG
    {
        BOARD,
        PIECES,
        MOVE_PLATE
    }

    public enum LAYER
    {
        PIECES,
        MOVE_PLATE
    }
}
