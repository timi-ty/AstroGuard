using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class OutsideSpawnableBase : MonoBehaviour, IAttractible
{
    #region Components
    protected Rigidbody2D mRigidBody { get; set; }
    protected Collider2D mCollider { get; set; }
    #endregion

    #region Inspector Parameters
    [Header("Properties")]
    public float maxImpulseMagnitude;
    public float minImpulseMagnitude;
    #endregion

    #region Properties
    public bool isInAttractionField { get; set; }
    #endregion

    #region Virtual Methods

    #region Unity Runtime
    protected virtual void Start()
    {
        mRigidBody = GetComponent<Rigidbody2D>();

        mCollider = GetComponent<Collider2D>();

        mCollider.isTrigger = true;

        ApplyImpulse();

        InvokeRepeating("DestroyIfLostInSpace", 3.0f, 3.0f);
    }

    protected virtual void Update()
    {
        if (ScreenBounds.IsInsideScreenBounds(mCollider.bounds))
        {
            OnEnterScreen();
        }
    }
    #endregion

    protected virtual void ApplyImpulse()
    {
        Vector2 destination = ScreenBounds.topEdge.middle;
        Vector2 direction = (destination - mRigidBody.position).normalized;

        float impulseMagnitude = Random.Range(minImpulseMagnitude, maxImpulseMagnitude);

        mRigidBody.AddForce(direction * impulseMagnitude, ForceMode2D.Impulse);
    }

    protected virtual void OnEnterScreen()
    {
        mCollider.isTrigger = false;
        enabled = false;
    }

    protected virtual void DestroyIfLostInSpace()
    {
        if (ScreenBounds.IsOutOfPlayableArea(mCollider.bounds))
        {
            Destroy(gameObject);
        }
    }

    public void OnEnterAttractionField()
    {
        isInAttractionField = true;

        if (mRigidBody == null) return;

        if (mRigidBody)
        {
            mRigidBody.gravityScale = -0.5f;
        }
    }

    public void RecieveAttractionForce(Vector2 sourcePoint, float forceMagnitude)
    {
        if (mRigidBody == null) return;

        Vector2 forceDirection = (sourcePoint - mRigidBody.position).normalized;

        mRigidBody.AddForce(forceDirection * forceMagnitude, ForceMode2D.Force);
    }

    public void OnExitAttractionField()
    {
        isInAttractionField = false;

        if (mRigidBody == null) return;

        if (mRigidBody)
        {
            mRigidBody.gravityScale = 1;
        }
    }

    #endregion
}
