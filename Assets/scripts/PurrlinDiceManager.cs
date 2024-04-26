using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using TMPro;


public class PurrlinDiceRoller : MonoBehaviour
{
    public float rollForce = 0.1f; // Adjust the force of the roll
    public float rollDuration = 2.0f; // Duration of the roll
    public TextMeshProUGUI resultText; // Reference to the TextMeshPro UI element
    private Rigidbody rb;
    private Vector3 originalScale = new Vector3(0.008544052f, 0.008544052f, 0.008544052f); // Set the visible scale for when dice is active
    private int result; // store the roll result

    // Public getter to access the result
    public int Result
    {
        get { return result; }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on the dice!");
            return;
        }

        // Set the dice to be invisible initially
        transform.localScale = Vector3.zero;
    }

    public void PurrlinRollDice()
    {
        // Scale up to make dice visible
        transform.localScale = originalScale;
        // Apply a random torque to roll the dice
        rb.AddTorque(new Vector3(Random.Range(-rollForce, rollForce), Random.Range(-rollForce, rollForce), Random.Range(-rollForce, rollForce)), ForceMode.Impulse);
        StartCoroutine(PurrlinStopRollingAfterDuration());
    }

    private IEnumerator PurrlinStopRollingAfterDuration()
    {
        yield return new WaitForSeconds(rollDuration);

        // Stop the dice from rolling
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Generate a random number between 1 and 6
        result = Random.Range(1, 7);

        // Display the result
        resultText.text = result.ToString();

        // Wait for one second before hiding the result text and dice
        yield return new WaitForSeconds(1);
        resultText.text = "";
        transform.localScale = Vector3.zero; // Scale down to hide the dice
    }
}