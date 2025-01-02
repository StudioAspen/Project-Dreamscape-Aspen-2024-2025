using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;

public class Weapon : MonoBehaviour
{
    private List<CapsuleCollider> capsuleColliders = new List<CapsuleCollider>();
    private ParticleSystem trailParticle;
    private Entity holderEntity;
    private LayerMask hitLayerMask; // Assigned in awake

    #region Scale
    public float OriginalScale { get; private set; } = 1f;
    public void SetOriginalScale(float newScale) => OriginalScale = newScale;
    #endregion

    #region Between-Frame Collisions
    private bool isCheckingCollisions = false;
    private List<Transform> colliderStartTransforms = new List<Transform>();
    private List<Transform> colliderEndTransforms = new List<Transform>();
    private Ray[] currentFrameCollisionRays;
    private Ray[] previousFrameCollisionRays;
    private int currentHitFrame;
    #endregion

    #region Events
    public Action<Entity> OnWeaponStartSwing = delegate { }; // parameter is the entity that started the swing
    public Action<Entity> OnWeaponEndSwing = delegate { }; // parameter is the entity that ended the swing
    public Action<Entity, Entity, Vector3, int> OnWeaponHit = delegate { }; // parameters: attacker, victim, hit point, damage
    #endregion

    [field: Header("Weapon: Combo")]
    [field: SerializeField] public List<ComboDataSO> Combos { get; private set; }
    private float percentDamage;
    private float impactFramesTimeScale;
    private float impactFramesDuration;
    private List<Entity> entitiesHitByCurrentAttack = new List<Entity>();

    private void Awake()
    {
        capsuleColliders = GetComponents<CapsuleCollider>().ToList();
        trailParticle = GetComponentInChildren<ParticleSystem>();
        holderEntity = GetComponentInParent<Entity>();

        hitLayerMask = LayerMask.GetMask("Damageable Entity");

        PopulateColliderStartEndPositions();
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

            start.transform.localPosition = capsuleColliders[i].center - (0.5f * capsuleColliders[i].height - capsuleColliders[i].radius) * Vector3.up;
            end.transform.localPosition = capsuleColliders[i].center + (0.5f * capsuleColliders[i].height - capsuleColliders[i].radius) * Vector3.up;

            colliderStartTransforms.Add(start.transform);
            colliderEndTransforms.Add(end.transform);
        }

        currentFrameCollisionRays = new Ray[capsuleColliders.Count];
        previousFrameCollisionRays = new Ray[capsuleColliders.Count];
    }

    private void Start()
    {
        OriginalScale = transform.localScale.x;

        DisableTriggers();
    }

    private void Update()
    {
        HandleHitDetectionBetweenFrames();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isCheckingCollisions) return;
        if ((hitLayerMask & (1 << other.gameObject.layer)) == 0) return; // if not in the layer mask

        Entity enemy = other.GetComponentInParent<Entity>();

        Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);

        AttemptToHitEnemy(enemy, hitPoint, true);
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

                    CheckHitsWithSphereCast(new Ray(prevPoint, currPoint - prevPoint), Vector3.Distance(currPoint, prevPoint), capsuleColliders[i].radius * transform.localScale.x);

                    // Debugging
                    /*Debug.DrawLine(currPoint, prevPoint, Color.red, 2f);
                    CustomGizmos.InstantiateTemporarySphere(currPoint, capsuleColliders[i].radius, 5f,
                        Color.Lerp(new Color(1f, 0, 0, 0.1f), new Color(0, 0, 1f, 0.1f), (i + 1) / capsuleColliders.Count));
                    CustomGizmos.InstantiateTemporarySphere(prevPoint, capsuleColliders[i].radius, 5f,
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

            AttemptToHitEnemy(enemy, hitPoint, false);
        }
    }

    /// <summary>
    /// Attempts to hit an enemy with the weapon.
    /// </summary>
    /// <param name="victim">The enemy to hit.</param>
    /// <param name="hitPoint">The point of impact.</param>
    /// <param name="fromTrigger">Flag indicating if the hit is from a trigger.</param>
    private void AttemptToHitEnemy(Entity victim, Vector3 hitPoint, bool fromTrigger)
    {
        if (victim == null) return;
        if (victim.Team == holderEntity.Team) return;
        if (victim.CurrentState == victim.EntityDeathState) return;

        if (entitiesHitByCurrentAttack.Contains(victim)) return;
        entitiesHitByCurrentAttack.Add(victim);

        HitEnemy(victim, hitPoint, fromTrigger);
    }

    /// <summary>
    /// Hits an enemy with the weapon, triggering impact frames, camera shake, and damage calculation.
    /// </summary>
    /// <param name="victim">The enemy to hit.</param>
    /// <param name="hitPoint">The point of impact.</param>
    /// <param name="fromTrigger">Flag indicating if the hit is from the trigger.</param>
    private void HitEnemy(Entity victim, Vector3 hitPoint, bool fromTrigger)
    {
        StartImpactFrames(impactFramesTimeScale, impactFramesDuration);
        CameraShakeManager.Instance.ShakeCamera(5f, 0.25f);

        // CustomGizmos.InstantiateTemporarySphere(hitPoint, 0.1f, 1.5f, fromTrigger ? Color.green : Color.red);

        int damageValue = holderEntity.CalculateDamage(percentDamage);

        OnWeaponHit?.Invoke(holderEntity, victim, hitPoint, damageValue);

        victim.TakeDamage(damageValue, hitPoint, holderEntity.gameObject);
    }

    /// <summary>
    /// Starts the impact frames with the specified time scale and duration.
    /// </summary>
    /// <param name="timeScale">The time scale of the impact frames.</param>
    /// <param name="duration">The duration of the impact frames.</param>
    private void StartImpactFrames(float timeScale, float duration)
    {
        if (impactFramesDuration <= 0) return;

        // cant change timescale without game manager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null) return;

        DOTween.Kill("ImpactFrames");
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = gameManager.DefaultFixedDeltaTime * Time.timeScale;

        DOVirtual.DelayedCall(duration, ResetTimeScale).SetId("ImpactFrames");

        // local function to reset timescale
        void ResetTimeScale()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = gameManager.DefaultFixedDeltaTime;
        }
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
    /// Clears the list of enemies hit by the current attack.
    /// </summary>
    public void ClearEnemiesHitList()
    {
        entitiesHitByCurrentAttack.Clear();
    }

    /// <summary>
    /// Enables all the colliders attached to the weapon.
    /// Sets the isCheckingCollisions flag to true.
    /// </summary>
    public void EnableTriggers()
    {
        isCheckingCollisions = true;

        trailParticle?.Play();

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

        trailParticle?.Stop();

        foreach (CapsuleCollider collider in capsuleColliders)
        {
            collider.enabled = false;
        }
    }

    /// <summary>
    /// Sets the percentage of damage for the weapon.
    /// </summary>
    /// <param name="newPercent">The new percentage of damage.</param>
    public void SetPercentDamage(float newPercent)
    {
        percentDamage = newPercent;
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
