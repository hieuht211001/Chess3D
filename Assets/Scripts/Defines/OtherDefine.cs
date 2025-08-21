using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeneralDefine;

public class OtherDefine
{
    public static float PLATE_FALL_HEIGHT = 0.08f;
    public enum PLATE_TYPE
    {
        NONE,
        LEGAL,
        ILLEGAL,
        SPECIAL,
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
