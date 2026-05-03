using UnityEngine;
using UnityEngine.UI;
using Vuforia;

/// <summary>
/// Shows/hides a planet name label when the marker is tracked or lost.
/// Attach this to the ImageTarget GameObject.
/// Drag the planet's label Text object into the "planetLabel" field in the Inspector.
/// </summary>
public class PlanetTrackingHandler : MonoBehaviour, ITrackableEventHandler
{
    [Header("UI Reference")]
    public Text planetLabel;          // World-space Canvas Text

    [Header("Planet Info")]
    public string planetName = "Earth";

    private TrackableBehaviour mTrackableBehaviour;

    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        // Hide label at start
        if (planetLabel != null)
            planetLabel.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        bool tracked =
            newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED;

        if (planetLabel != null)
        {
            planetLabel.gameObject.SetActive(tracked);
            planetLabel.text = planetName;
        }
    }
}
