using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSpecialMove : MonoBehaviour
{
    public Transform boss; // Reference to the boss Transform.
    public Transform player; // Reference to the player Transform.
    public CameraManager cameraManager; // Reference to the CameraManager script.

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // Replace 'T' with your desired test key.
        {
            StartCoroutine(SpecialAttackSequence());
        }
    }

    private IEnumerator SpecialAttackSequence()
    {
        // Step 1: Switch to launch cinematic camera.
        cameraManager.SwitchToCinematicCamera(CameraFocusType.Launch);

        // Step 2: Launch the player into the air.
        Vector3 launchPosition = player.position + new Vector3(0, 15, 10); // Adjust for height and distance.
        player.DOMove(launchPosition, 1f).SetEase(Ease.OutQuad); // Smooth upward motion.

        yield return new WaitForSeconds(1f); // Wait for the player to reach the height.

        // Step 3: Teleport the boss behind the player.
        Vector3 bossTeleportPosition = launchPosition + new Vector3(0, 0, -3); // Adjust relative to the player.
        boss.position = bossTeleportPosition;

        // Step 4: Switch to the slam cinematic camera.
        cameraManager.SwitchToCinematicCamera(CameraFocusType.Slam);

        yield return new WaitForSeconds(0.5f); // Brief delay.

        // Step 5: Slam the player back to the ground.
        Vector3 slamPosition = new Vector3(player.position.x, 0, player.position.z); // Ground level.
        player.DOMove(slamPosition, 0.5f).SetEase(Ease.InQuad); // Smooth downward motion.

        // Optional: Slow motion on impact.
        Time.timeScale = 0.2f; // Slow down time briefly for dramatic effect.
        yield return new WaitForSecondsRealtime(0.3f); // Wait in real-time during slow motion.
        Time.timeScale = 1f; // Reset time back to normal.

        yield return new WaitForSeconds(0.5f); // Wait for the slam animation to finish.

        // Step 6: Return to gameplay camera.
        cameraManager.SwitchToGameplayCamera();
    }
}
