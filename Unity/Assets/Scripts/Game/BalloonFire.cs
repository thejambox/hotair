using UnityEngine;
using System.Collections;

public class BalloonFire : MonoBehaviour
{
    public ParticleSystem psFire;
    public Light lightFire;

    public bool isLit;

    private float lightIntensityBase;

    private void Start()
    {
        isLit = false;
        lightIntensityBase = 8;
    }

    private void Update()
    {
        if (isLit)
        {
            if (!psFire.isPlaying)
                psFire.Play();

            lightFire.intensity = Mathf.Lerp(lightIntensityBase / 2f, lightIntensityBase, MathTools.CosineWave(Time.time * 8f));
        }
        else if (lightFire.intensity > 0)
        {
            if (psFire.isPlaying)
            {
                psFire.Stop();
            }

            lightFire.intensity = Mathf.Lerp(lightFire.intensity, 0f, Time.deltaTime * 4f);
        }
    }
}
