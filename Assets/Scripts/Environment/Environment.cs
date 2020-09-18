//In Progress
using UnityEngine;

[System.Serializable]
public struct EnvironmentSettings
{
    public int backgroundAssetIndex;
}

public class Environment : MonoBehaviour
{

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        ScreenBounds.CreateColliders(top: false, bottom: false, left: true, right: true, parent: transform, tag: "Wall");
    }
}
