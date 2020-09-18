using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotationAxis;
    public float rotationSpeed;


    void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * rotationSpeed, rotationAxis);
    }
}