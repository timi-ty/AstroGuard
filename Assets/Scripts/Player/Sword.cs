//In Progress
using UnityEngine;
using System.Collections.Generic;

public class Sword : MonoBehaviour
{
    #region Inspector Parameters
    [Header("Effects")]
    public GameObject sparkEffect;
    [Header("Settings")]
    [Tooltip("Maximum speed of the sword in revolutions per second")]
    public float maxSpeed;
    [Tooltip("Acceleration of the sword in revolutions per second squared")]
    public float acceleration;
    #endregion

    #region Internal
    internal float speed { get; set; }
    internal bool isAccelerating { get; set; }
    #endregion

    #region Components
    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    #endregion

    #region Extensions
    [Header("Extensions")]
    public List<TrailRenderer> trailRenderers;
    #endregion

    #region Properties
    private Color originalBladeColor { get; set; }
    #endregion

    void FixedUpdate()
    {
        SpeedControl();
        RotateSword();
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        CreateSpark(other.GetContact(0).point);
    }

    private void SpeedControl()
    {
        if (isAccelerating)
        {
            speed += acceleration * Time.fixedUnscaledDeltaTime;
        }
        else
        {
            speed -= (acceleration / 8) * Time.fixedUnscaledDeltaTime;
        }

        speed = Mathf.Clamp(speed, 1, maxSpeed);
    }

    private void RotateSword()
    {
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.z -= (speed * 360) * Time.fixedUnscaledDeltaTime;
        transform.eulerAngles = eulerAngles;
    }

    public void StartAccelerating()
    {
        isAccelerating = true;
    }

    public void StopAccelerating()
    {
        isAccelerating = false;
    }

    private void CreateSpark(Vector2 contactPoint)
    {
        Instantiate(sparkEffect, contactPoint, Quaternion.identity);
    }

    public void ChangeAppearance(Sprite sprite, Color color)
    {
        spriteRenderer.sprite = sprite;
        originalBladeColor = color;
    }

    public void UpdateColor(float anger)
    {
        Color newColor = ((1 - anger) * originalBladeColor) + (anger * Color.red);
        spriteRenderer.color = newColor;

        foreach (TrailRenderer trail in trailRenderers)
        {
            Gradient gradient = trail.colorGradient;

            gradient.colorKeys[0] = new GradientColorKey(newColor, 0);
            gradient.colorKeys[1] = new GradientColorKey(newColor, 1);

            trail.colorGradient = gradient;

            trail.startColor = trail.endColor = newColor;
        }
    }
}
