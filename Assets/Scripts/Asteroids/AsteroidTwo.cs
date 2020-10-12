using UnityEngine;
using System.Collections;

public class AsteroidTwo : AteroidBase
{

    protected override void Start()
    {
        base.Start();
        type = AsteroidType.Two;
    }

    protected override void Move()
    {
        //Do nothing.
    }

    protected override void StartMove()
    {
        Vector2 leftAttackVector = new Vector2(ScreenBounds.leftEdge.middle.x - mRigidBody.position.x, 
            Random.Range(ScreenBounds.leftEdge.topVertex.y, ScreenBounds.leftEdge.middle.y) - mRigidBody.position.y).normalized;
        Vector2 rightAttackVector = new Vector2(ScreenBounds.rightEdge.middle.x - mRigidBody.position.x,
            Random.Range(ScreenBounds.rightEdge.topVertex.y, ScreenBounds.rightEdge.middle.y) - mRigidBody.position.y).normalized;

        int decider = Random.Range(0, 2);

        Vector2 attackVector = decider == 0 ? leftAttackVector : rightAttackVector;

        mRigidBody.AddForce(attackVector * speed * 1.5f, ForceMode2D.Impulse);
    }
}
