
using UnityEngine;

public class ForceNoRotation : MonoBehaviour
{
    
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
