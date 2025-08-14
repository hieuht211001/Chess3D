using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnumDefines;

public abstract class IPieces : MonoBehaviour
{
    public TEAM_SIDE teamSide;
    public PIECE_TYPE pieceType;
    private CoordXY currentPos;
    protected BoardLogic boardGrid;
    private Collider collider;
    private Rigidbody rb;
    [SerializeField] public PiecesUI pieceObjectRef;

    public void AssignRefIntance(BoardLogic boardLogic)
    {
        this.boardGrid = boardLogic;
    }

    public void Init(PiecesUI piece, TEAM_SIDE teamSide, PIECE_TYPE pieceType, CoordXY position)
    {
        this.pieceObjectRef = piece;
        this.teamSide = teamSide;
        this.pieceType = pieceType;
        this.currentPos = position;
        Vector2 pos = Util.ConvertCoordToWorldVector(position);
        this.transform.position = new Vector3(pos.x, 0.7f, pos.y);
        this.tag = TAG.PIECES.ToString();
        this.gameObject.layer = LayerMask.NameToLayer(LAYER.PIECES.ToString());
        Renderer rend = GetComponent<Renderer>();
        if (teamSide == TEAM_SIDE.ALLY)
            rend.material = pieceObjectRef.GetPieceMaterial(PIECE_MATERIAL.WHITE);
        else
            rend.material = pieceObjectRef.GetPieceMaterial(PIECE_MATERIAL.BLACK);
        SetUpColliderNRigidBody();

    }

    private void SetUpColliderNRigidBody()
    {
        gameObject.AddComponent<BoxCollider>();
        gameObject.AddComponent<Rigidbody>();
        collider = gameObject.GetComponent<BoxCollider>();
        rb = gameObject.GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public CoordXY GetCurrentPosition() => this.currentPos;

    public void MoveToCaptureQueue()
    {

    }


    public abstract List<CoordXY> GetPossibleMoves();
    public bool IsCoordInPossiblePosList(CoordXY pos)
    {
        List<CoordXY > possibleMoves = GetPossibleMoves();
        for (int i = 0; i < possibleMoves.Count; i++)
        {
            if (pos.x == possibleMoves[i].x && pos.y == possibleMoves[i].y) return true;
        }
        return false;
    }
    public bool IsCoordValidToMove(CoordXY pos)
    {
        return (IsCoordValid(pos) && !boardGrid.IsAnyAllyPiecesAt(pos, teamSide) && !boardGrid.IsAnyEnermyPiecesAt(pos, teamSide));
    }

    public bool IsCoordValidToOccupy(CoordXY pos)
    {
        return (IsCoordValid(pos) && !boardGrid.IsAnyAllyPiecesAt(pos, teamSide) && boardGrid.IsAnyEnermyPiecesAt(pos, teamSide));
    }

    public bool IsAnyAllyPiecesAt(CoordXY pos)
    {
        return (IsCoordValid(pos) && boardGrid.IsAnyAllyPiecesAt(pos, teamSide));
    }

    public bool IsCoordValid(CoordXY pos)
    {
        if (pos == null) return false;
        if (pos.x < COORD_X.A || pos.x >= COORD_X.MAX) return false;
        if (pos.y < COORD_Y._1 || pos.y >= COORD_Y.MAX) return false;
        return true;
    }

    public bool IsAnyEnemyPiecesAt(CoordXY pos)
    {
        return (IsCoordValid(pos) && boardGrid.IsAnyEnermyPiecesAt(pos, teamSide));
    }

    public float liftHeight = 0.3f;    
    public float liftDuration = 0.1f; 
    public float moveDuration = 0.2f; 

    public void MoveToWithLift(CoordXY pos)
    {
        this.currentPos = pos;
        Vector2 vectorPos = Util.ConvertCoordToWorldVector(pos);
        StartCoroutine(MoveRoutine(new Vector3(vectorPos.x, 0 ,vectorPos.y)));
    }

    private IEnumerator MoveRoutine(Vector3 targetPos)
    {
        collider.isTrigger = true;
        rb.isKinematic = true;
        Vector3 startPos = transform.position;
        Vector3 liftedPos = new Vector3(startPos.x, startPos.y + liftHeight, startPos.z);

        // Fly Up Animation
        float timer = 0f;
        while (timer < liftDuration)
        {
            transform.position = Vector3.Lerp(startPos, liftedPos, timer / liftDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = liftedPos;

        // Fly to target pos
        Vector3 targetLiftedPos = new Vector3(targetPos.x, liftedPos.y, targetPos.z);
        timer = 0f;
        while (timer < moveDuration)
        {
            transform.position = Vector3.Lerp(liftedPos, targetLiftedPos, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = targetLiftedPos;

        // A
        timer = 0f;
        while (timer < liftDuration)
        {
            transform.position = Vector3.Lerp(targetLiftedPos, targetPos, timer / liftDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        rb.isKinematic = false;
        collider.isTrigger = false;
    }

    public void ForceSetPiecePos(CoordXY pos) => this.currentPos = pos;
}
