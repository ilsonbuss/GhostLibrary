using UnityEngine;
using Photon.Bolt;

public class PlayerAtack : EntityBehaviour<ICustomStatePlayer>
{
    public bool AtackIsActive { get; set; }

    public BoltEntity Entity { get; set; }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {       
            return;
        }

        Entity = other.TryGetComponent<BoltEntity>(out var entity) ? entity : null;
        AtackIsActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        AtackIsActive = false;
        Entity = null;
    }

    public override void SimulateOwner()
    {
        if (!AtackIsActive)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            var eventAtack = PlayerAtackEvent.Create();
            eventAtack.Victim = Entity;
            eventAtack.Atacker = entity;
            eventAtack.Send();
        }
    }
}