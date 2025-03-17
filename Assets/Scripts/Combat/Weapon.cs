using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

public class Weapon : MonoBehaviour
{
    private GameManager gameManager;

    private List<CapsuleCollider> capsuleColliders = new List<CapsuleCollider>();
    private ParticleSystem trailParticle;
    public Entity HolderEntity { get; private set; }
    private LayerMask hitLayerMask; // Assigned in awake

    #region Scale
    public float OriginalScale { get; private set; } = 1f;
    public void SetOriginalScale(float newScale) => OriginalScale = newScale;
    #endregion

    #region Between-Frame Collisions
    private bool isCheckingCollisions = false;
    private bool willDoDamage = true;
    private List<Transform> colliderStartTransforms = new List<Transform>();
    private List<Transform> colliderEndTransforms = new List<Transform>();
    private Ray[] currentFrameCollisionRays;
    private Ray[] previousFrameCollisionRays;
    private int currentHitFrame;
    #endregion

    #region Events
    /// <summary>
    /// Action that is invoked when the weapon starts swinging.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Entity attacker</c>: The attacker entity</description></item>
    /// /// <item><description><c>ComboDataSO combo</c>: The started combo</description></item>
    /// </list>
    /// </remarks>
    public Action<Entity, ComboDataSO> OnWeaponStartSwing = delegate { };
    /// <summary>
    /// Action that is invoked when the weapon stops swinging.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Entity attacker</c>: The attacker entity</description></item>
    /// <item><description><c>ComboDataSO combo</c>: The ended combo</description></item>
    /// </list>
    /// </remarks>
    public Action<Entity, ComboDataSO> OnWeaponEndSwing = delegate { };
    /// <summary>
    /// Action that is invoked when the weapon hits.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Entity attacker</c>: The attacker entity</description></item>
    /// <item><description><c>Entity victim</c>: The victim entity that got hit</description></item>
    /// <item><description><c>Vector3 hitPoint</c>: Where the hit was</description></item>
    /// <item><description><c>int damage</c>: How much damage the hit did</description></item>
    /// </list>
    /// </remarks>
    public Action<Entity, Entity, Vector3, int> OnWeaponHit = delegate { };
    /// <summary>
    /// Action that is invoked when the weapon hits another weapon.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Weapon attackingWeapon</c>: The attacker weapon</description></item>
    /// <item><description><c>Weapon victimWeapon</c>: The victim weapon that got hit</description></item>
    /// <item><description><c>Vector3 hitPoint</c>: Where the hit was</description></item>
    /// </list>
    /// </remarks>
    public Action<Weapon, Weapon, Vector3> OnWeaponHitWeapon = delegate { };
    #endregion

    [field: Header("Weapon: Combo")]
    [field: SerializeField] public List<ComboDataSO> Combos { get; private set; }
    private float damageMultiplier;
    private float impactFramesTimeScale;
    private float impactFramesDuration;
    private float impactFramesRemainingTime;
    private Coroutine impactFramesCoroutine;
    private List<GameObject> objectsHitByCurrentAttack = new List<GameObject>();

    [field: Header("Weapon: Tip")]
    [field: SerializeField] public Transform TipTransform { get; private set; }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        capsuleColliders = GetComponents<CapsuleCollider>().ToList();
        trailParticle = GetComponentInChildren<ParticleSystem>();
        HolderEntity = GetComponentInParent<Entity>();

        hitLayerMask = LayerMask.GetMask("Damageable Entity", "Obstacle", "Damage Collider");

        PopulateColliderStartEndPositions();

        OriginalScale = transform.localScale.x;

