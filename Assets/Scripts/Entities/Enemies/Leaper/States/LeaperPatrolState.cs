using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class LeaperPatrolState : EnemyBaseState {
    private Leaper leaper;

    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;
    bool isJumping = false;
    bool jumpLaunched = false;
    private Tween jumpTween;

    public LeaperPatrolState(Leaper enemy) : base(enemy) 
    {
        leaper = enemy;
    }
    public override void OnEnter() 
    {
        enemy.DefaultTransitionToAnimation("FlatMovement");
        leaper.SetSpeedModifier(1f);
        leaper.ClearTarget();
        wanderTimeElapsed = Mathf.Infinity;
        randomWanderIntervalDuration = Random.Range(leaper.PatrolIntervalDurationRange.x, leaper.PatrolIntervalDurationRange.y);
    }

    public override void OnExit() { }

    public override void Update() 
    {
        wanderTimeElapsed += Time.deltaTime;

        leaper.TryAssignTarget();

        if (wanderTimeElapsed > randomWanderIntervalDuration) 
        {
            wanderTimeElapsed = 0f;
            randomWanderIntervalDuration = Random.Range(leaper.PatrolIntervalDurationRange.x, leaper.PatrolIntervalDurationRange.y);
            currentWanderDestination = GetRandomWanderPoint();
            Debug.DrawRay(currentWanderDestination, Vector3.up * 5f, Color.green, 3f);
            leaper.StartCoroutine(DoLeap());
        }

        if (isJumping) 
        {
            leaper.LookAt(currentWanderDestination);
        }

        // Setting speed modifier here is more for animation speed than movement speed (jumping uses tweening)
        leaper.SetSpeedModifier(!jumpLaunched ? 0f : 1f);

        if (leaper.Target != null) 
        {
            //leaper.LeaperTargetDetectedState.AssignCurrentRememberedTarget(leaper.Target);
            //leaper.ChangeState(leaper.LeaperTargetDetectedState);
            return;
        }
    }

    public override void FixedUpdate() { }

    private bool CloseToPoint(Vector3 point, float error) 
    {
        return leaper.Distance(point) < error;
    }

    private Vector3 GetRandomWanderPoint() 
    {
        // Raycast downwards to prevent charger from not wandering at all because it cannot reach
        RaycastHit raycastHit;
        int numTries = 16;
        for (int i = 0; i < numTries; i++) 
        {
            float randomRadius = Random.Range(leaper.PatrolRadiusRange.x, leaper.PatrolRadiusRange.y);
            Vector3 randomPointOnUnitCircle = Random.onUnitSphere;
            randomPointOnUnitCircle.y = 0;
            Vector3 randomPoint = randomRadius * randomPointOnUnitCircle + leaper.transform.position;
            bool isValidPoint =
                Physics.Raycast(randomPoint + 10f * Vector3.up, Vector3.down, out raycastHit, Mathf.Infinity, LayerMask.GetMask("Ground"))
                && NavMesh.SamplePosition(raycastHit.point, out _, 0.5f, NavMesh.AllAreas);

            if (isValidPoint) return raycastHit.point;
        }
        return leaper.transform.position;
    }

    private IEnumerator DoLeap() 
    {
        isJumping = true;
        yield return new WaitForSeconds(leaper.PatrolJumpPrepareTime);
        jumpLaunched = true;
        Vector3 startPoint = leaper.transform.position;
        Vector3 endPoint = currentWanderDestination;
        Vector3 midPoint = (startPoint + endPoint) / 2;
        midPoint.y += leaper.PatrolJumpHeight;
        Vector3[] path = { startPoint, midPoint, endPoint };
        jumpTween = leaper.transform.DOPath(path, leaper.PatrolJumpDuration, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() => OnJumpTweenComplete()).OnUpdate(CheckForCollision); 
     }

    private void CheckForCollision() 
    {
        Debug.DrawRay(leaper.transform.position + (Vector3.up * .5f), leaper.transform.forward * .5f, Color.blue, 3f);
        if (Physics.Raycast(leaper.transform.position + (Vector3.up * .5f), leaper.transform.forward, out _, .5f, LayerMask.GetMask("Ground"))) 
        {
            OnJumpTweenComplete();
            Debug.DrawRay(leaper.transform.position, Vector3.up * 5f, Color.yellow, 3f);
            jumpTween.Kill();
        }
    }

    private void OnJumpTweenComplete() 
    {
        isJumping = false;
        jumpLaunched = false;
    }

}


