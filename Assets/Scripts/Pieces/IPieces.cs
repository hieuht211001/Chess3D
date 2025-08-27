using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static GeneralDefine;
using static PiecesDefine;
using static UnityEditor.PlayerSettings;

public abstract class IPieces : MonoBehaviour
{
    #region VARIABLES
    public TEAM_SIDE teamSide;
    public PIECE_TYPE pieceType;
    private CoordXY currentPos;
    protected BoardLogic boardGrid;
    private MeshCollider meshCollider;
    private BoxCollider triggerCollider;
    private Rigidbody rb;
    private MouseAction mouseAction;
    private Plate triggeredPlate;
    private Vector3 finalPos;
    public bool isSelected { get; private set; }
    public bool isActive { get; private set; }
    public bool hasMoved { get; private set; }
    [SerializeField] public PiecesUI pieceObjectRef;
    #endregion

    #region INITIALIZE
    public void Init(PiecesUI piece, TEAM_SIDE teamSide, PIECE_TYPE pieceType, CoordXY position)
    {
        this.pieceObjectRef = piece;
        this.teamSide = teamSide;
        this.pieceType = pieceType;
        this.currentPos = position;
        SetPieceDropFrom(this.currentPos);
        this.tag = TAG.PIECES.ToString();
        this.gameObject.layer = LayerMask.NameToLayer(LAYER.PIECES.ToString());
        SetUpColliderNRigidBody();
        SetMaterial(1f);
        DeSelectPiece();
        ActivePiece();
    }

    private void SetUpColliderNRigidBody()
    {
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        gameObject.AddComponent<Rigidbody>();

        rb = gameObject.GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);

        triggerCollider = gameObject.AddComponent<BoxCollider>();
        triggerCollider.center = new Vector3(0f, - PIECE_LIFT_HEIGHT * 2, 0f);
        triggerCollider.size = new Vector3(0.4f, 1.34f, 0.4f);
        triggerCollider.isTrigger = true;

