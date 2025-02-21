using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RockScript : MonoBehaviour
{
    public float speed = 5f;              // Speed of the rock's movement
    public float arcHeight = 3f;          // Height of the arc
    public float lifetime = 5f;           // Time before the rock disappears
    public float damage = 20f;            // Damage dealt by the rock

    private Transform target;             // The player or target to hit
    private Vector3 startPos;             // The initial position from which the rock is thrown
    private float startTime;              // Time when the rock is thrown
    private Vector3 targetPos;            // The position where the rock is aimed

    void Start()
    {
        startPos = transform.position;
        startTime = Time.time;

        // Assuming we have a reference to the player's position
        // You can replace the player's position with a different target if needed
        target = GameObject.FindWithTag("Player").transform;

        if (target != null)
        {
            targetPos = target.position;
        }

        // Destroy the rock after a set time to avoid it staying forever
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Calculate the time percentage of the projectile's flight
        float journeyLength = Vector3.Distance(startPos, targetPos);
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;

        // Calculate the arc by modifying the Y position of the rock
        Vector3 horizontalMove = Vector3.Lerp(startPos, targetPos, fractionOfJourney);
        float verticalMove = Mathf.Sin(fractionOfJourney * Mathf.PI) * arcHeight; // Arc effect

        // Combine horizontal and vertical movement
        transform.position = new Vector3(horizontalMove.x, horizontalMove.y + verticalMove, horizontalMove.z);

        // If the rock hits the player, deal damage
        if (Vector3.Distance(transform.position, target.position) < 1f) // Adjust the threshold as needed
        {
        
            Destroy(gameObject); // Destroy the rock after hitting the player
        }
    }
}
