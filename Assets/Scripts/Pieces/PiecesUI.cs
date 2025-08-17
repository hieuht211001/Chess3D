using System;
using UnityEngine;
using static GeneralDefine;
using static PiecesDefine;

[CreateAssetMenu(fileName = "PieceUI", menuName = "Scriptable Objects/PieceUI")]
public class PiecesUI : ScriptableObject
{
    [Serializable]
    private class PieceMaterial
    {
        public Material material;
        public PIECE_MATERIAL name;
    }

    [SerializeField] private PieceMaterial[] pieceMaterialArray;
    public Material GetPieceMaterial(PIECE_MATERIAL teamColor, float alpha)
    {
        try
        {
            foreach (var pieceInfo in pieceMaterialArray)
            {
                if (pieceInfo.name == teamColor)
                {
                    if (alpha != 1f)
                    {
                        return MakeTransparent(pieceInfo.material, alpha);
                    }
                    else return pieceInfo.material;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }

    public Material MakeTransparent(Material original, float alpha)
    {
        if (original == null) return null;

        Material transparentMat = new Material(original);

        transparentMat.SetFloat("_Mode", 3);
        transparentMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        transparentMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        transparentMat.SetInt("_ZWrite", 0);
        transparentMat.DisableKeyword("_ALPHATEST_ON");
        transparentMat.EnableKeyword("_ALPHABLEND_ON");
        transparentMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        transparentMat.renderQueue = 3000;

        Color c = transparentMat.color;
        c.a = alpha;
        transparentMat.color = c;

        return transparentMat;
    }

    [Serializable]
    private class PieceType
    {
        public GameObject pieceObject;
        public PIECE_TYPE type;
    }

    [SerializeField] private PieceType[] pieceTypeArray;
    public GameObject GetPieceObject(PIECE_TYPE pieceType)
    {
        try
        {
            foreach (var pieceInfo in pieceTypeArray)
            {
                if (pieceInfo.type == pieceType)
                    return pieceInfo.pieceObject;
            }
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }
}
