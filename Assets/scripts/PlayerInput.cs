using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class AnimationTrigger : MonoBehaviour
{
    public Animator animator;
    public Transform targetCharacter;
    public EnergyGemCounter gemCounter;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    public Button Whirlwindbutton;
    public Button Backslashbutton;
    public HealthManager targetHealthManager;
    public Collider attackHitbox;
    public float damageCooldown = 3f;
    private float lastDamageTime;
    private Image buttonImage;
    private Image backslashbuttonImage;
    public TurnManager turnManager;
    public DiceRoller diceRoller;

    private void Start()
    {
        buttonImage = Whirlwindbutton.GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("Failed to get the Image component on the WhirlWind button.");
        }
        Whirlwindbutton.onClick.AddListener(CheckAndPerformWhirlWind);

        backslashbuttonImage = Backslashbutton.GetComponent<Image>();
        if (backslashbuttonImage == null)
        {
            Debug.LogError("Failed to get the Image component on the Back Slash Button.");
        }
        Backslashbutton.onClick.AddListener(CheckAndPerformBackslash);
        gemCounter.UseGemForBackSlash();

        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
        }
        EnableAttackHitbox();

        
    }

    private void CheckAndPerformWhirlWind()
    {
        if (gemCounter.CanPerformMove(2))
        {
            gemCounter.UseGemsForWhirlWind();
            StartWhirlWindAnimation();
            buttonImage.color = Color.white;
        }
        else
        {
            buttonImage.color = new Color(1f, 0.5f, 0.5f, 1f);
            StartCoroutine(FadeButtonColorBack());
        }
    }

    private void CheckAndPerformBackslash()
    {
        if (gemCounter.CanPerformMove(1))
        {
            gemCounter.UseGemForBackSlash();
            StartBackSlashAnimation();
            backslashbuttonImage.color = Color.white;
        }
        else
        {
            backslashbuttonImage.color = new Color(1f, 0.5f, 0.5f, 1f);
            StartCoroutine(FadeButtonColorBack());
        }
    }

    IEnumerator FadeButtonColorBack()
    {
        float duration = 1.0f;
        float currentTime = 0;
        Color startColor = buttonImage.color;
        Color endColor = Color.white;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            buttonImage.color = Color.Lerp(startColor, endColor, currentTime / duration);
            backslashbuttonImage.color = Color.Lerp(startColor, endColor, currentTime / duration);
            yield return null;
        }

        buttonImage.color = endColor;
        backslashbuttonImage.color = endColor;
    }

    public void StartBackSlashAnimation()
    {
        
        if (gemCounter.CanPerformMove(1))
        {
            diceRoller.RollDice();
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            EnableAttackHitbox();
            StartCoroutine(MoveAndPlayBackSlash(targetCharacter.position, 3));
        }
    }

   public void StartWhirlWindAnimation()
{
    
    if (gemCounter.CanPerformMove(2))
    {
        diceRoller.RollDice();
        originalPosition = transform.position;  // Store original position before the animation
        originalRotation = transform.rotation;  // Store the original rotation
        EnableAttackHitbox();  // Enable any attack hitboxes relevant to this animation
        animator.SetTrigger("WhirlWindTrigger");  // Trigger the whirlwind animation
        StartCoroutine(MoveTowardsTargetAndReset(targetCharacter.position, 2));  // Move to target and reset
        turnManager.SwitchTurn();
    }
    else
    {
        Debug.Log("Not enough energy gems for WhirlWind.");
    }
}
private IEnumerator MoveTowardsTargetAndReset(Vector3 target, float duration)
{
    Vector3 startPosition = transform.position;  // Starting position
    Quaternion startRotation = Quaternion.Euler(0, 180, 0);  // Initial rotation for the movement
    transform.rotation = startRotation;  // Apply the initial rotation

    float time = 0;
    while (time < duration)
    {
        transform.position = Vector3.Lerp(startPosition, target, time / duration);  // Lerp towards the target
        time += Time.deltaTime;
        yield return null;
    }

    // Wait for the animation to complete
    yield return new WaitForSeconds(GetAnimationLength("WhirlWindTrigger") + 1.0f);  // 1.0f buffer to ensure animation completeness

    // If the target has a HealthManager component, reduce health
    HealthManager healthManager = targetCharacter.GetComponent<HealthManager>();
if (healthManager != null)
{
    int damage = diceRoller.Result >= 5 ? 50 : 25;  // Use the dice result to determine damage
    healthManager.TakeDamage(damage);
    Debug.Log($"Damage applied: -{damage} Health to the target");
}
else
{
    Debug.LogError("HealthManager component not found on the target character");
}

    // Reset position and rotation after the animation
    transform.position = originalPosition;
    transform.rotation = Quaternion.Euler(0, 0, 0);  // Reset rotation to neutral
    animator.SetTrigger("idle");  // Set to idle state
    
}


