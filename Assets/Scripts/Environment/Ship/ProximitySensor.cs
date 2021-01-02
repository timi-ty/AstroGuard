using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]
public class ProximitySensor : MonoBehaviour
{
    #region Components
    private CircleCollider2D mCollider;
    #endregion

    #region Properties
    private ContactFilter2D contactFilter;
    private Collider2D[] nearbyObjects;
    private int sensorLayerMask;
    #endregion

    #region Events
    public UnityEvent OnSensedSingle = new UnityEvent();
    public UnityEvent OnSensedMultiple = new UnityEvent();
    public UnityEvent OnSensedSpacedMultiple = new UnityEvent();
    #endregion

    private void Start()
    {
        mCollider = GetComponent<CircleCollider2D>();
        sensorLayerMask = LayerMask.GetMask("Default");
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(sensorLayerMask);
        nearbyObjects = new Collider2D[8];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnSensedSingle?.Invoke();

        if(CountNearbyObjects() >= 2)
        {
            TriggerMultiSensor();
        }
    }

    private int CountNearbyObjects()
    {
        return mCollider.OverlapCollider(contactFilter, nearbyObjects);
    }

    private bool IsNearbyObjectsSpaced()
    {
        if (nearbyObjects[0] == null) return false;

        float minDist = 0f;
        float maxDist = 0f;

        Transform refTransform = nearbyObjects[0].transform;

        foreach(Collider2D nearbyObject in nearbyObjects)
        {
            if (nearbyObject)
            {
                float dist = (refTransform.position - nearbyObject.transform.position).x;

                minDist = Mathf.Min(dist, minDist);
                maxDist = Mathf.Max(dist, maxDist);
            }

            if (maxDist - minDist >= ScreenBounds.width / 3.0f)
            {
                return true;
            }
        }

        return false;
    }

    private void TriggerMultiSensor()
    {
        OnSensedMultiple?.Invoke();

        if (IsNearbyObjectsSpaced())
        {
            OnSensedSpacedMultiple?.Invoke();
        }
    }
}
