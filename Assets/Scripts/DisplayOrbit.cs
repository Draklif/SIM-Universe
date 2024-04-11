using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DisplayOrbit : MonoBehaviour
{
    // Variables públicas
    public int numSteps = 1000;
    public float timeStep = 0.1f;
    public bool relativeToPlanet;
    public GameObject centralPlanet;

    void Start()
    {
        if (Application.isPlaying) this.enabled = false;
    }

    void Update()
    {
        DrawOrbits();
    }

    /// <summary>
    /// Dibuja las órbitas de los planetas
    /// </summary>
    void DrawOrbits()
    {
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        VirtualPlanet[] virtualPlanets = new VirtualPlanet[planets.Length];
        Vector3[][] drawPoints = new Vector3[planets.Length][];
        Vector3 referencePlanetInitialPosition = Vector3.zero;
        int referencePlanetIndex = 0;

        // Crea los planetas virtuales y llena la lista con ellos
        for (int index = 0; index < virtualPlanets.Length; index++)
        {
            virtualPlanets[index] = new VirtualPlanet(planets[index]);
            drawPoints[index] = new Vector3[numSteps];

            if (planets[index] == centralPlanet && relativeToPlanet)
            {
                referencePlanetIndex = index;
                referencePlanetInitialPosition = virtualPlanets[index].position;
            }
        }

        // Simulación con respecto al número de pasos
        for (int step = 0; step < numSteps; step++)
        {
            Vector3 referenecePlanetPosition = (relativeToPlanet) ? virtualPlanets[referencePlanetIndex].position : Vector3.zero;
            
            // Actualiza las velocidades de cada planeta
            for (int index = 0; index < virtualPlanets.Length; index++)
            {
                virtualPlanets[index].velocity += CalculateAcceleration(index, virtualPlanets) * timeStep;
            }

            // Actualiza las posiciones de cada planeta
            for (int index = 0; index < virtualPlanets.Length; index++)
            {
                Vector3 newPosition = virtualPlanets[index].position + virtualPlanets[index].velocity * timeStep;
                virtualPlanets[index].position = newPosition;
                if (relativeToPlanet)
                {
                    Vector3 referencePlanetOffset = referenecePlanetPosition - referencePlanetInitialPosition;
                    newPosition -= referencePlanetOffset;
                }
                if (relativeToPlanet && index == referencePlanetIndex)
                {
                    newPosition = referencePlanetInitialPosition;
                }

                drawPoints[index][step] = newPosition;
            }
        }

        // Dibuja las órbitas
        for (int planetIndex = 0; planetIndex < virtualPlanets.Length; planetIndex++)
        {
            Color pathColour = planets[planetIndex].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color;

            for (int step = 0; step < drawPoints[planetIndex].Length - 1; step++)
            {
                Debug.DrawLine(drawPoints[planetIndex][step], drawPoints[planetIndex][step + 1], pathColour);
            }

            var lineRenderer = planets[planetIndex].gameObject.GetComponentInChildren<LineRenderer>();
            if (lineRenderer)
            {
                lineRenderer.enabled = false;
            }

        }
    }

    /// <summary>
    /// Calcula la aceleración de los planetas para dibujar sus órbitas
    /// </summary>
    /// <param name="indexPlanet">Índice del planeta sobre el cual se calcula la aceleración</param>
    /// <param name="virtualPlanets">Lista de planetas virtuales</param>
    /// <returns></returns>
    Vector3 CalculateAcceleration(int indexPlanet, VirtualPlanet[] virtualPlanets)
    {
        float distance;
        Vector3 forceDirection, acceleration = Vector3.zero;
        for (int index = 0; index < virtualPlanets.Length; index++)
        {
            if (indexPlanet == index) { continue; }

            distance = (virtualPlanets[index].position - virtualPlanets[indexPlanet].position).sqrMagnitude;
            forceDirection = (virtualPlanets[index].position - virtualPlanets[indexPlanet].position).normalized;
            acceleration += forceDirection * Universe.gravitationalConstant * virtualPlanets[index].mass / distance;
        }
        return acceleration;
    }

    /// <summary>
    /// Copia virtual de los planetas para acceder a sus propiedades sin modificar las originales
    /// </summary>
    class VirtualPlanet
    {
        public Vector3 position;
        public Vector3 velocity;
        public float mass;

        public VirtualPlanet(GameObject planet)
        {
            position = planet.GetComponent<Transform>().position;
            velocity = planet.GetComponent<Planet>().initialVelocity;
            mass = planet.GetComponent<Planet>().mass;
        }
    }
}
