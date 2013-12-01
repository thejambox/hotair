using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour
{
    public Transform tFollow;
    public Vector3 followVector;

    private Transform cachedTransform;

    private void Start()
    {
        cachedTransform = transform;
    }

    private void Update()
    {
        cachedTransform.position = Vector3.Scale(tFollow.position, followVector);
    }
}
