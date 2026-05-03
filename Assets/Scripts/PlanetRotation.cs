using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public float rotationSpeed = 45f;
    public float axialTilt = 23.5f;

    void Start()
    {
        transform.localRotation = Quaternion.Euler(axialTilt, 0f, 0f);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}