        DisableTriggers();
    }

    /// <summary>
    /// Creates and assigns the start and end positions for each collider attached to the weapon.
    /// This method is responsible for populating the colliderStartTransforms and colliderEndTransforms lists,
    /// as well as initializing the currentFrameCollisionRays and previousFrameCollisionRays arrays.
    /// </summary>
    private void PopulateColliderStartEndPositions()
    {
        for (int i = 0; i < capsuleColliders.Count; i++)
        {
            GameObject start = new GameObject($"Collider{i} Start");
            GameObject end = new GameObject($"Collider{i} End");

            start.transform.SetParent(transform);
            end.transform.SetParent(transform);

            Vector3 capsuleColliderDirection = Vector3.up; // Assume the capsule collider direction is set to Y-Axis by default
            switch (capsuleColliders[i].direction)
            {
                case 0: capsuleColliderDirection = Vector3.right; break; // X-Axis
                case 1: capsuleColliderDirection = Vector3.up; break; // Y-Axis
                case 2: capsuleColliderDirection = Vector3.forward; break; // Z-Axis
            }

            start.transform.localPosition = capsuleColliders[i].center - (0.5f * capsuleColliders[i].height - capsuleColliders[i].radius) * capsuleColliderDirection;
            end.transform.localPosition = capsuleColliders[i].center + (0.5f * capsuleColliders[i].height - capsuleColliders[i].radius) * capsuleColliderDirection;

            colliderStartTransforms.Add(start.transform);
            colliderEndTransforms.Add(end.transform);
        }

        currentFrameCollisionRays = new Ray[capsuleColliders.Count];
        previousFrameCollisionRays = new Ray[capsuleColliders.Count];
    }

    private void OnEnable()
    {
        DisableTriggers();
    }

    private void Update()
    {
        HandleHitDetectionBetweenFrames();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isCheckingCollisions) return;
        if (!willDoDamage) return;
        if ((hitLayerMask & (1 << other.gameObject.layer)) == 0) return; // if not in the layer mask

        CheckEntityCollisionsWithTrigger(other);
        CheckObstacleCollisionsWithTrigger(other);
        CheckWeaponCollisionsWithTrigger(other);
    }

    /// <summary>
    /// Handles entity hit detection with the triggers
    /// </summary>
    /// <param name="other">The collider hit by the trigger</param>
    private void CheckEntityCollisionsWithTrigger(Collider other)
    {
        Entity enemy = other.GetComponentInParent<Entity>();
        Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);
        AttemptToHitEntity(enemy, hitPoint, true);
    }

    /// <summary>
    /// Handles obstacle hit detection with the triggers
    /// </summary>
    /// <param name="other">The collider hit by the trigger</param>
    private void CheckObstacleCollisionsWithTrigger(Collider other)
    {
        Obstacle obstacle = other.GetComponent<Obstacle>();
        Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);

        AttemptToHitObstacle(obstacle, hitPoint, true);
    }

    /// <summary>
    /// Handles weapon hit detection with the triggers
    /// </summary>
    /// <param name="other">The collider hit by the trigger</param>
    private void CheckWeaponCollisionsWithTrigger(Collider other)
    {
        Weapon weapon = other.GetComponent<Weapon>();
        Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);

        AttemptToHitWeapon(weapon, hitPoint, true);
    }

    /// <summary>
    /// Handles hit detection between frames.
    /// </summary>
    private void HandleHitDetectionBetweenFrames()
    {
        if (!isCheckingCollisions)
        {
            currentHitFrame = 0;
            return;
        }

        if (!willDoDamage)
        {
            currentHitFrame = 0;
            return;
        }

        // Loop through every capsule collider attached
        for (int i = 0; i < capsuleColliders.Count; i++)
        {
            previousFrameCollisionRays[i] = currentFrameCollisionRays[i];

            // Calculate the current frame collision ray (from start to end)
            Vector3 dir = colliderEndTransforms[i].position - colliderStartTransforms[i].position;
            currentFrameCollisionRays[i] = new Ray(colliderStartTransforms[i].position, dir);

            if (currentHitFrame > 0)
            {
                // Split the current fram ray into segments and sphere cast between each frame's segment
                int segments = (int)Mathf.Ceil(dir.magnitude / capsuleColliders[i].radius);
                for (int s = 0; s <= segments; s++)
                {
                    Vector3 currPoint = currentFrameCollisionRays[i].origin + s / (float)segments * currentFrameCollisionRays[i].direction;
                    Vector3 prevPoint = previousFrameCollisionRays[i].origin + s / (float)segments * previousFrameCollisionRays[i].direction;

                    CheckHitsWithSphereCast(new Ray(prevPoint, currPoint - prevPoint), Vector3.Distance(currPoint, prevPoint), capsuleColliders[i].radius * transform.lossyScale.x);

                    // Debugging
/*                    Debug.DrawLine(currPoint, prevPoint, Color.red, 2f);
                    CustomDebug.InstantiateTemporarySphere(currPoint, capsuleColliders[i].radius * transform.lossyScale.x, 5f,
                        Color.Lerp(new Color(1f, 0, 0, 0.1f), new Color(0, 0, 1f, 0.1f), (i + 1) / capsuleColliders.Count));
                    CustomDebug.InstantiateTemporarySphere(prevPoint, capsuleColliders[i].radius * transform.lossyScale.x, 5f,
                        Color.Lerp(new Color(1f, 0, 0, 0.1f), new Color(0, 0, 1f, 0.1f), (i + 1) / capsuleColliders.Count));*/
                }
            }
        }

        currentHitFrame++;
    }

    /// <summary>
    /// Checks for hits using a sphere cast and attempts to hit the enemy.
    /// </summary>
    /// <param name="ray">The ray to cast.</param>
    /// <param name="distance">The distance of the sphere cast.</param>
    /// <param name="radius">The radius of the sphere cast.</param>
    private void CheckHitsWithSphereCast(Ray ray, float distance, float radius)
    {
        RaycastHit[] hits = Physics.SphereCastAll(ray, radius, distance, hitLayerMask);

        if (hits == null) return;
        if (hits.Length == 0) return;

        foreach (RaycastHit hit in hits)
        {
            Vector3 hitPoint = hit.collider.ClosestPointOnBounds(hit.point);
            if (hit.distance == 0) hitPoint = hit.collider.ClosestPointOnBounds(transform.position);

            Entity enemy = hit.collider.GetComponentInParent<Entity>();
            AttemptToHitEntity(enemy, hitPoint, false);

            Obstacle obstacle = hit.collider.GetComponent<Obstacle>();
            AttemptToHitObstacle(obstacle, hitPoint, false);

            Weapon weapon = hit.collider.GetComponent<Weapon>();
            AttemptToHitWeapon(weapon, hitPoint, false);
        }
    }

    /// <summary>
    /// Attempts to hit an enemy with the weapon.
    /// </summary>
    /// <param name="victim">The enemy to hit.</param>
    /// <param name="hitPoint">The point of impact.</param>
    /// <param name="fromTrigger">Flag indicating if the hit is from a trigger.</param>
    private void AttemptToHitEntity(Entity victim, Vector3 hitPoint, bool fromTrigger)
    {
        if (victim == null) return;
        if (victim.Team == HolderEntity.Team) return;
        if (victim.CurrentState == victim.EntityDeathState) return;

        if (objectsHitByCurrentAttack.Contains(victim.gameObject)) return;
        objectsHitByCurrentAttack.Add(victim.gameObject);

        HitEntity(victim, hitPoint, fromTrigger);
    }

    /// <summary>
    /// Attempts to hit an obstacle with the weapon.
    /// </summary>
    /// <param name="obstacle">The obstacle to hit.</param>
    /// <param name="hitPoint">The point of impact.</param>
    /// <param name="fromTrigger">Flag indicating if the hit is from a trigger.</param>
    private void AttemptToHitObstacle(Obstacle obstacle, Vector3 hitPoint, bool fromTrigger)
    {
        if (obstacle == null) return;
        if (HolderEntity.Team != 0) return; // If not in player's team
        if (objectsHitByCurrentAttack.Contains(obstacle.gameObject)) return;

        objectsHitByCurrentAttack.Add(obstacle.gameObject);
        obstacle.TakeDamage(hitPoint, HolderEntity.gameObject);
    }

    /// <summary>
    /// Attempts to hit another weapon with the weapon.
    /// </summary>
    /// <param name="weapon">The weapon to hit.</param>
    /// <param name="hitPoint">The point of impact.</param>
    /// <param name="fromTrigger">Flag indicating if the hit is from a trigger.</param>
    private void AttemptToHitWeapon(Weapon weapon, Vector3 hitPoint, bool fromTrigger)
    {
        if (weapon == null) return;
        if (weapon == this) return; // If the weapon is your's
        if(weapon.HolderEntity == null) return; // If the weapon has no holder
        if (HolderEntity.Team == weapon.HolderEntity.Team) return; // If the weapon hit was an ally's
        if (objectsHitByCurrentAttack.Contains(weapon.gameObject)) return;

        objectsHitByCurrentAttack.Add(weapon.gameObject);

        // Invoke action for both weapons
        OnWeaponHitWeapon.Invoke(this, weapon, hitPoint);
        weapon.OnWeaponHitWeapon.Invoke(this, weapon, hitPoint);
    }

    /// <summary>
    /// Hits an entity with the weapon, triggering impact frames, camera shake, and damage calculation.
    /// </summary>
    /// <param name="victim">The enemy to hit.</param>
    /// <param name="hitPoint">The point of impact.</param>
    /// <param name="fromTrigger">Flag indicating if the hit is from the trigger.</param>
    private void HitEntity(Entity victim, Vector3 hitPoint, bool fromTrigger)
    {
        StartImpactFrames(impactFramesTimeScale, impactFramesDuration);

        // CustomGizmos.InstantiateTemporarySphere(hitPoint, 0.1f, 1.5f, fromTrigger ? Color.green : Color.red);

        int damageValue = HolderEntity.CalculateDamage(damageMultiplier);

        OnWeaponHit?.Invoke(HolderEntity, victim, hitPoint, damageValue);

        HolderEntity.DealDamageToOtherEntity(victim, damageValue, hitPoint);
    }

    /// <summary>
    /// Starts the impact frames with the specified time scale and duration.
    /// </summary>
    /// <param name="timeScale">The time scale of the impact frames.</param>
    /// <param name="duration">The duration of the impact frames.</param>
    private void StartImpactFrames(float timeScale, float duration)
    {
        if (duration <= 0) return;

        if(impactFramesCoroutine != null)
        {
            impactFramesRemainingTime = Mathf.Max(impactFramesRemainingTime, duration);
            return;
        }
        
        impactFramesRemainingTime = duration;
        impactFramesCoroutine = StartCoroutine(ImpactFramesCoroutine(timeScale));
    }

    /// <summary>
    /// Coroutine that handles the impact frames of the weapon.
    /// </summary>
    /// <param name="timeScale">The time scale of the impact frames.</param>
    private IEnumerator ImpactFramesCoroutine(float timeScale)
    {
        gameManager.SetTimeScale(timeScale);

        while (impactFramesRemainingTime > 0f)
        {
            if (gameManager.CurrentState == GameState.PLAYING)
                impactFramesRemainingTime -= Time.unscaledDeltaTime; // only increment if playing

            yield return null;
        }
        impactFramesRemainingTime = 0f;

        gameManager.SetTimeScale(1);
        impactFramesCoroutine = null;
    }

    /// <summary>
    /// Sets the timescale and duration of the impact frames.
    /// </summary>
    /// <param name="newScale">The new timescale of the impact frames.</param>
    /// <param name="newDuration">The new duration of the impact frames.</param>
    public void ConfigureImpactFrames(float newScale, float newDuration)
    {
        impactFramesTimeScale = newScale;
        impactFramesDuration = newDuration;
    }

    /// <summary>
    /// Clears the list of objects hit by the current attack.
    /// </summary>
    public void ClearObjectHitList()
    {
        objectsHitByCurrentAttack.Clear();
    }

    /// <summary>
    /// Enables all the colliders attached to the weapon.
    /// Sets the isCheckingCollisions flag to true.
    /// </summary>
    /// <param name="willDoDamage">Determines whether the trigger will do damage</param>
    public void EnableTriggers(bool willDoDamage = true)
    {
        isCheckingCollisions = true;

        if (willDoDamage)
        {
            this.willDoDamage = true;
            trailParticle?.Play();
        }

        foreach (CapsuleCollider collider in capsuleColliders)
        {
            collider.enabled = true;
        }
    }

    /// <summary>
    /// Disables all the colliders attached to the weapon.
    /// Sets the isCheckingCollisions flag to false.
    /// </summary>
    public void DisableTriggers()
    {
        isCheckingCollisions = false;

        willDoDamage = false;
        trailParticle?.Stop();

        foreach (CapsuleCollider collider in capsuleColliders)
        {
            collider.enabled = false;
        }
    }

    /// <summary>
    /// Sets the damage multiplier for the weapon.
    /// </summary>
    /// <param name="newDamageMultiplier">The new damage multiplier.</param>
    public void SetDamageMultiplier(float newDamageMultiplier)
    {
        damageMultiplier = newDamageMultiplier;
    }

    public void AddCombo(ComboDataSO comboData)
    {
        Combos.Add(comboData);
    }

    public void RemoveCombo(ComboDataSO comboData)
    {
        Combos.Remove(comboData);
    }

    /// <summary>
    /// Retrieves the list of valid combos based on the specified air combo flag.
    /// </summary>
    /// <param name="isAirCombo">Flag indicating whether the combo is an air combo.</param>
    /// <returns>The list of valid combos.</returns>
    public List<ComboDataSO> GetCombos(bool isAirCombo)
    {
        List<ComboDataSO> validCombos = new List<ComboDataSO>();

        foreach (ComboDataSO comboData in Combos)
        {
            if (comboData.IsAirCombo == isAirCombo) validCombos.Add(comboData);
        }

        return validCombos;
    }
}
