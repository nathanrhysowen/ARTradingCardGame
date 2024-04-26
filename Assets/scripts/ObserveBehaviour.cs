using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class EnergyGemStatus : MonoBehaviour
{
    // Public variable to check if this target is currently being tracked
    public bool IsTracking { get; private set; }

    private ObserverBehaviour observerBehaviour;

    private void Awake()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
        else
        {
            Debug.LogError("ObserverBehaviour component missing from this GameObject.", this);
        }
    }

    private void OnDestroy()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour observer, TargetStatus targetStatus)
    {
        // Update tracking status based on whether the target is TRACKED, EXTENDED_TRACKED
        bool currentlyTracked = targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED;
        if (currentlyTracked && !IsTracking)
        {
            IsTracking = true;
            Debug.Log("Started tracking: " + observer.TargetName);
            // Additional actions when tracking starts can be added here
        }
        else if (!currentlyTracked && IsTracking)
        {
            IsTracking = false;
            Debug.Log("Stopped tracking: " + observer.TargetName);
            // Additional actions when tracking stops can be added here
        }
    }
}
