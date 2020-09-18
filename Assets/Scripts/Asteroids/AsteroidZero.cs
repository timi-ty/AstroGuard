//In Progress
using UnityEngine;

public class AsteroidZero : AteroidBase
{
    protected override void Start()
    {
        base.Start();
        type = Type.Zero;
    }

    protected override void StartMove()
    {
        //Do nothing.
    }

    protected override void Move()
    {
        if (isInAttractionField) return;

        mRigidBody.MovePosition(Vector2.MoveTowards(mRigidBody.position, Vector2.down * 100, speed * Time.fixedDeltaTime));
    }
}
