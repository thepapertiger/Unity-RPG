using UnityEngine;
using UnityEditor;

public class ItemAssetUtility
{
    [MenuItem("Assets/Create/Game Item")]
    public static void CreateItem()
    {
        ScriptableObjectUtility.CreateAsset<ItemBase>();
    }
}