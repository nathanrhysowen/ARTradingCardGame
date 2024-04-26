using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Fireball : MonoBehaviour
{
    public float speed = 0.2f;
    public Transform target;
    private Vector3 targetPosition;
    public bool isActive = false; // Initially inactive
    

     void Start()
    {
        if (target != null)
        {
            targetPosition = target.position; // Get the target's position on spawn
            Debug.Log("Fireball started. Target position: " + targetPosition);
        }
        else
        {
            Debug.LogError("Target not assigned.");
        }
    }

    void Update()
    {
        if (isActive && target != null)
        {
            // Move towards the target if active
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Check if the fireball has reached the target
            if (Vector3.Distance(transform.position, targetPosition) <= 0.001f)
            {
                if (target.GetComponent<HealthManager>() != null)
                {
                    target.GetComponent<HealthManager>().TakeDamage(20); // Apply damage to the target
                }
                Destroy(gameObject); // Destroy the fireball when it reaches the target
            }
        }
    }

    public void Activate()
    {
        isActive = true; // Activate movement on call
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == target)
        {
            // Call a method on the target to deal damage
            if (target.GetComponent<HealthManager>() != null)
            {
                target.GetComponent<HealthManager>().TakeDamage(20); 
            }
            Destroy(gameObject); // Destroy the fireball on hit
            Debug.Log("Fireball hit the target and is destroyed.");
        }
    }
}
