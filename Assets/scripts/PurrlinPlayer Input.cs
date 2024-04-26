using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PurrlinAnimationTrigger : MonoBehaviour
{
    public Animator animator;
    public Button biteButton;
    public Button flamingHairballButton;
    public Transform targetCharacter; // The target character's position
    private Vector3 originalPosition; // To store the character's original position
    public GameObject fireballPrefab; // Assign this in the Inspector
    public HealthManager targetHealthManager;
    public TurnManager turnManager;
    public PurrlinDiceRoller diceRoller;

    void Start()
    {
        biteButton.onClick.AddListener(StartBiteAnimation);
        flamingHairballButton.onClick.AddListener(StartFlamingHairballAnimation);
        originalPosition = transform.position; // Store original position at start
    }

    public void StartBiteAnimation()
    {
        diceRoller.PurrlinRollDice();
        Debug.Log("Attempting to start Bite Animation.");
        if (targetCharacter != null)
        {
            Vector3 originalPosition = transform.position; // Store original position at start of animation
            StartCoroutine(MoveAndPlayBite(targetCharacter.position, originalPosition));
            
            if (GetComponent<PurrlinEnergyGemCounter>() != null)
             {
               GetComponent<PurrlinEnergyGemCounter>().DeductGemsForBite();
             }
        }
        else
        {
            Debug.LogError("Target character is not assigned.");
        }
    }

    

    private IEnumerator MoveAndPlayBite(Vector3 target, Vector3 originalPosition)
    {
        float walkDuration = GetAnimationLength("Cat_walk");
        Debug.Log("Walk duration fetched: " + walkDuration);

        animator.SetTrigger("WalkTrigger");
        Debug.Log("WalkTrigger set.");

        Vector3 startPosition = transform.position;
        Vector3 directionToTarget = (target - transform.position).normalized;
        Vector3 modifiedTarget = target - directionToTarget * 0.08f; // Slightly before the target to avoid overshooting
        float time = 0;

        while (time < walkDuration)
        {
            transform.position = Vector3.Lerp(startPosition, modifiedTarget, time / walkDuration);
            time += Time.deltaTime;
            Debug.Log($"Moving to target: Current Position = {transform.position}");
            yield return null;
        }

        transform.position = modifiedTarget;
        Debug.Log($"Reached target, starting Bite animation at {transform.position}");
        animator.ResetTrigger("WalkTrigger");
        animator.SetTrigger("BiteTrigger");

        // Wait for the Bite animation to complete
        yield return new WaitForSeconds(GetAnimationLength("Cat_Bite"));

        // If the target is in range and the HealthManager component exists, apply damage
    if (targetHealthManager != null)
{
    // Check the dice result and apply damage accordingly
    int damage = diceRoller.Result >= 3 ? 20 : 10;
    targetHealthManager.TakeDamage(damage);  // Apply conditional damage based on dice roll
    Debug.Log($"Damage applied to target: -{damage} Health");
}

        // Ensure the model is visible and in the correct position after the animation
        transform.position = originalPosition; // Reset to original position after animation
        Debug.Log($"Animation complete. Reset position to {originalPosition} and set to idle.");
        animator.SetTrigger("idle");
        

     turnManager.SwitchTurn();

        

        
}

    public void StartFlamingHairballAnimation()
    {
        diceRoller.PurrlinRollDice();
        animator.SetTrigger("FlamingHairballTrigger");
        
        if (GetComponent<PurrlinEnergyGemCounter>() != null)
        {
          GetComponent<PurrlinEnergyGemCounter>().DeductGemsForFlamingHairball();
        }

        SpawnFireball();
        StartCoroutine(ResetAnimationStateAfterDelay("FlamingHairballTrigger", GetAnimationLength("FlamingHairballTrigger")));
        turnManager.SwitchTurn();
    }

    private void SpawnFireball()
{
    GameObject fireballInstance = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
    Fireball fireballScript = fireballInstance.GetComponent<Fireball>();
    if (fireballScript != null)
    {
        fireballScript.target = targetCharacter; // Set the target of the fireball
        fireballScript.Activate(); // Now activate the fireball to start moving
    }
    else
    {
        Debug.LogError("Fireball script not found on the instantiated fireball prefab!");
    }
}



    private IEnumerator ResetAnimationStateAfterDelay(string triggerName, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger("idle");
    }

    private float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name.Equals(animationName))
            {
                return clip.length;
            }
        }
        return 0;
    }
}