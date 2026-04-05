using UnityEngine;

public class SolarSystemGenerator : MonoBehaviour
{
    [Header("System Generation Settings")]
    [SerializeField] [Range(0.5f, 100.0f)] private float startingMass = 1.0f;
    [SerializeField] private Star starPrefab = null;
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int seed = 0;

    private System.Random random = null;
    private float currentMass = 0;


    void Start()
    {
        if (useRandomSeed) seed = System.Environment.TickCount;
        random = new System.Random(seed);

        currentMass = startingMass;
        GenerateStar();
    }


    private void GenerateStar()
    {
        float randomFraction = (1.0f + (float)random.NextDouble()) * 0.5f;
        randomFraction = Mathf.Lerp(0.7f, 0.8f, randomFraction);

        float stellarMass = startingMass * randomFraction;
        currentMass -= stellarMass;

        Star star = Instantiate(starPrefab, transform);
        star.Generate(stellarMass, random);
    }
}
