using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
public class RandomMaterialColor : MonoBehaviour
{
    public float minHue;
    public float maxHue;
    public float minSaturation;
    public float maxSaturation;
    public float minValue;
    public float maxValue;

    private void Start()
    {
        Material mMaterial = GetComponent<MeshRenderer>().material;

        float hue = Random.Range(minHue, maxHue) / 360;
        float sat = Random.Range(minSaturation, maxSaturation) / 100;
        float val = Random.Range(minValue, maxValue) / 100;

        Color color = Color.HSVToRGB(hue, sat, val);

        mMaterial.color = color;
    }
}