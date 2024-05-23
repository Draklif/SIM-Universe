using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
    // Prefab de la explosi�n en colisi�n
    [SerializeField] private ParticleSystem explosionPrefab;

    // Constantes p�blicas
    public const float gravitationalConstant = 0.0001f;
    public const float universeTime = 0.1f;

    // Variables privadas
    private List<GameObject> planets = new List<GameObject>();
    private ParticleSystem explosion;
    private Vector3 acceleration;

    // Instancia del universo
    static Universe instance;

    void Awake()
    {
        planets.AddRange(GameObject.FindGameObjectsWithTag("Planet"));
        Debug.Log("FixedDeltaTime (" + Time.fixedDeltaTime + ") is now: " + universeTime);
        Time.fixedDeltaTime = universeTime;
    }

    void FixedUpdate()
    {
        // Update velocity
        foreach (GameObject planet in planets)
        {
            acceleration = CalculateAcceleration(planet.GetComponent<Planet>().position, planet);
            planet.GetComponent<Planet>().UpdateVelocity(acceleration, universeTime);
        }

        // Check and merge
        for (int i = 0; i < planets.Count; i++)
        {
            GameObject planetA = planets[i];
            for (int j = 0; j < planets.Count; j++)
            {
                GameObject planetB = planets[j];
                if (planetA != planetB)
                {
                    float distance = Vector3.Distance(planetA.transform.position, planetB.transform.position);
                    if (distance < (planetA.GetComponent<Planet>().radius + planetB.GetComponent<Planet>().radius) / 2)
                    {
                        MergePlanets(planetA, planetB, distance);
                    }
                }
            }
        }

        // Update position
        foreach (GameObject planet in planets)
        {
            planet.GetComponent<Planet>().UpdatePosition(universeTime);
        }
    }

    Vector3 CalculateAcceleration(Vector3 position, GameObject ignore = null)
    {
        float distance;
        Vector3 forceDirection, acceleration = Vector3.zero;
        foreach (GameObject planet in planets)
        {
            if (planet != ignore)
            {
                distance = (planet.GetComponent<Planet>().position - position).sqrMagnitude;
                forceDirection = (planet.GetComponent<Planet>().position - position).normalized;
                acceleration += forceDirection * gravitationalConstant * planet.GetComponent<Planet>().mass / distance;
            }
        }
        return acceleration;
    }

    void MergePlanets(GameObject planetA, GameObject planetB, float distance)
    {
        GameObject bigPlanet = planetA.GetComponent<Transform>().localScale.x > planetB.GetComponent<Transform>().localScale.x ? planetA : planetB;
        GameObject smallPlanet = planetA == bigPlanet ? planetB : planetA;

        bigPlanet.GetComponent<Planet>().BuildPlanet(bigPlanet.GetComponent<Planet>().radius + smallPlanet.GetComponent<Planet>().radius * 0.1f);
        Vector3 forceDirection = (bigPlanet.transform.position - smallPlanet.transform.position).normalized;
        acceleration += bigPlanet.GetComponent<Planet>().velocity + smallPlanet.GetComponent<Planet>().velocity + forceDirection * gravitationalConstant * bigPlanet.GetComponent<Planet>().mass / distance;

        bigPlanet.GetComponent<Planet>().UpdateVelocity(acceleration, universeTime);

        planets.Remove(smallPlanet);
        Destroy(smallPlanet);

        explosion = Instantiate(explosionPrefab, smallPlanet.transform.position, Quaternion.identity);
        explosion.transform.localScale = smallPlanet.transform.localScale * 4;
    }

    public static List<GameObject> Planets
    {
        get {
            return Instance.planets;
        }
    }

    static Universe Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Universe>();
            }
            return instance;
        }
    }
}
