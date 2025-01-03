using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light areaLight;
    public ParticleSystem sparkParticles;
    private float normalIntensity = 0.1f;
    private float offIntensity = 0.0f;
    private float flickerDuration = 0.05f;
    private float flickerChance = 0.1f;
    private float timeBetweenFlickers = 2.0f;

    private float nextFlickerTime;
    private bool isFlickering = false;
    private float flickerEndTime;

    void Start()
    {
        if (areaLight == null)
        {
            areaLight = GetComponent<Light>();
        }
        if (sparkParticles != null)
        {
            sparkParticles.Stop();
        }
        nextFlickerTime = Time.time + timeBetweenFlickers;
    }

    void Update()
    {
        if (isFlickering)
        {
            if (Time.time >= flickerEndTime)
            {
                areaLight.intensity = normalIntensity;
                isFlickering = false;
                nextFlickerTime = Time.time + timeBetweenFlickers;

                if (sparkParticles != null && sparkParticles.isPlaying)
                {
                    sparkParticles.Stop();
                }
            }
        }
        else if (Time.time >= nextFlickerTime)
        {
            if (Random.value < flickerChance)
            {
                areaLight.intensity = offIntensity;
                isFlickering = true;
                flickerEndTime = Time.time + flickerDuration;
                if (sparkParticles != null)
                {
                    sparkParticles.Play();
                }
            }
        }
    }
}