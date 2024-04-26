using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public Button[] knightButtons;
    public Button[] purrlinButtons;
    private bool isKnightTurn = true;  // Start with knight's turn by default
    public EnergyGemCounter energyGemCounter;
    public PurrlinEnergyGemCounter purrlinenergygemcounter;
    

    void Start()
    {
        UpdateUI();
        // Ensure there's a reference to the EnergyGemCounter
        if (energyGemCounter == null)
        {
            Debug.LogError("EnergyGemCounter script is not assigned!");
        }
    }

    // Call this method to switch turns
    public void SwitchTurn()
    {
        isKnightTurn = !isKnightTurn;
        energyGemCounter.IncrementGemCount(1); 
        UpdateUI();
    }

    // Update the UI based on whose turn it is
    void UpdateUI()
    {
        // Enable all knight buttons and disable all purrlin buttons if it's the knight's turn
        foreach (var button in knightButtons)
        {
            button.interactable = isKnightTurn;
        }

        // Enable all purrlin buttons and disable all knight buttons if it's the purrlin's turn
        foreach (var button in purrlinButtons)
        {
            button.interactable = !isKnightTurn;
        }
    }

    // Optionally, you can add methods that are triggered when buttons are pressed
    public void OnKnightActionComplete()
    {
        // Call this when the knight finishes his move
        SwitchTurn();
    }

    public void OnPurrlinActionComplete()
    {
        // Call this when the purrlin finishes her move
        SwitchTurn();
    }
}
