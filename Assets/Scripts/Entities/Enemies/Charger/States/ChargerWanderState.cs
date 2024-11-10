using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class ChargerWanderState : EnemyBaseState
{	
    private Charger charger;
	private Vector3 wanderDestination;
	private float wanderTimeElapsed;

	private float canChaseTimer;
	private float canChaseCooldown = 2f;
	public ChargerWanderState(Charger enemy) : base(enemy) 
	{
		charger = enemy;
	}
	public override void OnEnter()
	{
		canChaseTimer = 0f;
		enemy.DefaultTransitionToAnimation("FlatMovement");
		charger.SetSpeedModifier(1f);
		wanderTimeElapsed = 0f;
		wanderDestination = charger.transform.position;
		GoToRandomWanderPoint();

		Ticker.Instance.OnTick.AddListener(Ticker_OnTick);
	}

	public override void OnExit() {

		Ticker.Instance.OnTick.RemoveListener(Ticker_OnTick);
	}

	private void Ticker_OnTick()
	{
		targetPlayer();
	}

	public override void Update()
	{
		wanderTimeElapsed += Time.deltaTime;
		canChaseTimer += Time.deltaTime;
		
		bool exitCondition = CloseEnoughToPoint(wanderDestination) || wanderTimeElapsed > charger.WanderTimeout;

		if (exitCondition) 
		{
			Debug.Log("change to idle from wander by exitCondition");
			charger.ChangeState(charger.ChargerIdleState);
			return;
		}

		charger.SetDestination(wanderDestination, true);

		


	}

	public override void FixedUpdate() { }

	private bool CloseEnoughToPoint(Vector3 point)
	{
		return (charger.transform.position - point).magnitude < charger.WanderEndDistanceThreshold;
	}


	#region Debug
	//private IEnumerator CreateDebugPoint(Vector3 pos)
	//{
	//    // show destination point for debugging
	//    GameObject hitPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
	//    hitPoint.transform.localScale = new Vector3(.2f, .2f, .2f);
	//    hitPoint.transform.position = pos;
	//    hitPoint.GetComponent<Collider>().enabled = false;
	//    yield return new WaitForSeconds(charger.WanderTimeout);
	//    GameObject.Destroy(hitPoint);
	//    yield return null;
	//}
	#endregion


	private void GoToRandomWanderPoint() 
	{
		// Raycast downwards to prevent charger from not wandering at all because it cannot reach
		RaycastHit raycastHit = new();
		bool success = false;
		int numTries = 16;
		int currTry = 0;

		while (currTry <= numTries) 
		{
			currTry++;

			Vector3 randomPoint = ((Random.onUnitSphere * charger.WanderMinRadius) + Random.insideUnitSphere * charger.WanderMaxRadius) + charger.transform.position;
			randomPoint.y += charger.WanderMaxRadius; // Offset the y before downward raycast

			// Success if raycast hits something and also hits NavMesh
			success = Physics.Raycast(randomPoint, Vector3.down, out raycastHit, charger.WanderRaycastDownMaxDistance) && NavMesh.SamplePosition(raycastHit.point, out _, charger.WanderNavMeshSampleRadius, NavMesh.AllAreas);
			#region Debug
			//Debug.DrawRay(randomPoint, Vector3.down * charger.WanderRaycastDownMaxDistance, success ? Color.green : Color.red, charger.WanderTimeout);
			#endregion
			if (success) 
			{
				break;
			}
		}

		if (!success) 
		{
			#region Debug
			// Debug.Log("Tried " + (currTry) + " times but failed to raycast down!");
			#endregion
			Debug.Log("!success");
			charger.ChangeState(charger.ChargerIdleState);
			return;
		}

		#region Debug
		//Debug.Log("Find wander point " + wanderDestination + " success; took " + (currTry) + (currTry > 1 ? " tries" : " try"));
		#endregion
		wanderDestination = raycastHit.point + Vector3.up; // Offset up a little so point is not on ground
		//charger.StartCoroutine(CreateDebugPoint(wanderDestination));
	}

	private void targetPlayer()
	{
		Debug.Log("try target player");
		// When charger sees player, change state to ChargerPlayerDetectedState
		
		if (charger.Target == null) return;

		if (charger.Distance(charger.Target) > charger.WanderMaxRadius)
		{
			Debug.Log("switch to chase from wander");
			charger.ChangeState(charger.ChargerChaseState);
			return;
		}
		if (charger.Distance(charger.Target) < charger.AttackRange)
		{
			Debug.Log("Switch to Close Attack from wander");
			charger.ChangeState(charger.ChargerCloseAttackState);
			return;
		}
		TryToChasePlayer();

	}

	private void TryToChasePlayer()
    {
        if (canChaseTimer < canChaseCooldown) return;

        if (charger.Target.TryGetComponent(out Player player))
        {
            List<Charger> playerNearbyChargers = player.GetNearbyEntitiesByType<Charger>(charger.WanderMinRadius + 1f);

            foreach (Charger c in new List<Charger>(playerNearbyChargers)) // filter so that we only look for followers that are alive
            {
                if (c.CurrentState == c.EntityDeathState) playerNearbyChargers.Remove(c);
            }

            playerNearbyChargers = playerNearbyChargers.Take(charger.WanderChargerCountThreshold).ToList();

            if (!playerNearbyChargers.Contains(charger)) return;

            charger.ChangeState(charger.ChargerChaseState);
        }
    }
}
