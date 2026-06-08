using UnityEngine;

[RequireComponent(typeof(SphereCreator))]
abstract public class CelestialBody : MonoBehaviour
{
    public const float ORBITAL_DISTANCE_UNIT = 30.0f; // A fake unit for representing orbital distance.

    // For most celestial bodies, I'd want them to be able to be viewed up close, so I keep 2k as a default.
    protected const int DEFAULT_BODY_TEXTURE_SIZE = 2048;
    // Smaller bodies that are less interesting, like the possibility of moons or asteroids, can get away with smaller texture.
    protected const int SMALL_BODY_TEXTURE_SIZE = 512;


    protected struct BodyData
    {
        public Vector3 northPole;
        public float rotationSpeed;
        public float orbitSpeed;
        public float bodySize;
    }
    
    
    public Vector3 SolarPlaneUp => solarPlaneUp;
    public float Energy => energy;
    public float OrbitalDistance => orbitalDistance;
    public float BodySize => data.bodySize;
    public bool IsClockwiseSolarRotation => isClockwiseSolarRotation;
    
    protected SphereCreator Sphere => sphereCreator;
    protected System.Random Random => random;
    protected float OrbitFrequency => data.orbitSpeed / (2.0f * Mathf.PI * orbitalDistance);

    private SphereCreator sphereCreator = null;
    private System.Random random = null;
    private BodyData data;
    private Vector3 solarPlaneUp = Vector3.up; // The 'disk' which solar systems form with.
    private Vector3 orbitalForward = Vector3.forward;
    private Vector3 orbitalRight = Vector3.right;
    private float energy = 0.0f;
    private float orbitalDistance = 0.0f;
    private float orbitPhase = 0.0f;
    private bool isClockwiseSolarRotation = true; // The rotational direction celestial bodies move in. (Ours rotations counter-clockwise when looking at the sun's north pole).

    public virtual void Initialize(float energy, System.Random random, Vector3 solarPlaneUp, bool isClockwiseSolarRotation, float orbitalDistance)
    {
        sphereCreator = GetComponent<SphereCreator>();

        this.random = random;
        this.solarPlaneUp = solarPlaneUp;
        this.energy = energy;
        this.orbitalDistance = orbitalDistance;
        this.isClockwiseSolarRotation = isClockwiseSolarRotation;
        data = CreateData();

        float fidelitySize = Mathf.Round(data.bodySize);
        sphereCreator.GenerateSphere((int)fidelitySize);

        float bodyScale = data.bodySize / fidelitySize;
        transform.localScale = Vector3.one * bodyScale; // Makes sure the scale is correct while having the best fidelity.

        orbitPhase = (float)random.NextDouble() * Mathf.PI;

        Vector3 orbitRandom = Vector3.zero;
        orbitRandom.x += ((float)random.NextDouble() - 0.5f) * 2.0f;
        orbitRandom.y += ((float)random.NextDouble() - 0.5f) * 2.0f;
        orbitRandom.z += ((float)random.NextDouble() - 0.5f) * 2.0f;

        Vector3 orbitalUp = (solarPlaneUp + orbitRandom * 0.1f).normalized;
        Vector3 helperDirection = Mathf.Abs(orbitalUp.y) < 0.9999f ? Vector3.up : Vector3.right;

        orbitalForward = Vector3.Normalize(Vector3.Cross(helperDirection, orbitalUp));
        orbitalRight = Vector3.Cross(orbitalUp, orbitalForward);
    }


    public abstract void Generate();

    protected abstract BodyData CreateData();


    private void Update()
    {
        if (orbitalDistance > float.Epsilon) UpdateOrbit(); // Not a star.

        float rotationSpeed = data.rotationSpeed * (isClockwiseSolarRotation ? 1.0f : -1.0f);
        transform.Rotate(data.northPole, rotationSpeed * Time.deltaTime);
    }


    private void UpdateOrbit()
    {
        Vector3 orbitalPosition = Vector3.zero;
        orbitalPosition += orbitalRight * Mathf.Sin((Time.time - orbitPhase) * OrbitFrequency);
        orbitalPosition += orbitalForward * Mathf.Cos((Time.time - orbitPhase) * OrbitFrequency);
        orbitalPosition *= ORBITAL_DISTANCE_UNIT * orbitalDistance;

        transform.position = orbitalPosition;

    }
}
