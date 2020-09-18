using UnityEngine;
using System.Collections;


[RequireComponent(typeof(ParticleSystem))]
public class AsteroidDebris : EffectBase
{
    #region Components
    [Header("Components")]
    public ParticleSystem sparkSystem;
    #endregion


    public void Initialize(Color color, Transform floor)
    {
        Color.RGBToHSV(color, out float hue, out float sat, out float val);

        ParticleSystemRenderer mParticleSystemRenderer = GetComponent<ParticleSystemRenderer>();

        ParticleSystem mParticleSystem = GetComponent<ParticleSystem>();


        mParticleSystemRenderer.material.color = Color.HSVToRGB(hue, sat, val);

        ParticleSystem.MainModule main = sparkSystem.main;

        ParticleSystem.MinMaxGradient startColor = new ParticleSystem.MinMaxGradient(Color.HSVToRGB(hue + 0.06f, 0.25f, val), Color.HSVToRGB(hue, 0.20f, val));
        main.startColor = startColor;

        mParticleSystem.collision.SetPlane(0, floor);
    }
}
