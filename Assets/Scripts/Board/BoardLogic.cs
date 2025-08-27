using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;
using static GeneralDefine;

public class BoardLogic : MonoBehaviour
{
    public static float SCALE_X = 0F;
    public static float SCALE_Y = 0F;
    private Player[] player;
    void Awake()
    {
        CalculateCoordRate();
    }

    public void AssignRefInstance(Player[] player)
    {
        this.player = player;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 CalculateCoordRate()
    {
        BoxCollider box = gameObject.GetComponent<BoxCollider>();
        if (box == null) throw new System.Exception();
        Vector3 size = box.size;  // size local space
        Vector3 scale = gameObject.transform.lossyScale;
        Vector3 worldSize = Vector3.Scale(size, scale);
        SCALE_X = worldSize.x / (int)COORD_X.MAX;
        SCALE_Y = worldSize.z / (int)COORD_Y.MAX;
        return new Vector2(SCALE_X, SCALE_Y);
    }

    public IPieces GetPieceAt(CoordXY coord)
    {
        List<IPieces> pieces = new List<IPieces>();
        pieces.AddRange(player[(int)TEAM_SIDE.ALLY].GetPieceList());
        pieces.AddRange(player[(int)TEAM_SIDE.ENEMY].GetPieceList());
        foreach (var piece in pieces)
        {
            if (piece.GetCurrentPosition().x == coord.x
                && piece.GetCurrentPosition().y == coord.y && piece.isActive) return piece;
        }
        return null;
    }

    public bool IsAnyAllyPiecesAt(CoordXY pos, TEAM_SIDE teamSide)
    {
        if (pos == null) return false;
        if (!IsAnyPiecesAt(pos)) return false;
        return GetPieceAt(pos).teamSide == teamSide;
    }

    public bool IsAnyEnermyPiecesAt(CoordXY pos, TEAM_SIDE teamSide)
    {
        if (pos == null) return false;
        if (!IsAnyPiecesAt(pos)) return false;
        return GetPieceAt(pos).teamSide != teamSide;
    }

    public bool IsAnyPiecesAt(CoordXY coord)
    {
        List<IPieces> pieces = new List<IPieces>();
        pieces.AddRange(player[(int)TEAM_SIDE.ALLY].GetPieceList());
        pieces.AddRange(player[(int)TEAM_SIDE.ENEMY].GetPieceList());
        foreach(var piece in pieces)
        {
            if (piece.GetCurrentPosition().x == coord.x
                && piece.GetCurrentPosition().y == coord.y && piece.isActive) return true;
        }
        return false;
    }
}
