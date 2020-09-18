//In Progress
using UnityEngine;

public class PowerUpOrb : OutsideSpawnable
{
    #region Inspector Parameters
    [Header("Properties")]
    public PowerType powerType;
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
