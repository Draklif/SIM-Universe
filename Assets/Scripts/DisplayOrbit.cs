using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DisplayOrbit : MonoBehaviour
{
    public int numSteps = 1000;
    public float timeStep = 0.1f;

    public bool relativeToBody;
    public GameObject centralPlanet;

    void Start()
    {
        if (Application.isPlaying)
        {
            this.enabled = false;
        }
    }

    void Update()
    {
        DrawOrbits();
    }

    void DrawOrbits()
    {
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        var virtualPlanets = new VirtualPlanet[planets.Length];
        var drawPoints = new Vector3[planets.Length][];
        int referenceFrameIndex = 0;
        Vector3 referenceBodyInitialPosition = Vector3.zero;

        // Crea los planetas virtuales
        for (int i = 0; i < virtualPlanets.Length; i++)
        {
            virtualPlanets[i] = new VirtualPlanet(planets[i]);
            drawPoints[i] = new Vector3[numSteps];

            if (planets[i] == centralPlanet && relativeToBody)
            {
                referenceFrameIndex = i;
                referenceBodyInitialPosition = virtualPlanets[i].position;
            }
        }

        // Simulate
        for (int step = 0; step < numSteps; step++)
        {
            Vector3 referenceBodyPosition = (relativeToBody) ? virtualPlanets[referenceFrameIndex].position : Vector3.zero;
            
            // Update velocities
            for (int i = 0; i < virtualPlanets.Length; i++)
            {
                virtualPlanets[i].velocity += CalculateAcceleration(i, virtualPlanets) * timeStep;
            }

            // Update positions
            for (int i = 0; i < virtualPlanets.Length; i++)
            {
                Vector3 newPos = virtualPlanets[i].position + virtualPlanets[i].velocity * timeStep;
                virtualPlanets[i].position = newPos;
                if (relativeToBody)
                {
                    var referenceFrameOffset = referenceBodyPosition - referenceBodyInitialPosition;
                    newPos -= referenceFrameOffset;
                }
                if (relativeToBody && i == referenceFrameIndex)
                {
                    newPos = referenceBodyInitialPosition;
                }

                drawPoints[i][step] = newPos;
            }
        }

        // Draw paths
        for (int bodyIndex = 0; bodyIndex < virtualPlanets.Length; bodyIndex++)
        {
            var pathColour = planets[bodyIndex].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color;

            for (int i = 0; i < drawPoints[bodyIndex].Length - 1; i++)
            {
                Debug.DrawLine(drawPoints[bodyIndex][i], drawPoints[bodyIndex][i + 1], pathColour);
            }

            var lineRenderer = planets[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
            if (lineRenderer)
            {
                lineRenderer.enabled = false;
            }

        }
    }

    Vector3 CalculateAcceleration(int i, VirtualPlanet[] virtualPlanets)
    {
        float distance;
        Vector3 forceDirection, acceleration = Vector3.zero;
        for (int j = 0; j < virtualPlanets.Length; j++)
        {
            if (i == j) { continue; }

            distance = (virtualPlanets[j].position - virtualPlanets[i].position).sqrMagnitude;
            forceDirection = (virtualPlanets[j].position - virtualPlanets[i].position).normalized;
            acceleration += forceDirection * Universe.gravitationalConstant * virtualPlanets[j].mass / distance;
        }
        return acceleration;
    }

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
