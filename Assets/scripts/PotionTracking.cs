using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class PotionTracking : MonoBehaviour
{
    // Reference to the HealthManager script on the player or character
    public HealthManager healthManager;
    public ParticleSystem explosionEffect;


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
        // Check if the target is TRACKED or EXTENDED_TRACKED
        bool isCurrentlyTracked = targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED;
        
        if (isCurrentlyTracked)
        {
            // When the potion is tracked, restore health
            if (healthManager != null)
            {
                healthManager.RestoreHealth(50);  // Adjust the health restoration value as necessary
                Debug.Log("Health Potion used, health restored by 50.");
            }
        }
    }

     IEnumerator TriggerEffects()
    {
        // Play the explosion effect
        explosionEffect.Play();
        yield return new WaitForSeconds(1); // Wait for 1 second after the explosion starts

        // Then restore health
        if (healthManager != null)
        {
            healthManager.RestoreHealth(50);
            Debug.Log("Health Potion used, health restored by 50.");
        }
    }

    public void OnTargetFound() {
    explosionEffect.Play();
    }
}
