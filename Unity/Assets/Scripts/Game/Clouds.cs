using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour
{
    public int cloudMin;
    public int cloudMax;

    private ParticleSystem clouds;

    void Start()
    {
        clouds = GetComponent<ParticleSystem>();
        clouds.Emit(Random.Range(cloudMin, cloudMax));
    }
}
