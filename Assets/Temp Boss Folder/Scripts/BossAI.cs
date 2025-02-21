using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine; 

public class BossAI : MonoBehaviour
{
    public enum BossState { Spawning, Walking, ThrowingRock, Dazed, TransitioningPhase, LeapSlam, Pulling, Cutscene }
    private BossState currentState;

    [Header("References")]
    public Transform player;
    public GameObject rockPrefab;
    public Transform throwPoint1; // First rock spawn point
    public Transform throwPoint2; // Second rock spawn point
    public BossHealth bossHealth;
    public Renderer bossRenderer;

    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera playerCamera; // Player's camera
    public CinemachineVirtualCamera bossCamera; // Boss POV camera

    [Header("Movement & Timings")]
    public float walkSpeed = 3f;
    public float phaseTwoWalkSpeed = 5f; // Faster walk speed in Phase 2
    public float phaseThreeWalkSpeed = 7f; // Faster walk speed in Phase 3
    public float rockThrowCooldown = 5f;
    public float dazedTime = 3f;
    public float phaseTransitionTime = 5f;
    public int phaseTwoHealthThreshold = 50;
    public int phaseThreeHealthThreshold = 25;
    public float pulseScaleMultiplier = 1.2f;
    public float pulseDuration = 0.5f;
    public float pushBackForce = 10f;
    public float pullForce = 10f; // Force to pull entities towards the boss
    public float maxPullDistance = 20f; // Maximum distance for the pull effect
    public float leapDuration = 1f;
    public float leapHeight = 5f;
    public float slamShockwaveRadius = 10f;
    public float leapCooldown = 10f; // Cooldown for leap attack
    public float cutscenePlayerDistance = 3f; // Distance in front of the boss for the player during cutscene
    public float playerLaunchHeight = 10f; // Height the player is launched to
    public float playerLaunchDuration = 2f; // Time to reach the peak of the launch
    public float bossRushDuration = 1f; // Time for the boss to reach behind the player
    public float slamDownDuration = 0.5f; // Time for the slam down
    public float postSlamDelay = 1f; // Delay after slam before returning to original position
    public float curveIntensity = 5f; // Intensity of the boss's curve around the player

    [Header("Phase 3 Settings")]
    public int leapCount = 3; // Number of additional leaps in Phase 3
    public float leapInterval = 0.5f; // Time between leaps in Phase 3
    public int rockBarrageCount = 10; // Number of rocks in the barrage
    public float rockBarrageIntervalStart = 1f; // Starting interval between rocks
    public float rockBarrageIntervalEnd = 0.1f; // Ending interval between rocks

    private float nextRockThrowTime;
    private float nextLeapTime;
    private bool isDazed;
    private Vector3 originalScale;
    private Vector3 originalPosition; // Boss's original position
    private bool isPhaseTwo;
    private bool isPhaseThree;
    private bool isWaitingForCutsceneTrigger; // Flag to wait for key press

    void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.position; // Store the boss's original position
        currentState = BossState.Spawning;
        StartCoroutine(SpawnSequence());

