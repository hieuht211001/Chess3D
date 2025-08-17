using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PiecesDefine
{
    public static float PIECE_LIFT_HEIGHT = 0.3f;
    public static float PIECE_LIFT_DURATION = 0.1f;
    public static float PIECE_MOVE_DURATION = 0.2f;
    public enum PIECE_TYPE
    {
        PAWN,
        BISHOP,
        KNIGHT,
        ROOK,
        QUEEN,
        KING,
    }

    public enum PIECE_MATERIAL
    {
        BLACK,
        WHITE,
        GOLD,
    }
}
