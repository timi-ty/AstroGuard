using UnityEngine;
using System.Collections;

public class DamageLayer : MonoBehaviour
{
    #region Components
    private SpriteRenderer mSpriteRenderer;
    private Animator mAnimator;
    #endregion

    #region Effects
    [Header("Effects")]
    public ParticleSystem smallDamageFX;
    public ParticleSystem bigDamageFX;
    public ParticleSystem destroyFX;
    public ParticleSystem respawnFX;

    [Header("Sprites")]
    public Sprite damageOne;
    public Sprite damageTwo;
    #endregion

    private void Awake()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mAnimator = GetComponent<Animator>();
        mAnimator.enabled = false;
    }

    
    public void ShowDamage(int health)
    {
        switch (health)
        {
            case 2:
                smallDamageFX.Play();
                mSpriteRenderer.sprite = damageOne;
                break;

            case 1:
                bigDamageFX.Play();
                mSpriteRenderer.sprite = damageTwo;
                break;
        }
    }

    public void ShowDestruction()
    {
        mAnimator.enabled = true;
        destroyFX.Play();
        Debug.Log("Ship Gone");
    }

    public void RemoveAllDamage()
    {
        smallDamageFX.Stop();
        bigDamageFX.Stop();
        destroyFX.Stop();

        mAnimator.enabled = false;
        respawnFX.Play();

        mSpriteRenderer.sprite = null;
    }
}
