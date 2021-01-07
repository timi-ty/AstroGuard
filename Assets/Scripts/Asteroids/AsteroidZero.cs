//In Progress
using UnityEngine;

public class AsteroidZero : AsteroidBase
{
    #region Worker Parameters
    private Vector2 targetPoint;
    private bool useManualTargetPoint;
    #endregion

    protected override void Start()
    {
        base.Start();
        type = AsteroidType.Zero;
    }

    protected override void StartMove()
    {
        if (useManualTargetPoint) return;

        targetPoint = new Vector2(ScreenBounds.RandomXCoord(15, out _), ScreenBounds.centre.y);
    }

    protected override void Move()
    {
        if (isInAttractionField) return;

        Vector2 force = (targetPoint - mRigidBody.position) * speed * 0.3f;
        mRigidBody.AddForce(force, ForceMode2D.Force);

    }

    public void ManualTargetPoint(Vector2 targetPoint)
    {
        useManualTargetPoint = true;
        this.targetPoint = targetPoint;
    }
}
