using UnityEngine;
using System.Collections;

public class AsteroidOne : AteroidBase
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
        type = Type.One;
    }

    protected override void Move()
    {
        if (isInAttractionField) return;

        if (mRigidBody.position.y > triggerYCoordinate && !triggered)
        {
            mRigidBody.MovePosition(Vector3.MoveTowards(mRigidBody.position, fakeTargetPoint, speed * Time.fixedDeltaTime));
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

        triggerYCoordinate = Random.Range(ScreenBounds.centre.y + 0.05f * ScreenBounds.height, ScreenBounds.centre.y + 0.25f * ScreenBounds.height);
    }
}
