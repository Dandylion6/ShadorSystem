using UnityEngine;

[RequireComponent(typeof(SphereCreator))]
public class Star : MonoBehaviour
{
    private SphereCreator creator = null;


    private void Awake()
    {
        creator = GetComponent<SphereCreator>();
    }


    public void Generate(int stellarMass)
    {
        creator.GenerateSphere(stellarMass);
    }
}
