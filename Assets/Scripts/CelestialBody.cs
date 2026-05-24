using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    // For most celestial bodies, I'd want them to be able to be viewed up close, so I keep 2k as a default.
    protected const int DEFAULT_BODY_TEXTURE_SIZE = 2048;
    // Smaller bodies that are less interesting, like the possibility of moons or asteroids, can get away with smaller texture.
    protected const int SMALL_BODY_TEXTURE_SIZE = 512;

    private float rotationSpeed = 1.0f;
    private Vector3 northPole = Vector3.up;

    
    public void SetRotationalSpeed(float rotationSpeed) => this.rotationSpeed = rotationSpeed;
    public void SetNorthPole(Vector3 poleDirection) => northPole = poleDirection.normalized;


    void Update()
    {
        transform.Rotate(northPole, rotationSpeed * Time.deltaTime);
    }
}