        // Ensure the player camera is active at the start
        playerCamera.Priority = 10;
        bossCamera.Priority = 0;
    }

    IEnumerator SpawnSequence()
    {
        yield return new WaitForSeconds(2f); // Simulate spawn delay
        currentState = BossState.Walking;
    }

    void Update()
    {
        if (bossHealth.IsInvulnerable) return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            bossHealth.TakeDamage(10);
            CheckPhaseTransition();
        }

        switch (currentState)
        {
            case BossState.Walking:
                WalkTowardsPlayer();
                if (isPhaseThree)
                {
                    if (CanLeapSlam())
                    {
                        StartCoroutine(LeapSlamPhaseThree());
                    }
                    else if (CanThrowRock())
                    {
                        StartCoroutine(RockBarrage());
                    }
                }
                else if (isPhaseTwo)
                {
                    if (CanLeapSlam())
                    {
                        StartCoroutine(LeapSlam());
                    }
                    else if (CanThrowRock())
                    {
                        StartCoroutine(ThrowRockPhaseTwo());
                    }
                }
                else if (CanThrowRock())
                {
                    StartCoroutine(ThrowRockPhaseOne());
                }
                break;

            case BossState.Dazed:
                // Do nothing while dazed
                break;

            case BossState.Pulling:
                // Continuously pull entities towards the boss
                PullEntities();
                if (IsPlayerInContact() && !isWaitingForCutsceneTrigger)
                {
                    isWaitingForCutsceneTrigger = true;
                    Debug.Log("Press [Space] to trigger the cutscene.");
                }
                if (isWaitingForCutsceneTrigger && Input.GetKeyDown(KeyCode.Space))
                {
                    StartCoroutine(StartCutscene());
                }
                break;

            case BossState.Cutscene:
                // Do nothing during cutscene
                break;
        }
    }

    void WalkTowardsPlayer()
    {
        if (!isDazed)
        {
            float speed = isPhaseThree ? phaseThreeWalkSpeed : (isPhaseTwo ? phaseTwoWalkSpeed : walkSpeed);
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    IEnumerator ThrowRockPhaseOne()
    {
        currentState = BossState.ThrowingRock;

        // Randomly choose a spawn point
        Transform throwPoint = Random.Range(0, 2) == 0 ? throwPoint1 : throwPoint2;

        // Throw 1 rock
        GameObject rock = Instantiate(rockPrefab, throwPoint.position, Quaternion.identity);
        rock.GetComponent<Rigidbody>().velocity = (player.position - throwPoint.position).normalized * 10f;

        // Set next rock throw time only if not dazed
        if (!isDazed)
        {
            nextRockThrowTime = Time.time + rockThrowCooldown;
        }

        yield return new WaitForSeconds(1.5f); // Delay before getting dazed
        StartCoroutine(DazedState());
    }

    IEnumerator ThrowRockPhaseTwo()
    {
        currentState = BossState.ThrowingRock;

        // Throw 2 rocks, alternating between spawn points
        for (int i = 0; i < 2; i++)
        {
            Transform throwPoint = i == 0 ? throwPoint1 : throwPoint2;
            GameObject rock = Instantiate(rockPrefab, throwPoint.position, Quaternion.identity);
            rock.GetComponent<Rigidbody>().velocity = (player.position - throwPoint.position).normalized * 10f;
            yield return new WaitForSeconds(0.5f); // Delay between rock throws
        }

        // Set next rock throw time only if not dazed
        if (!isDazed)
        {
            nextRockThrowTime = Time.time + rockThrowCooldown;
        }

        yield return new WaitForSeconds(1.5f); // Delay before getting dazed
        StartCoroutine(DazedState());
    }

    IEnumerator RockBarrage()
    {
        currentState = BossState.ThrowingRock;

        // Calculate the interval decrease per rock
        float intervalDecrease = (rockBarrageIntervalStart - rockBarrageIntervalEnd) / rockBarrageCount;

        for (int i = 0; i < rockBarrageCount; i++)
        {
            // Alternate between spawn points
            Transform throwPoint = i % 2 == 0 ? throwPoint1 : throwPoint2;
            GameObject rock = Instantiate(rockPrefab, throwPoint.position, Quaternion.identity);
            rock.GetComponent<Rigidbody>().velocity = (player.position - throwPoint.position).normalized * 10f;

            // Decrease the interval between rocks
            float currentInterval = Mathf.Lerp(rockBarrageIntervalStart, rockBarrageIntervalEnd, (float)i / rockBarrageCount);
            yield return new WaitForSeconds(currentInterval);
        }

        // Set next rock throw time only if not dazed
        if (!isDazed)
        {
            nextRockThrowTime = Time.time + rockThrowCooldown;
        }

        yield return new WaitForSeconds(1.5f); // Delay before getting dazed
        StartCoroutine(DazedState());
    }

    IEnumerator DazedState()
    {
        isDazed = true;
        currentState = BossState.Dazed;
        yield return new WaitForSeconds(dazedTime);
        isDazed = false;
        currentState = BossState.Walking;
    }

    void CheckPhaseTransition()
    {
        if (bossHealth.currentHealth <= phaseTwoHealthThreshold && !isPhaseTwo)
        {
            isPhaseTwo = true;
            StartCoroutine(TransitionToPhaseTwo());
        }
        else if (bossHealth.currentHealth <= phaseThreeHealthThreshold && !isPhaseThree)
        {
            isPhaseThree = true;
            StartCoroutine(TransitionToPhaseThree());
        }
    }

    IEnumerator TransitionToPhaseTwo()
    {
        currentState = BossState.TransitioningPhase;
        bossHealth.SetInvulnerable(true);

        // Initial push force
        PushBackEntities();

        // Smooth Pulse Effect
        Sequence pulseSequence = DOTween.Sequence();
        pulseSequence.Append(transform.DOScale(originalScale * pulseScaleMultiplier, pulseDuration).SetEase(Ease.InOutQuad))
                      .Append(transform.DOScale(originalScale, pulseDuration).SetEase(Ease.InOutQuad))
                      .SetLoops(Mathf.FloorToInt(phaseTransitionTime / (pulseDuration * 2)), LoopType.Yoyo);

        yield return new WaitForSeconds(phaseTransitionTime);

        transform.localScale = originalScale; // Ensure it resets
        bossHealth.SetInvulnerable(false);

        // Start Phase 2 with a leap attack
        StartCoroutine(LeapSlam());
    }

    IEnumerator LeapSlam()
    {
        currentState = BossState.LeapSlam;

        // Leap to player position
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = player.position;
        float elapsedTime = 0f;

        while (elapsedTime < leapDuration)
        {
            float t = elapsedTime / leapDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * Mathf.Sin(t * Mathf.PI) * leapHeight;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Land and create shockwave
        transform.position = targetPosition;
        CreateShockwave();

        // Enter dazed state and set leap cooldown
        nextLeapTime = Time.time + leapCooldown;
        StartCoroutine(DazedState());
    }

    IEnumerator TransitionToPhaseThree()
    {
        currentState = BossState.Pulling;
        bossHealth.SetInvulnerable(true);

        // Start pulsing effect
        StartCoroutine(PulseEffect());

        // Continuously pull entities until the player collides with the boss
        while (!IsPlayerInContact())
        {
            PullEntities();
            yield return null;
        }

        // Wait for key press to trigger cutscene
        isWaitingForCutsceneTrigger = true;
        Debug.Log("Press [Space] to trigger the cutscene.");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        // Start the cutscene
        StartCoroutine(StartCutscene());
    }

    IEnumerator PulseEffect()
    {
        while (true)
        {
            Sequence pulseSequence = DOTween.Sequence();
            pulseSequence.Append(transform.DOScale(originalScale * pulseScaleMultiplier, pulseDuration).SetEase(Ease.InOutQuad))
                          .Append(transform.DOScale(originalScale, pulseDuration).SetEase(Ease.InOutQuad));
            yield return pulseSequence.WaitForCompletion();
        }
    }

    IEnumerator StartCutscene()
    {
        currentState = BossState.Cutscene;

        // Switch to the boss POV camera
        playerCamera.Priority = 0;
        bossCamera.Priority = 15;

        // Position the player in front of the boss
        Vector3 playerPosition = transform.position + transform.forward * cutscenePlayerDistance;
        player.position = playerPosition;

        // Launch the player into the air
        Vector3 launchTarget = player.position + Vector3.up * playerLaunchHeight + transform.forward * 5f; // Adjust forward distance as needed
        player.DOJump(launchTarget, playerLaunchHeight, 1, playerLaunchDuration).SetEase(Ease.OutQuad);

        // Boss waits briefly before rushing to the target point
        yield return new WaitForSeconds(0.5f);

        // Boss follows a curved path around the player
        Vector3 curveStart = transform.position;
        Vector3 curveMid = curveStart + (transform.right * curveIntensity); // Curve to the side
        Vector3 curveEnd = launchTarget + transform.forward * 2f; // Target point behind the player

        // Create a curved path using DOTween's Path system
        transform.DOPath(new Vector3[] { curveStart, curveMid, curveEnd }, bossRushDuration, PathType.CatmullRom)
                 .SetEase(Ease.InOutQuad);

        // Wait for both player and boss to reach the target point
        yield return new WaitForSeconds(bossRushDuration);

        // Slam the player down to the ground
        Vector3 groundPosition = new Vector3(player.position.x, 0, player.position.z); // Adjust ground level as needed
        player.DOMove(groundPosition, slamDownDuration).SetEase(Ease.InQuad);

        // Boss pauses briefly in the air before returning to original position
        yield return new WaitForSeconds(postSlamDelay);

        // Boss returns to original position
        transform.DOMove(originalPosition, 1f).SetEase(Ease.InOutQuad);

        // Switch back to the player camera
        playerCamera.Priority = 10;
        bossCamera.Priority = 0;

        // End the cutscene and start Phase 3
        currentState = BossState.Walking;
        bossHealth.SetInvulnerable(false);
        Debug.Log("Phase 3 begins!");
    }

    IEnumerator LeapSlamPhaseThree()
    {
        currentState = BossState.LeapSlam;

        // Perform the initial leap
        yield return StartCoroutine(LeapToPosition(player.position));

        // Perform additional leaps
        for (int i = 0; i < leapCount; i++)
        {
            yield return new WaitForSeconds(leapInterval);
            yield return StartCoroutine(LeapToPosition(player.position));
        }

        // Enter dazed state and set leap cooldown
        nextLeapTime = Time.time + leapCooldown;
        StartCoroutine(DazedState());
    }

    IEnumerator LeapToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < leapDuration)
        {
            float t = elapsedTime / leapDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * Mathf.Sin(t * Mathf.PI) * leapHeight;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Land and create shockwave
        transform.position = targetPosition;
        CreateShockwave();
    }

    void CreateShockwave()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, slamShockwaveRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (hit.transform.position - transform.position).normalized;
                rb.AddForce(direction * pushBackForce, ForceMode.Impulse);
            }
        }
    }

    void PullEntities()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxPullDistance);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pullDirection = (transform.position - hit.transform.position).normalized;
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float force = Mathf.Lerp(pullForce, 0, distance / maxPullDistance);
                rb.AddForce(pullDirection * force, ForceMode.Impulse);
            }
        }
    }

    bool IsPlayerInContact()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f); // Check a small radius around the boss
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Player")) // Ensure the player has the "Player" tag
            {
                return true;
            }
        }
        return false;
    }

    void PushBackEntities()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxPullDistance);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDirection = (hit.transform.position - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float force = Mathf.Lerp(pushBackForce, 0, distance / maxPullDistance);
                rb.AddForce(pushDirection * force, ForceMode.Impulse);
            }
        }
    }

    bool CanLeapSlam()
    {
        return Time.time >= nextLeapTime && !isDazed;
    }

    bool CanThrowRock()
    {
        return Time.time >= nextRockThrowTime && !isDazed;
    }
}
