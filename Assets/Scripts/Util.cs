using UnityEngine;
using UnityEngine.UIElements;
using static EnumDefines;

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
}
