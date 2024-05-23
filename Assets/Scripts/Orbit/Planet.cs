using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    // Variables públicas
    public float radius;
    public float surfaceGravity;
    public Vector3 initialVelocity;

    // Getters y setters
    public Vector3 velocity { get; private set; }
    public Vector3 position { get; private set; }
    public float mass { get; private set; }

    void Awake()
    {
        velocity = initialVelocity;
        position = transform.position;
    }

    void OnValidate()
    {
        BuildPlanet(this.radius);
    }

    /// <summary>
    /// Actualiza la velocidad del planeta
    /// </summary>
    /// <param name="acceleration">Aceleración del planeta</param>
    /// <param name="time">Tiempo de simulación</param>
    public void UpdateVelocity(Vector3 acceleration, float time)
    {
        velocity += acceleration * time;
    }

    /// <summary>
    /// Actualiza la posición del planeta con base en la velocidad
    /// </summary>
    /// <param name="time">Tiempo de simulación</param>
    public void UpdatePosition(float time)
    {
        position += velocity * time;
        transform.position = position;
    }

    /// <summary>
    /// Establece la masa del planeta con base en el radio
    /// </summary>
    /// <param name="radius">Radio del planeta a construir</param>
    public void BuildPlanet(float radius)
    {
        mass = surfaceGravity * radius * radius / Universe.gravitationalConstant;
        transform.localScale = Vector3.one * radius;
    }
}
