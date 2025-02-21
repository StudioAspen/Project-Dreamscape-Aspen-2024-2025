
using UnityEngine; 

public class DreamMushroomSplotchState : DreamMushroomBaseState
{
    private float respawnTimer;
    [SerializeField] private float respawnDuration=60f;

    public override void OnEnter()
    {
        respawnTimer = 0;
        // Leaves paint on the floor have no effect on player 
        Debug.Log("splotch");
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        respawnTimer += Time.deltaTime; 
        if(respawnTimer > respawnDuration)
        {
            dreamMushroom.ChangeState(dreamMushroom.DreamMushroomIdleState);
        }

    }
}
