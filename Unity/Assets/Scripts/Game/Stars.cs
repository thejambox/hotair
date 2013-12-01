using UnityEngine;
using System.Collections;

public class Stars : MonoBehaviour
{
    private ParticleSystem stars;

    private bool starsOn;

    private void Start()
    {
        stars = GetComponent<ParticleSystem>();

        starsOn = false;
    }

    private void Update()
    {
        if (!WorldTime.Instance.isDayTime && !starsOn)
        {
            stars.Play();

            starsOn = true;
        }
        else if (WorldTime.Instance.isDayTime && starsOn)
        {
            ParticleSystem.Particle[] ps = new ParticleSystem.Particle[stars.particleCount];

            stars.GetParticles(ps);

            for (int i = 0; i < ps.Length; ++i)
                ps[i].lifetime = Random.Range(0.25f, 1f);

            stars.SetParticles(ps, ps.Length);

            stars.Stop();

            starsOn = false;
        }
    }
}
