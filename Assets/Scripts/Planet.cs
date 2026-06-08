using UnityEngine;

public class Planet : CelestialBody
{
    private const float MASS_CONSTANT = 1.0f / 100.0f;


    [SerializeField] private Vector2 sizeRange = new();


    public override void Generate()
    {
        
    }


    protected override BodyData CreateData()
    {
        BodyData data = new()
        {
            bodySize = Mathf.Lerp(sizeRange.x, sizeRange.y, Energy * MASS_CONSTANT),
            orbitSpeed = 5.0f,
            northPole = Vector3.up
        };

        return data;
    }
}