        mouseAction = gameObject.AddComponent<MouseAction>();;
    }

    public void AssignRefIntance(BoardLogic boardLogic)
    {
        this.boardGrid = boardLogic;
    }
    #endregion

    #region UNITY_FUNCTION

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TAG.MOVE_PLATE.ToString())) this.triggeredPlate = other.gameObject.GetComponent<Plate>();
        else if (other.gameObject.CompareTag(TAG.PIECES.ToString()) && other.transform.root != transform.root)
        {
            IPieces otherPiece = other.gameObject.GetComponent<IPieces>();
            if (!otherPiece.isSelected) otherPiece.SetMaterial(0.5f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(TAG.MOVE_PLATE.ToString())) this.triggeredPlate = other.gameObject.GetComponent<Plate>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(TAG.MOVE_PLATE.ToString())) this.triggeredPlate = null;
        else if (other.gameObject.CompareTag(TAG.PIECES.ToString()) && other.transform.root != transform.root)
        {
            IPieces otherPiece = other.gameObject.GetComponent<IPieces>();
            if (!otherPiece.isSelected) otherPiece.SetMaterial(1f);
        }
    }

    void LateUpdate()
    {
        UpdateKinematic();
        if (!transform.position.Equals(finalPos))
        {
            transform.position = finalPos;
        }
    }
    #endregion

    #region MOTION_RELATED_FUNCTION

    #region FORCE_MOTION
    public void ForceSimulatePieceCoord(CoordXY pos) => this.currentPos = pos;
    public void ForceSetPiecePos(CoordXY pos)
    {
        Vector2 vector = Util.ConvertCoordToWorldVector(pos);
        SetPosition(new Vector3(vector.x, transform.position.y, vector.y));
    }
    public void ForceSetPiecePos(Vector2 pos)
    {
        SetPosition(new Vector3(pos.x, transform.position.y, pos.y));
    }
    public void ForceSetPieceHeight(float liftHeight)
    {
        SetPosition(new Vector3(transform.position.x, liftHeight, transform.position.z));
    }
    private void SetPosition(Vector3 pos) => finalPos = pos;
    #endregion

    #region PIECE_MOTION_FUNCTION
    public void SetPieceDropFrom(CoordXY pos)
    {
        Vector2 vector = Util.ConvertCoordToWorldVector(pos);
        DropPiece(new Vector3(vector.x, OtherDefine.PIECE_DROP_HEIGHT, vector.y));
    }

    public void MoveToCaptureQueue(Vector2 captureQueuePos)
    {
        DeActivePiece();
        DropPiece(new Vector3(captureQueuePos.x, OtherDefine.PIECE_DROP_HEIGHT, captureQueuePos.y), OtherDefine.CAPTURE_QUEUE_HEIGHT);
    }

    public void MoveToWithLift(CoordXY pos)
    {
        hasMoved = true;
        this.currentPos = pos;
        Vector2 vectorPos = Util.ConvertCoordToWorldVector(pos);
        StartCoroutine(MoveRoutine(new Vector3(vectorPos.x, 0, vectorPos.y)));
    }
    #endregion

    #region MOTION_ANIMATION
    private void DropPiece(Vector3 startPos, float dropPos = 0f)
    {
        StartCoroutine(DropRoutine(startPos, dropPos));
    }

    IEnumerator DropRoutine(Vector3 startPos, float dropPos)
    {
        Vector3 dropStartPos = startPos;
        Vector3 liftedPos = new Vector3(dropStartPos.x, dropPos, dropStartPos.z);

        float timer = 0f;
        while (timer < PIECE_LIFT_DURATION)
        {
            SetPosition(Vector3.Lerp(dropStartPos, liftedPos, timer / PIECE_LIFT_DURATION));
            timer += Time.deltaTime;
            yield return null;
        }
        SetPosition(liftedPos);
    }

    private IEnumerator MoveRoutine(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        Vector3 liftedPos = new Vector3(startPos.x, PIECE_LIFT_HEIGHT, startPos.z);

        // Fly Up Animation
        float timer = 0f;
        while (timer < PIECE_LIFT_DURATION)
        {
            SetPosition(Vector3.Lerp(startPos, liftedPos, timer / PIECE_LIFT_DURATION));
            timer += Time.deltaTime;
            yield return null;
        }
        SetPosition(liftedPos);

        // Fly to target pos
        Vector3 targetLiftedPos = new Vector3(targetPos.x, liftedPos.y, targetPos.z);
        timer = 0f;
        while (timer < PIECE_MOVE_DURATION)
        {
            SetPosition(Vector3.Lerp(liftedPos, targetLiftedPos, timer / PIECE_MOVE_DURATION));
            timer += Time.deltaTime;
            yield return null;
        }
        SetPosition(targetLiftedPos);

        // Fall Down
        timer = 0f;
        while (timer < PIECE_LIFT_DURATION)
        {
            SetPosition(Vector3.Lerp(targetLiftedPos, targetPos, timer / PIECE_LIFT_DURATION));
            timer += Time.deltaTime;
            yield return null;
        }
        SetPosition(targetPos);
    }
    #endregion
    #endregion

    #region GET_DATA_FUNCTION
    public CoordXY GetCurrentPosition() => this.currentPos;
    public Plate GetTriggeredPlate() => this.triggeredPlate;
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
    #endregion

    #region SET_DATA_FUNCTION
    public void ActivePiece()
    {
        this.isActive = true;
        IPlayerController gm = GameManager.Instance.playerController[(int)this.teamSide];
        mouseAction.OnClick += gm.OnClickEvent;
        mouseAction.OnHoldStart += gm.OnHoldStartEvent;
        mouseAction.OnHoldDrag += gm.OnHoldDragEvent;
        mouseAction.OnHoldEnd += gm.OnHoldEndEvent;
    }

    public void DeActivePiece()
    {
        this.isActive = false;
        IPlayerController gm = GameManager.Instance.playerController[(int)this.teamSide];
        mouseAction.OnClick -= gm.OnClickEvent;
        mouseAction.OnHoldStart -= gm.OnHoldStartEvent;
        mouseAction.OnHoldDrag -= gm.OnHoldDragEvent;
        mouseAction.OnHoldEnd -= gm.OnHoldEndEvent;
    }

    public void SelectPiece() => this.isSelected = true;
    public void DeSelectPiece() => this.isSelected = false;
    public void UpdateKinematic()
    {
        if (rb.IsSleeping()) rb.isKinematic = false;
        else rb.isKinematic = true;
    }

    public void SetMaterial(float alpha = 1f)
    {
        Renderer rend = GetComponent<Renderer>();
        PIECE_MATERIAL material = PIECE_MATERIAL.BLACK;
        if (teamSide == TEAM_SIDE.ALLY) material = PIECE_MATERIAL.WHITE;
        else material = PIECE_MATERIAL.BLACK;
        rend.material = pieceObjectRef.GetPieceMaterial(material, alpha);
    }
    #endregion
}
