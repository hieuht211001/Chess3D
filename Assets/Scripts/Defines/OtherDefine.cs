using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeneralDefine;

public class OtherDefine
{
    public static float PLATE_FALL_HEIGHT = 0.08f;
    public static float CAPTURE_QUEUE_OFFSET_FROM_BOARD = 3.3f;
    public static float CAPTURE_QUEUE_HEIGHT = -0.8f;
    public static float CAPTURE_QUEUE_OFFSET_BETWEEN_LINE = 1.5f;
    public static float PIECE_DROP_HEIGHT = 0.7f;
    public enum PLATE_TYPE
    {
        NONE,
        LEGAL,
        ILLEGAL,
        SPECIAL,
        CAPTURE,
    }

    public class CastlePosPair
    {
        public CoordXY originPos;
        public CoordXY castlePos;
        public CastlePosPair(CoordXY originPos, CoordXY castlePos)
        {
            this.originPos = originPos;
            this.castlePos = castlePos;
        }

        public CastlePosPair() { }
    }
}
