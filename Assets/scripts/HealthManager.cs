using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Slider healthSlider;  // Assign this in the inspector to your health slider
    public float maxHealth = 100f;
    private float currentHealth;

    void Awake()
    {
        currentHealth = 100f;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " has died.");
            // Handle death here (e.g., disable the character, show a death animation, etc.)
        }
    }

    public void RestoreHealth(float amount)
{
    currentHealth += amount;
    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // Ensure health does not exceed max health
    healthSlider.value = currentHealth;  // Update the health slider

    Debug.Log("Health restored. Current health: " + currentHealth);
}
}
