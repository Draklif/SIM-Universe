using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField] Transform Spawner;
    [SerializeField] float DampingFactor;
    [SerializeField] float LaunchVel;
    [SerializeField] float LaunchPitch;
    [SerializeField] float LaunchYaw;
    [SerializeField] float Mass;
    [SerializeField] float Restitution;
    [SerializeField] Transform RoomCenter;

    Vector3 Pos;
    Vector3 Force;
    Vector3 Vel;

    float floor, wallX, wallZ;
    float roomHeight = 7f;
    float roomWidth = 12f;
    float roomDepth = 12f;

    public void RemoteStart()
    {
        // Defaults
        transform.localScale = Vector3.one;

        Pos = Spawner.position;

        float velX = LaunchVel * Mathf.Cos(Mathf.Deg2Rad * LaunchYaw);
        float velY = LaunchVel * Mathf.Sin(Mathf.Deg2Rad * LaunchPitch);
        float velZ = LaunchVel * Mathf.Sin(Mathf.Deg2Rad * LaunchYaw);

        Vel = new Vector3(velX, velY, velZ);

        transform.localScale *= Mass * 0.5f;
        floor = RoomCenter.position.y + 0.15f + transform.localScale.y * 0.5f;
        wallX = RoomCenter.position.x + 0.15f + transform.localScale.y * 0.5f - roomWidth * 0.5f;
        wallZ = RoomCenter.position.z + 0.15f + transform.localScale.y * 0.5f - roomDepth * 0.5f;
    }

    public void Shoot(float Steps, float FrictionMag, float GravityMag)
    {
        Vector3 Acceleration = Force / Mass;

        Vel = Vel + (Steps * Acceleration);
        Pos = Pos + (Steps * Vel);

        CalculatePosition();
        CalculateCollisions();
        CalculateForce(FrictionMag, GravityMag);
    }

    public void CalculateCollisions()
    {
        if (Pos.y <= floor)
        {
            Pos = new Vector3(Pos.x, floor, Pos.z);
            Vel = new Vector3(Vel.x, Vel.y * -DampingFactor, Vel.z);
        }

        if (Pos.y >= floor + roomHeight)
        {
            Pos = new Vector3(Pos.x, roomHeight + floor, Pos.z);
            Vel = new Vector3(Vel.x, Vel.y * -DampingFactor, Vel.z);
        }

        if (Pos.x <= wallX)
        {
            Pos = new Vector3(wallX, Pos.y, Pos.z);
            Vel = new Vector3(Vel.x * -DampingFactor, Vel.y, Vel.z);
        }

        if (Pos.x >= wallX + roomWidth)
        {
            Pos = new Vector3(wallX + roomWidth, Pos.y, Pos.z);
            Vel = new Vector3(Vel.x * -DampingFactor, Vel.y, Vel.z);
        }

        if (Pos.z <= wallZ)
        {
            Pos = new Vector3(Pos.x, Pos.y, wallZ);
            Vel = new Vector3(Vel.x, Vel.y, Vel.z * -DampingFactor);
        }

        if (Pos.z >= wallZ + roomDepth)
        {
            Pos = new Vector3(Pos.x, Pos.y, wallZ + roomDepth);
            Vel = new Vector3(Vel.x, Vel.y, Vel.z * -DampingFactor);
        }
    }

    public void CalculateForce(float FrictionMag, float GravityMag)
    {
        Vector3 Gravity = new Vector3(0, -GravityMag * Mass, 0);
        Vector3 Friction = -FrictionMag * Vel.normalized * Mass;

        if (Pos.y > floor)
        {
            Friction = Vector3.zero;
        }

        Force = Gravity + Friction;
    }

    public void CalculatePosition()
    {
        transform.position = Pos;
    }

    public void ParticleCollide(Particle otherParticle)
    {
        Vector3 direction = Pos - otherParticle.getPos();
        direction.Normalize();

        Vector3 relativeVelocity = Vel - otherParticle.getVel();

        float velocityAlongDirection = Vector3.Dot(relativeVelocity, direction);

        if (velocityAlongDirection > 0)
            return;

        float effectiveRestitution = (Restitution + otherParticle.getRestitution()) * 0.5f;

        // Magnitud del impulso
        float impulseMagnitude = -(1 + effectiveRestitution) * velocityAlongDirection / (1 / Mass + 1 / otherParticle.getMass());

        // Impulso
        Vector3 impulse = impulseMagnitude * direction;

        // Actualizar velocidades de las partículas
        Vel += impulse / Mass;
        otherParticle.setVel(otherParticle.getVel() - impulse / otherParticle.getMass());
    }

    public Vector3 getVel()
    {
        return Vel;
    }

    public Vector3 getPos()
    {
        return Pos;
    }

    public float getMass()
    {
        return Mass;
    }

    public float getRestitution()
    {
        return Restitution;
    }

    public void setVel(Vector3 Vel)
    {
        this.Vel = Vel;
    }
}
