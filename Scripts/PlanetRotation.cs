using UnityEngine;

/// <summary>
/// Rotates a planet around its own axis and optionally tilts it.
/// Attach this to each planet sphere GameObject.
/// </summary>
public class PlanetRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Degrees per second around Y axis")]
    public float rotationSpeed = 45f;

    [Tooltip("Axial tilt in degrees (Earth=23.5, Mars=25, Saturn=26.7)")]
    public float axialTilt = 23.5f;

    void Start()
    {
        // Apply axial tilt on startup
        transform.localRotation = Quaternion.Euler(axialTilt, 0f, 0f);
    }

    void Update()
    {
        // Rotate around the planet's own tilted up axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
