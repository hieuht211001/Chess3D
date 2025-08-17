using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;
using static GeneralDefine;
using static OtherDefine;

public class Plate : MonoBehaviour
{
    public string movePlateTag = TAG.MOVE_PLATE.ToString();
    public string movePlateLayer = LAYER.MOVE_PLATE.ToString();
    private PLATE_TYPE plateType;
    private CoordXY position;
    private PlateUI plateUi;

    void Start()
    {
        Collider myCol = GetComponent<Collider>();
        GameObject[] pieces = GameObject.FindGameObjectsWithTag(TAG.PIECES.ToString());
        foreach (GameObject piece in pieces)
        {
            Collider pieceCollider = piece.GetComponent<Collider>();
            if (pieceCollider != null)
            {
                Physics.IgnoreCollision(myCol, pieceCollider, true);
            }
        }
    }

    public Plate()
    {
        position = new CoordXY();
    }

    public Plate(PlateUI plateUI)
    {
        this.plateUi = plateUI;
        position = new CoordXY();
    }
    public CoordXY GetPos() => position;
    public PLATE_TYPE GetPlateType() => plateType;
    public Plate GetPlateInAssignedPos(CoordXY pos)
    {
        GameObject[] plates = GameObject.FindGameObjectsWithTag(movePlateTag);
        foreach (GameObject plate in plates)
        {
            Plate plateObj = plate.GetComponent<Plate>();
            if (plateObj != null && plateObj.position.IsEqual(pos)) return plateObj;
        }
        return null;
    }

    public void DestroyAllPlates()
    {
        GameObject[] plates = GameObject.FindGameObjectsWithTag(movePlateTag);
        foreach (GameObject plate in plates)
        {
            Destroy(plate);
        }
    }

    public void ShowPlateAt(PLATE_TYPE type, List<CoordXY> coords, CoordXY originPos)
    {
        GameObject movePlatePrefab = plateUi.GetPlateObject(type);
        
        List<(CoordXY, float)> returnList = GetFallHeightList(coords, originPos, type == PLATE_TYPE.LEGAL);

        foreach ((CoordXY, float) coord in returnList)
        {
            Vector2 coord2D = Util.ConvertCoordToWorldVector(coord.Item1);
            GameObject gameObject = Instantiate(movePlatePrefab,
                new Vector3(coord2D.x, coord.Item2, coord2D.y), Quaternion.identity);
            Plate mp = gameObject.AddComponent<Plate>();
            mp.position.x = coord.Item1.x;
            mp.position.y = coord.Item1.y;
            mp.plateType = type;
            gameObject.AddComponent<BoxCollider>();
            gameObject.AddComponent<Rigidbody>();
            gameObject.layer = LayerMask.NameToLayer(movePlateLayer);
            gameObject.tag = movePlateTag;

            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            MouseAction mouseAction = gameObject.AddComponent<MouseAction>();
            GameManager pieces = FindAnyObjectByType<GameManager>();
            mouseAction.OnClick += pieces.OnClickEvent;
            mouseAction.OnHoldStart += pieces.OnHoldStartEvent;
            mouseAction.OnHoldDrag += pieces.OnHoldDragEvent;
            mouseAction.OnHoldEnd += pieces.OnHoldEndEvent;

            gameObject.SetActive(true);
        }
    }

    public List<(CoordXY, float)> GetFallHeightList(List<CoordXY> originalList, CoordXY originPos, bool returnOriginPos)
    {
        List<(CoordXY, float)> returnList = new List<(CoordXY, float)>();

        List<CoordXY> xGroups = originalList
            .GroupBy(item => item.x)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .ToList();

        foreach (CoordXY xGroup in xGroups)
        {
            int iRate = Math.Abs((int)xGroup.y - (int)originPos.y);
            float fallHeight = iRate * PLATE_FALL_HEIGHT;
            returnList.Add((xGroup, fallHeight));
        }

        List<CoordXY> restAfterX = originalList.Except(xGroups).ToList();
        List<CoordXY> yGroups = restAfterX
            .GroupBy(item => item.y)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .ToList();

        foreach (CoordXY yGroup in yGroups)
        {
            int iRate = Math.Abs((int)yGroup.y - (int)originPos.y);
            float fallHeight = iRate * PLATE_FALL_HEIGHT;
            returnList.Add((yGroup, fallHeight));
        }

        List<CoordXY> rest = originalList.Except(xGroups).Except(yGroups).ToList();
        foreach (CoordXY other in rest)
        {
            returnList.Add((other, PLATE_FALL_HEIGHT * 2));
        }

        if (returnOriginPos) returnList.Add((originPos, PLATE_FALL_HEIGHT));

        return returnList;
    }
}
