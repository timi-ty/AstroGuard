using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithUniversal : MonoBehaviour
{

    public bool preserveExistingScale;

    private void Start()
    {
        if (preserveExistingScale)
        {
            transform.localScale *= GameManager.universalGameScale;
        }
        else
        {
            transform.localScale = Vector3.one * GameManager.universalGameScale;
        }   
    }
}
