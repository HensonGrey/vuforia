using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class PlanetTrackingHandler : MonoBehaviour, ITrackableEventHandler
{
    public Text planetLabel;
    public string planetName = "Earth";

    private TrackableBehaviour mTrackableBehaviour;

    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

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
            planetLabel.gameObject.SetActive(tracked);
    }
}