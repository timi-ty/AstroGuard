//In Progress
using UnityEngine;

public class EnvironmentPS : MonoBehaviour
{
    public float extraHeight;
    private ParticleSystem mParticleSystem;

    void Start()
    {
        SetPosition();
        SetSize();
    }

    private void SetPosition()
    {
        mParticleSystem = GetComponent<ParticleSystem>();

        ParticleSystem.ShapeModule shape = mParticleSystem.shape;

        shape.position = ScreenBounds.topEdge.middle + (Vector2.up * extraHeight);
    }

    private void SetSize()
    {
        mParticleSystem = GetComponent<ParticleSystem>();

        ParticleSystem.ShapeModule shape = mParticleSystem.shape;

        shape.radius = ScreenBounds.width / 2;
    }
}
