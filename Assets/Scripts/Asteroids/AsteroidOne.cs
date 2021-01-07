using UnityEngine;
using System.Collections;

public class AsteroidOne : AsteroidBase
{
    #region Worker Parameters
    private Vector2 fakeTargetPoint;
    private Vector2 launchForce;
    private float triggerYCoordinate;
    private bool triggered;
    #endregion


    protected override void Start()
    {
        base.Start();
        type = AsteroidType.One;
    }

    protected override void Move()
    {
        if (isInAttractionField) return;

        if (mRigidBody.position.y > triggerYCoordinate && !triggered)
        {
            Vector2 force = (fakeTargetPoint - mRigidBody.position) * speed * 0.1f;
            mRigidBody.AddForce(force, ForceMode2D.Force);
        }
        else if(!triggered)
        {
            mRigidBody.AddForce(launchForce, ForceMode2D.Impulse);
            mRigidBody.gravityScale = 1;
            triggered = true;
        }
    }

    protected override void StartMove()
    {
        fakeTargetPoint = new Vector2(ScreenBounds.RandomXCoord(15, out _), ScreenBounds.centre.y);
        launchForce = Random.insideUnitCircle * 3;
        launchForce.y = Mathf.Abs(launchForce.y);
        launchForce.y = Mathf.Clamp(launchForce.y, 1, 20);

        triggerYCoordinate = Random.Range(ScreenBounds.topEdge.middle.y - 0.15f * ScreenBounds.height, ScreenBounds.topEdge.middle.y - 0.35f * ScreenBounds.height);
    }
}
