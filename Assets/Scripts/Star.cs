using UnityEngine;

[RequireComponent(typeof(SphereCreator))]
public class Star : CelestialBody
{
    private const float MASS_CONSTANT = 1.0f / 100.0f;

    [SerializeField] private Texture2D emissionTexture = null;
    [SerializeField] private Gradient temperatureColor = new();
    [SerializeField] private Vector2 temperatureRange = new();
    [SerializeField] private Vector2 sizeRange = new();
    [Space]
    public float starTemperature = 0.0f;
    public float stellarMass = 0.0f;
    private SphereCreator creator = null;
    private System.Random random = null;


    private void Awake()
    {
        creator = GetComponent<SphereCreator>();
        emissionTexture = new(DEFAULT_BODY_TEXTURE_SIZE, DEFAULT_BODY_TEXTURE_SIZE);
    }


    public void Generate(float stellarMass, System.Random random)
    {
        this.random = random;
        this.stellarMass = stellarMass;

        float normalizedMass = Mathf.Pow(stellarMass * MASS_CONSTANT, 0.4f);
        int starSize = Mathf.RoundToInt(Mathf.Lerp(sizeRange.x, sizeRange.y, normalizedMass));

        GenerateEmissionNoise(starSize);
        creator.GenerateSphere(starSize);

        Material starMaterial = creator.Renderer.material;
        starMaterial.SetTexture("_EmissionMap", emissionTexture);

        starTemperature = Mathf.Lerp(temperatureRange.x, temperatureRange.y, normalizedMass);
        Color baseColor = temperatureColor.Evaluate(normalizedMass);

        starMaterial.SetColor("_BaseColor", baseColor);

        SetRotationalSpeed(8.0f);
    }


    public void GenerateEmissionNoise(int starSize)
    {
        Vector2 noiseOffset = Vector2.zero;
        noiseOffset.x = (float)random.NextDouble() * 10000.0f;

        int pixelCount = emissionTexture.width * emissionTexture.height;
        for (int i = 0; i < pixelCount; ++i)
        {
            int x = i % emissionTexture.width;
            int y = i / emissionTexture.width;

            float normalizedX = (float)x / emissionTexture.width;
            float normalizedY = (float)y / emissionTexture.height;

            Vector2 noisePosition = new Vector2(normalizedX, normalizedY) * starSize + noiseOffset;
            float noiseValue = Mathf.PerlinNoise(noisePosition.x, noisePosition.y);
            Color color = Color.white * noiseValue;

            color.a = 1.0f;
            emissionTexture.SetPixel(x, y, color);
        }

        emissionTexture.Apply();
    }
}
