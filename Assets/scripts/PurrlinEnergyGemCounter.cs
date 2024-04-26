using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using TMPro;

public class PurrlinEnergyGemCounter : MonoBehaviour
{
    public TextMeshProUGUI energyGemCountText;
    private int energyGemCount = 1;
    public Transform characterTarget;  // Assign the character's target transform in the inspector
    public float proximityThreshold = 0.1f;  // Distance threshold to add gems

    void Start()
    {
        ObserverBehaviour[] observers = FindObjectsOfType<ObserverBehaviour>();
        foreach (var observer in observers)
        {
            observer.OnTargetStatusChanged += HandleTargetStatusChanged;
        }
        UpdateGemCountDisplay();
    }

    void OnDestroy()
    {
        ObserverBehaviour[] observers = FindObjectsOfType<ObserverBehaviour>();
        foreach (var observer in observers)
        {
            observer.OnTargetStatusChanged -= HandleTargetStatusChanged;
        }
    }

    void HandleTargetStatusChanged(ObserverBehaviour observer, TargetStatus targetStatus)
    {
        ObserverStatusInfo statusInfo = observer.GetComponent<ObserverStatusInfo>();
        if (statusInfo == null)
        {
            Debug.LogError("ObserverStatusInfo component is missing on " + observer.name);
            return;
        }

        // Calculate the distance between the observer and the character's target
        float distance = Vector3.Distance(observer.transform.position, characterTarget.position);
        Debug.Log($"Distance to {observer.name}: {distance}, Threshold: {proximityThreshold}");

        if (distance <= proximityThreshold)
        {
            Debug.Log($"Within proximity: {observer.name}, Status: {targetStatus.Status}");
            if (targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED)
            {
                if (!statusInfo.IsTracking)
                {
                    statusInfo.IsTracking = true;
                    energyGemCount++;
                    Debug.Log($"Energy gem added for {observer.name}. Total now: {energyGemCount}");
                }
            }
            else
            {
                if (statusInfo.IsTracking)
                {
                    statusInfo.IsTracking = false;
                    energyGemCount--;
                    Debug.Log($"Energy gem removed for {observer.name}. Total now: {energyGemCount}");
                }
            }
            UpdateGemCountDisplay();
        }
    }

    public bool CanPerformMove(int requiredGems)
    {
        return energyGemCount >= requiredGems;
    }

    public void DeductGemsForBite()
    {
        if (CanPerformMove(1))
        {
            energyGemCount -= 1;
            Debug.Log("Used 1 gem for Bite, remaining: " + energyGemCount);
            UpdateGemCountDisplay();
        }
        else
        {
            Debug.Log("Not enough gems to perform Bite");
        }
    }

    public void DeductGemsForFlamingHairball()
    {
        if (CanPerformMove(2))
        {
            energyGemCount -= 2;
            Debug.Log("Used 2 gems for Flaming Hairball, remaining: " + energyGemCount);
            UpdateGemCountDisplay();
        }
        else
        {
            Debug.Log("Not enough gems to perform Flaming Hairball");
        }
    }

    void UpdateGemCountDisplay()
    {
        energyGemCountText.text = "Energy Gems: " + energyGemCount;
    }

    public void IncrementGemCount(int amount)
{
    energyGemCount += amount;
    Debug.Log("Energy Gems incremented by " + amount + ", Total Gems: " + energyGemCount);
    UpdateGemCountDisplay();
}
}


