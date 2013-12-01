using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class LightControl : MonoBehaviour
{
    public GameObject moon;
    public Light lightMoon;

    public GameObject sun;
    public Light lightSun;

    public Gradient colorSun;
    public Gradient colorDay;
    public Gradient colorNight;

    public float sunMaxIntensity = 0.4f;
    public float moonMaxIntensity = 0.05f;

    private Transform cachedTransform;
    private Material skybox;

    private Material sunMaterial;
    private float lightRotation;

    private void Start()
    {
        cachedTransform = transform;
        skybox = RenderSettings.skybox;
        sunMaterial = sun.renderer.material;
        lightRotation = Random.Range(-5f, 5f);
    }

    private void Update()
    {
        RotateToTime();
    }

    private void RotateToTime()
    {
        float complete = WorldTime.Instance.phaseComplete;

        float angle = complete * 180f;

        if (!WorldTime.Instance.isDayTime)
            angle += 180f;

        cachedTransform.localRotation = Quaternion.AngleAxis(angle, Vector3.right) * Quaternion.AngleAxis(lightRotation, Vector3.up);

        if (WorldTime.Instance.isDayTime)
        {
            skybox.color = colorDay.Evaluate(complete);
            lightSun.color = colorSun.Evaluate(complete);
            sunMaterial.color = colorSun.Evaluate(complete);
        }
        else
        {
            if (complete < 0.05f)
            {
                lightSun.intensity = Mathf.Lerp(sunMaxIntensity, 0f, complete / 0.2f);
            }
            else if (complete > 0.95f)
            {
                lightSun.intensity = Mathf.Lerp(0f, sunMaxIntensity, (complete - 0.95f) / 0.05f);
                lightSun.color = colorSun.Evaluate(0f);
            }
            else
            {
                lightSun.intensity = 0f;
            }

            lightMoon.intensity = moonMaxIntensity;

            skybox.color = colorNight.Evaluate(complete);
        }
    }
}
