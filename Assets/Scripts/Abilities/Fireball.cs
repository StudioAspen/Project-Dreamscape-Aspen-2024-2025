using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float aoeRadius = 5f;
    [SerializeField] private float damage = 10f;

    public int Team;
    public GameObject caster;
    private Vector3 targetPosition;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //just for testing
        //StartCoroutine(MoveTowardsTarget());
    }

    public void Fire(Vector3 direction, Vector2Int damageRange, GameObject caster, int team)
    {
        this.caster = caster;
        this.Team = team;
        targetPosition = direction.normalized * maxDistance;
        StartCoroutine(MoveTowardsTarget());
    }

    private System.Collections.IEnumerator MoveTowardsTarget()
    {
        float distanceTraveled = 0f;

        while (distanceTraveled < maxDistance)
        {
            Vector3 movePosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            rb.MovePosition(movePosition);
            distanceTraveled += (movePosition - transform.position).magnitude;

            yield return null;
        }

        Explode();
    }

    private void Explode()
    {
        List<Entity> entitiesHit = Entity.GetEntitiesThroughAOE(transform.position, aoeRadius);

        foreach (Entity entity in entitiesHit)
        {
            int damageDealt = Mathf.FloorToInt(damage);
            
            if (entity.Team != this.Team) 
            {
                entity.TakeDamage(damageDealt, transform.position, caster);
            }
        }

        Destroy(gameObject);

        //insert explosion vfx here:
    }

    void OnDrawGizmos()
    {
        //Visualize AOE radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }


}
