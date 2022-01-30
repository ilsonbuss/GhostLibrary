using UnityEngine;
using Photon.Bolt;

public class PlayerAtack : EntityBehaviour<ICustomStatePlayer>
{
    public bool AtackIsActive { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {       
            return;
        }

        AtackIsActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        AtackIsActive = false;
    }

    public override void SimulateOwner()
    {
        if (!AtackIsActive)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Atacou");
        }
    }
}