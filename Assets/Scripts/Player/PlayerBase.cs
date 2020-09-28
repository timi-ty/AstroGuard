//In Progress
using UnityEngine;
using System.Collections;


public abstract class PlayerBase : MonoBehaviour
{
    #region Inspector Parameters
    [Header("Movement Settings")]
    public float agility;
    #endregion

    #region Components
    protected Rigidbody2D mRigidBody { get; set; }
    #endregion

    #region Internal
    internal Vector2 worldInputPos;
    #endregion

    #region Unity Runtime
    protected virtual void Start()
    {
        mRigidBody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        GetInput();
    } 

    protected virtual void FixedUpdate()
    {
        MovePlayer();
    }
    #endregion

    #region Game Events
    public virtual void OnInitialize()
    {
        enabled = false;
    }

    public virtual void OnPlay()
    {
        enabled = true;
    }

    public virtual void OnPause()
    {
        enabled = false;
    }

    public virtual void OnResume()
    {
        enabled = true;

        OnScreenReleased();
    }

    public virtual void OnContinue()
    {
        Respawn();
    }
    #endregion

    #region Abstract Methods
    protected abstract void OnScreenPressed();

    protected abstract void OnScreenReleased();

    protected abstract void Respawn();

    public abstract void Die();
    #endregion

    #region Sealed Methods
    public void SetScale(float size)
    {
        transform.localScale = Vector3.one * size;
    }

    public void MovePlayer()
    {
        float newPosX = Mathf.Lerp(mRigidBody.position.x, worldInputPos.x, agility * Time.fixedUnscaledDeltaTime);

        float deltaPositionX = newPosX - mRigidBody.position.x;

        mRigidBody.MovePosition(mRigidBody.position + Vector2.right * deltaPositionX);
    }

    private void GetInput()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            worldInputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnScreenPressed();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnScreenReleased();
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            worldInputPos = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                OnScreenPressed();
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                OnScreenReleased();
            }
        }
#endif
    }
    #endregion
}
