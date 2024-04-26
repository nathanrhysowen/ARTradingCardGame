using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using TMPro;


public class EnergyGemCounter : MonoBehaviour
{
    public TextMeshProUGUI energyGemCountText; // Assign in the Inspector
    private int energyGemCount = 1;

    void Start()
    {
        // Find all ObserverBehaviours in the scene
        ObserverBehaviour[] observers = FindObjectsOfType<ObserverBehaviour>();
        foreach (var observer in observers)
        {
            observer.OnTargetStatusChanged += HandleTargetStatusChanged;
        }

        UpdateGemCountDisplay();
    }

    private void OnDestroy()
    {
        // Clean up to avoid memory leaks
        ObserverBehaviour[] observers = FindObjectsOfType<ObserverBehaviour>();
        foreach (var observer in observers)
        {
            observer.OnTargetStatusChanged -= HandleTargetStatusChanged;
        }
    }

    private void HandleTargetStatusChanged(ObserverBehaviour observer, TargetStatus targetStatus)
{
    ObserverStatusInfo statusInfo = observer.GetComponent<ObserverStatusInfo>();
    if (statusInfo == null)
    {
        Debug.LogError("ObserverStatusInfo component is missing on " + observer.name);
        return;
    }

    if (targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED)
    {
        if (!statusInfo.IsTracking)
        {
            statusInfo.IsTracking = true;
            energyGemCount++;
            Debug.Log("Tracking " + observer.name);
        }
    }
    else
    {
        if (statusInfo.IsTracking)
        {
            statusInfo.IsTracking = false;
            energyGemCount--;
            Debug.Log("Lost tracking " + observer.name);
        }
    }
    UpdateGemCountDisplay();
}

public void UseGemForBackSlash()
    {
        if (CanPerformMove(1))
        {
            energyGemCount -= 1;
            Debug.Log("Used 1 gem for BackSlash, remaining: " + energyGemCount);
        }
        else
        {
            Debug.Log("Not enough gems to perform BackSlash");
        }
        UpdateGemCountDisplay();
    }


    public void UseGemsForWhirlWind()
    {
        if (CanPerformMove(2))
        {
            energyGemCount -= 2;
            Debug.Log("Used 2 gems for WhirlWind, remaining: " + energyGemCount);
        }
        else
        {
            Debug.Log("Not enough gems to perform WhirlWind");
        }
        UpdateGemCountDisplay();
    }

    private void UpdateGemCountDisplay()
    {
        energyGemCount = Mathf.Max(0, energyGemCount); // Prevent negative gem count
        energyGemCountText.text = "Energy Gems: " + energyGemCount;
    }

    public bool CanPerformMove(int requiredGems)
    {
        return energyGemCount >= requiredGems;
    }

    public void IncrementGemCount(int amount)
{
    energyGemCount += amount;
    Debug.Log("Energy Gems incremented by " + amount + ", Total Gems: " + energyGemCount);
    UpdateGemCountDisplay();
}
}

