using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static GeneralDefine;
using static OtherDefine;

public class CaptureQueue : MonoBehaviour
{
    private GameObject captureLineUI;
    private List<GameObject>[] captureLineList;
    private CoordXY coord;

    public void AssignRefInstance(GameObject refInstance)
    {
        captureLineUI = refInstance;
        captureLineList = new List<GameObject>[(int)TEAM_SIDE.MAX];
        captureLineList[(int)TEAM_SIDE.ALLY] = new List<GameObject>();
        captureLineList[(int)TEAM_SIDE.ENEMY] = new List<GameObject>();
    }

    public Vector2 AddNewCaptureLine(TEAM_SIDE teamSide)
    {
        (Vector2 newPos, CoordXY coord) =  GetRemainPosInLine(teamSide);
        GameObject gameObject = Instantiate(captureLineUI,
                new Vector3(newPos.x, CAPTURE_QUEUE_HEIGHT, newPos.y), Quaternion.identity);
        gameObject.AddComponent<BoxCollider>();
        CaptureQueue data = gameObject.AddComponent<CaptureQueue>();
        data.coord = coord;
        gameObject.SetActive(true);
        captureLineList[(int)teamSide].Add(gameObject);
        return newPos;
    }

    public CaptureQueue() { }

    private (Vector2 pos, CoordXY coord) GetRemainPosInLine(TEAM_SIDE teamSide)
    {
        var list = captureLineList[(int)teamSide];
        float offsetFromBoard = (teamSide == TEAM_SIDE.ENEMY ? -1 : 1) * CAPTURE_QUEUE_OFFSET_FROM_BOARD;
        float offsetBetweenLine = (teamSide == TEAM_SIDE.ENEMY ? -1 : 1) * CAPTURE_QUEUE_OFFSET_BETWEEN_LINE;
        CoordXY startCoord = (teamSide == TEAM_SIDE.ENEMY)
            ? new CoordXY(COORD_X.H, COORD_Y._8)
            : new CoordXY(COORD_X.A, COORD_Y._1);

        if (list.Count == 0)
        {
            Vector2 start = Util.ConvertCoordToWorldVector(startCoord);
            return (new Vector2(start.x + offsetFromBoard, start.y), startCoord);
        }

        CoordXY lastCoord = list[^1].GetComponent<CaptureQueue>().coord;

        bool isLastCoordOfLine = (teamSide == TEAM_SIDE.ALLY && lastCoord.y >= COORD_Y._8)
                        || (teamSide == TEAM_SIDE.ENEMY && lastCoord.y <= COORD_Y._1);

        int iLineNo = list.Count / (int)COORD_Y.MAX;

        CoordXY nextCoord;

        if (isLastCoordOfLine)
        {
            nextCoord = new CoordXY(lastCoord.x,
                                    (teamSide == TEAM_SIDE.ENEMY ? COORD_Y._8 : COORD_Y._1));
        }
        else
        {
            int dy = (teamSide == TEAM_SIDE.ENEMY ? -1 : 1);
            nextCoord = new CoordXY(lastCoord.x, (COORD_Y)((int)lastCoord.y + dy));
        }

        Vector2 pos = Util.ConvertCoordToWorldVector(nextCoord);
        return (new Vector2(pos.x + offsetFromBoard + offsetBetweenLine * iLineNo, pos.y), nextCoord);
    }

}
