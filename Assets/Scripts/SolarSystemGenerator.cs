using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolarSystemGenerator : MonoBehaviour
{
    private const float SOLAR_TO_EARTH_MASS = 333000.0f; // How many earths fit into the sun mass-wise.


    [Header("System Generation Settings")]
    [SerializeField] [Range(0.5f, 100.0f)] private float startingMass = 1.0f;
    [SerializeField] private Star starPrefab = null;
    [SerializeField] private Planet planetPrefab = null;
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int seed = 0;

    [Header("Planet Generation Settings")]
    [SerializeField] [Range(0.01f, 1.0f)] private float minimumPlanetMass = 0.1f;
    [SerializeField] [Range(1, 12)] private int minPlanets = 1;
    [SerializeField] [Range(1, 12)] private int maxPlanets = 8;

    private System.Random random = null;
    private int planetCount = 0;


    void Start()
    {
        if (useRandomSeed) seed = System.Environment.TickCount;
        random = new System.Random(seed);

        Star star = GenerateStar();
        GeneratePlanets(star);
    }


    private Star GenerateStar()
    {
        float randomFraction = Mathf.Lerp(0.998f, 0.9994f, (float)random.NextDouble());
        float stellarMass = startingMass * randomFraction;

        Star star = Instantiate(starPrefab, transform);
        star.Initialize(stellarMass, random, Vector3.right, true, 0.0f);
        star.Generate();

        return star;
    }


    private void GeneratePlanets(Star star)
    {
        float diskBudget = (startingMass - star.Energy) * SOLAR_TO_EARTH_MASS; // Simulates the disk of mass left from the formation of the star.

        float orbitalDistance = Mathf.Lerp(0.7f,  0.9f, (float)random.NextDouble());
        orbitalDistance += star.BodySize / CelestialBody.ORBITAL_DISTANCE_UNIT;

        planetCount = random.Next(maxPlanets) + 1;
        List<float> masses = GeneratePlanetaryMasses(diskBudget);
        planetCount = masses.Count;


        foreach(float mass in masses)
        {
            Planet planet = GeneratePlanet(mass, orbitalDistance);
            ++planetCount;

            orbitalDistance = GetOrbitalDistance(orbitalDistance);
            orbitalDistance += planet.BodySize / CelestialBody.ORBITAL_DISTANCE_UNIT;
        }
    }


    private List<float> GeneratePlanetaryMasses(float diskBudget)
    {
        List<float> masses = new();
        int targetPlanetCount = Mathf.RoundToInt(Mathf.Lerp(minPlanets, maxPlanets, (float)random.NextDouble()));

        for (int i = 0; i < targetPlanetCount; ++i)
        {
            masses.Add(Mathf.Lerp(0.1f, 1.0f, (float)random.NextDouble()));
        }

        float total = masses.Sum();
        for (int i = 0; i < targetPlanetCount; ++i)
        {
            float mass = masses[i] / total * diskBudget;
            if (mass < minimumPlanetMass)
            {
                masses.RemoveAt(i);
                --i;
                continue;
            }
            masses[i] = mass;
        }

        return masses;
    }


    private Planet GeneratePlanet(float planetaryMass, float orbitalDistance)
    {
        Planet planet = Instantiate(planetPrefab, transform);
        planet.Initialize(planetaryMass, random, Vector3.up, true, orbitalDistance);
        planet.Generate();

        return planet;
    }


    private float GetOrbitalDistance(float lastOrbitalDistance)
    {
        float scaler = Mathf.Lerp(1.3f, 1.7f, (float)random.NextDouble());
        return lastOrbitalDistance * scaler;
    }
}
