using UnityEngine;

public class WallCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var color = gameObject.GetComponent<MeshRenderer>().material.color;

            color.a = 0.5f;
            gameObject.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            var color = gameObject.GetComponent<MeshRenderer>().material.color;

            color.a = 1f;
            gameObject.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
