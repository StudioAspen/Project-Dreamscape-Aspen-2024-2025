
// An NPC will run around the map for X minutes. Only land the NPC stands on spawn enemies,if they survive trigger EOW
public class EscortWorldEvent : BaseState
{
    private EventManager eventManager;
    private WorldManager worldManager;

    public EscortWorldEvent(EventManager eventManager, WorldManager worldManager)
    {
        this.eventManager = eventManager;
        this.worldManager = worldManager;
    }

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }
}