private IEnumerator ResetPositionAfterAnimation(string trigger, float delay)
{
    yield return new WaitForSeconds(delay);  // Delay to ensure the animation has completed
    transform.position = originalPosition;  // Reset position to the original
    transform.rotation = Quaternion.Euler(0, 0, 0);  // Reset rotation to ensure it's aligned
    animator.SetTrigger("idle");  // Return to idle state
}

    private IEnumerator MoveAndPlayBackSlash(Vector3 target, float walkDuration)
{
    // Apply the rotation needed for the walk animation
    transform.rotation = Quaternion.Euler(0, 180, 0);
    animator.SetTrigger("WalkTrigger");

    Vector3 startPosition = transform.position;
    Vector3 directionToTarget = (target - transform.position).normalized;
    Vector3 modifiedTarget = target - directionToTarget * 0.08f;
    float time = 0;
    float speedFactor = 0.5f;

    while (time < walkDuration)
    {
        transform.position = Vector3.Lerp(startPosition, modifiedTarget, time / walkDuration);
        time += Time.deltaTime * speedFactor;
        yield return null;
    }

    // Animation starts
    transform.position = modifiedTarget;
    animator.ResetTrigger("WalkTrigger");
    animator.SetTrigger("BackSlashTrigger");

    // Wait for animation to completely finish, add a small buffer (e.g., 0.5 seconds) to ensure all transitions are complete
    yield return new WaitForSeconds(GetAnimationLength("Armature_backslash") + 0.8f);

   HealthManager healthManager = targetCharacter.GetComponent<HealthManager>();
if (healthManager != null)
{
    int damage = diceRoller.Result >= 3 ? 18 : 15;  // Use the dice result to determine damage
    healthManager.TakeDamage(damage);
    Debug.Log($"Damage applied: -{damage} Health to the target");
}
else
{
    Debug.LogError("HealthManager component not found on the target character");
}


    // Reset position to the original and set rotation to zero
    transform.position = startPosition;
    transform.rotation = Quaternion.Euler(0, 0, 0);  // Manually set rotation to zero

    // Ensure all triggers are reset and set to idle to clear any potential lingering triggers
    ResetAllTriggers();
    animator.SetTrigger("idle");
    turnManager.SwitchTurn();
    
}

    private IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        animator.SetTrigger("WalkTrigger");
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
        animator.ResetTrigger("WalkTrigger");
        
        yield return new WaitForSeconds(duration + 1.0f);

        ResetAllTriggers();
    }

    private IEnumerator ResetAnimationStateAfterDelay(string trigger, float delay)
{
    yield return new WaitForSeconds(delay);
    animator.ResetTrigger(trigger);
    animator.SetTrigger("idle");
    transform.position = originalPosition;  // Ensure the position is reset after the animation
    transform.rotation = Quaternion.Euler(0, 0, 0);  // Ensure rotation is reset to default
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

    private void EnableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }

    private void HandleCollision(Collider other)
{
    if (other.transform == targetCharacter && Time.time > lastDamageTime + damageCooldown)
    {
        Animator targetAnimator = targetCharacter.GetComponent<Animator>();
        if (targetAnimator != null)
        {
            AnimatorStateInfo stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(0); // 'idle' is in layer 0
            bool isIdle = stateInfo.IsName("Cat_idle"); // Check if the current state name is "idle"

            if (isIdle)
            {
                targetHealthManager.TakeDamage(15);
                lastDamageTime = Time.time;
                Debug.Log("Damage applied as the target was idle.");
            }
            else
            {
                Debug.Log("No damage applied as the target was not idle.");
            }
        }
        else
        {
            Debug.LogError("Animator component not found on target character.");
        }
    }
}

    private IEnumerator DisableHitboxAfterDelay(Collider hitbox, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitbox != null)
        {
            hitbox.enabled = false;
        }
    }


    private void ResetAllTriggers()
{
    animator.ResetTrigger("WalkTrigger");
    animator.ResetTrigger("BackSlashTrigger");
    animator.ResetTrigger("WhirlWindTrigger");
    animator.SetTrigger("idle");
}

}