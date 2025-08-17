using System;
using UnityEngine;
using static GeneralDefine;
using static OtherDefine;

[CreateAssetMenu(fileName = "PlateUI", menuName = "Scriptable Objects/PlateUI")]
public class PlateUI : ScriptableObject
{
    [Serializable]
    private class PlatePrefab
    {
        public GameObject plateObject;
        public PLATE_TYPE type;
    }

    [SerializeField] private PlatePrefab[] platePrefabs;
    public GameObject GetPlateObject(PLATE_TYPE type)
    {
        try
        {
            foreach (PlatePrefab pieceInfo in platePrefabs)
            {
                if (pieceInfo.type == type)
                    return pieceInfo.plateObject;
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
