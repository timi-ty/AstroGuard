using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMoRings : MonoBehaviour
{
    public ParticleSystem ringSystem1;
    public ParticleSystem ringSystem2;

    public void StartEmitting()
    {
        ParticleSystem.EmissionModule emission1 = ringSystem1.emission;
        ParticleSystem.EmissionModule emission2 = ringSystem2.emission;

        emission1.enabled = true;
        emission2.enabled = true;
    }

    public void StopEmitting()
    {
        ParticleSystem.EmissionModule emission1 = ringSystem1.emission;
        ParticleSystem.EmissionModule emission2 = ringSystem2.emission;

        emission1.enabled = false;
        emission2.enabled = false;
    }
}
