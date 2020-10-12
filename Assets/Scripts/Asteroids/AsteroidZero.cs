//In Progress
using UnityEngine;

public class AsteroidZero : AteroidBase
{
    #region Worker Parameters
    private Vector2 targetPoint;
    #endregion

    protected override void Start()
    {
        base.Start();
        type = AsteroidType.Zero;
    }

    protected override void StartMove()
    {
        targetPoint = new Vector2(ScreenBounds.RandomXCoord(15, out _), ScreenBounds.centre.y);
    }

    protected override void Move()
    {
        if (isInAttractionField) return;

        Vector2 force = (targetPoint - mRigidBody.position) * speed * 0.3f;
        mRigidBody.AddForce(force, ForceMode2D.Force);

    }
}
