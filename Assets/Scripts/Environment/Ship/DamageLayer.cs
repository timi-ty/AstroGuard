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

    [Header("SFX")]
    public AudioClip damageClip;
    public AudioClip destroyClip;
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
                AudioManager.PlayGameClip(damageClip);
                break;

            case 1:
                bigDamageFX.Play();
                mSpriteRenderer.sprite = damageTwo;
                AudioManager.PlayGameClip(damageClip);
                break;
        }
    }

    public void ShowDestruction()
    {
        mAnimator.enabled = true;
        destroyFX.Play();
        AudioManager.PlayGameClip(destroyClip);
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
