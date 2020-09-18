//In Progress
using UnityEngine;

public class Missile : MonoBehaviour
{
    #region Inspector Parameters
    [Header("Movement Properties")]
    public float speed;
    public float waveAmplitude;
    public float waveSpeed;
    [Header("Effects")]
    public GameObject destructionEffect;
    #endregion

    #region Properties
    [Header("Sound Effects")]
    public AudioClip launchClip;
    #endregion

    #region Worker Parameters
    private float angle;
    private Transform target;
    #endregion

    private void Start()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        MoveMissile();
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        Destruct(0);
    }

    public void LockTarget(Transform target)
    {
        this.target = target;
    }

    private void Initialize()
    {
        Destruct(5);

        AudioManager.PlayGameClip(launchClip);
    }

    private void MoveMissile()
    {
        if (target)
        {
            Vector2 direction = target ? (target.position - transform.position).normalized : transform.up;

            angle += (waveSpeed * 2 * Mathf.PI * Time.fixedDeltaTime) % (2 * Mathf.PI);
            float nudgeValue = waveAmplitude * Mathf.Sin(angle);

            Vector3 waveVector = Vector2.Perpendicular(direction) * nudgeValue;

            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime);
            transform.position += waveVector;

            float rotation = Vector2.SignedAngle(Vector2.up, direction);
            transform.eulerAngles = new Vector3(0, 0, rotation);
        }
        else
        {
            LockTarget(FindTarget());
        }
    }

    private void CreateDestructionEffect()
    {
        Instantiate(destructionEffect, transform.position, Quaternion.identity);
    }

    private Transform FindTarget()
    {
        float searchRadius = 0.1f;

        while(searchRadius < ScreenBounds.width)
        {
            Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, searchRadius);

            foreach (Collider2D potentialTarget in potentialTargets)
            {
                if (potentialTarget.CompareTag("Enemy"))
                {
                    return potentialTarget.transform;
                }
            }

            searchRadius += 0.1f;
        }

        Destruct(0);

        return null;
    }

    private void Destruct(float t)
    {
        if (t > 0)
        {
            Destroy(gameObject, t);
            Invoke("CreateDestructionEffect", t);
        }
        else
        {
            enabled = false;
            Destroy(gameObject);
            CreateDestructionEffect();
        }
    }
}