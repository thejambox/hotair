using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicLoop : MonoBehaviour
{
    public AudioClip music;

    void Start()
    {
        audio.clip = music;
    }

    void Update()
    {

    }
}
