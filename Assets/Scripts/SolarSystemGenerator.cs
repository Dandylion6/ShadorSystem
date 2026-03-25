using UnityEngine;

public class SolarSystemGenerator : MonoBehaviour
{
    [Header("System Generation Settings")]
    [SerializeField] private int startingMass = 100;
    [SerializeField] private Star starPrefab = null;


    void Start()
    {
        GenerateStar();
    }


    private void GenerateStar()
    {
        float fraction = Random.Range(0.6f, 0.8f);
        int stellarMass = Mathf.RoundToInt(startingMass * fraction);
        startingMass -= stellarMass;

        Star star = Instantiate(starPrefab, transform);
        star.Generate(stellarMass);
    }
}
