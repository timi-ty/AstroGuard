using UnityEngine;
using System.Collections;

public class GoldCoin : OutsideSpawnableBase
{

    #region Inspector Parameters
    [Header("Effects")]
    public GameObject collectionFX;
    #endregion

    #region Properties
    public bool isCollected { get; set; }
    #endregion

    public void OnCollected()
    {
        isCollected = true;

        Instantiate(collectionFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
