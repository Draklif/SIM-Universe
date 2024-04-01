using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    public float radius;
    public float mass;
    public float surfaceGravity;
    public Vector3 initialVelocity;

    public Vector3 velocity { get; private set; }
    public Vector3 position { get; private set; }

    void Awake()
    {
        velocity = initialVelocity;
        position = transform.position;
    }

    public void UpdateVelocity(Vector3 acceleration, float time)
    {
        velocity += acceleration * time;
    }

    public void UpdatePosition(float time)
    {
        position += velocity * time;
        transform.position = position;
    }

    public void BuildPlanet()
    {
        mass = surfaceGravity * radius * radius / Universe.gravitationalConstant;
        transform.localScale = Vector3.one * radius;
    }

    void OnValidate()
    {
        BuildPlanet();
    }
}
