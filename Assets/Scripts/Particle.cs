using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField] Transform Spawner;
    [SerializeField] float DampingFactor;
    [SerializeField] float Mass;
    [SerializeField] float LaunchVel;
    [SerializeField] float LaunchPitch;
    [SerializeField] float LaunchYaw;
    [SerializeField] Transform Floor;

    Vector3 Pos;
    Vector3 Vel;
    Vector3 Force;

    float clamp;

    public void RemoteStart()
    {
        // Defaults
        transform.localScale = Vector3.one;

        Pos = Spawner.position;

        float velX = LaunchVel * Mathf.Cos(Mathf.Deg2Rad * LaunchYaw);
        float velY = LaunchVel * Mathf.Sin(Mathf.Deg2Rad * LaunchYaw);
        float velZ = LaunchVel * Mathf.Sin(Mathf.Deg2Rad * LaunchPitch);

        Vel = new Vector3(velX, velY, velZ);

        transform.localScale *= Mass * 0.5f;
        clamp = Floor.position.y + 0.15f + transform.localScale.y * 0.5f;
    }

    public void Shoot(float Steps, float FrictionMag, float GravityMag)
    {
        Vector3 Acceleration = Force / Mass;

        Vel = Vel + (Steps * Acceleration);
        Pos = Pos + (Steps * Vel);

        CalculatePosition();
        CalculateFloor();
        CalculateForce(FrictionMag, GravityMag);
    }

    public void CalculateFloor()
    {
        if (Pos.y <= clamp)
        {
            Pos.y = clamp;
            Vel.y *= -DampingFactor;
        }
    }

    public void CalculateForce(float FrictionMag, float GravityMag)
    {
        Vector3 Gravity = new Vector3(0, -GravityMag * Mass, 0);
        Vector3 Friction = -FrictionMag * Vel.normalized * Mass;

        if (Pos.y > clamp)
        {
            Friction = Vector3.zero;
        }

        Force = Gravity + Friction;
    }

    public void CalculatePosition()
    {
        transform.position = Pos;
    }

    public void Bounce()
    {
        Vel.x *= -DampingFactor;
        Vel.z *= -DampingFactor;
    }
}
