using UnityEngine;

public class PlayerAtack : MonoBehaviour
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

    private void Update()
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