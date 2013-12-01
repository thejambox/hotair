using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindLayer
{
    public float altitude = 0;
    public float windForce = 0;
    public float lifetime = 0;
    public Vector3 heading;

    public WindLayer(float altitude, float windForce)
    {
        this.altitude = altitude;
        this.windForce = windForce;
    }

    public void Update()
    {
        if (lifetime <= 0f)
            ChangeDirection();

        lifetime -= Time.deltaTime;
    }

    private void ChangeDirection()
    {
        lifetime = Random.Range(2.5f, 10f);
        heading = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up) * Vector3.right;
    }
}

public class Wind : MonoBehaviour
{
    private static Wind instance = null;

    private List<WindLayer> layers;
    private List<Rigidbody> items;

    private void Awake()
    {
        instance = this;

        layers = new List<WindLayer>();

        layers.Add(new WindLayer(40f, 2f));
        layers.Add(new WindLayer(90f, 3.5f));
        layers.Add(new WindLayer(180f, 5f));
        layers.Add(new WindLayer(250f, 8f));
        layers.Add(new WindLayer(400f, 10f));
        layers.Add(new WindLayer(600f, 15f));


        items = new List<Rigidbody>();
    }

    private void Update()
    {
        for (int i = 0; i < layers.Count; ++i)
        {
            layers[i].Update();
        }

        for (int i = items.Count - 1; i >= 0; --i)
        {
            if (items[i] == null)
            {
                items.RemoveAt(i);
                continue;
            }

            float altitude = items[i].transform.position.y;
            WindLayer layer = GetLayerForAltitude(altitude);

            if (layer != null)
                items[i].AddForce(layer.windForce * layer.heading);
        }
    }

    private WindLayer GetLayerForAltitude(float altitude)
    {
        WindLayer result = null;

        for (int i = 0; i < layers.Count; ++i)
        {
            if (layers[i].altitude < altitude)
                result = layers[i];
            else
                break;
        }

        return result;
    }

    public static void Affect(Rigidbody rb)
    {
        instance.items.Add(rb);
    }
}
