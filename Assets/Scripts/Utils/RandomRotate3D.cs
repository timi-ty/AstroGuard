using UnityEngine;

public class RandomRotate3D : MonoBehaviour
{
    private Vector3 rotationAxisSecondary;
    private Vector3 rotationAxis;
    public float rotationSpeed;

    void Start()
    {
        rotationAxis = Random.insideUnitSphere;
        rotationAxisSecondary = Random.insideUnitSphere;
    }

    void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * rotationSpeed, rotationAxis);
        rotationAxis = Vector3.RotateTowards(rotationAxis, rotationAxisSecondary, Time.deltaTime * rotationSpeed, 0);
    }
}