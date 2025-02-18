using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamPond : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.7f;

    private List<Entity> slowedEntities = new List<Entity>();

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Entity entity))
        {
            if (slowedEntities.Contains(entity)) return;
            entity.StatusSpeedModifier.AddMultiplier(speedMultiplier);
            slowedEntities.Add(entity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Entity entity))
        {
            if (!slowedEntities.Contains(entity)) return;
            entity.StatusSpeedModifier.RemoveMultiplier(speedMultiplier);
            slowedEntities.Remove(entity);
        }
    }
}
