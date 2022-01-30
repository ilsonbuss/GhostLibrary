using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCredit : MonoBehaviour
{
   
    public float scrollSpeed;
    public GameObject scrollPanel;    // Start is called before the first frame update
    public GameObject MaskArea;
    
    public void Start()
    {
            
    }


    public void Update()
    {
        if (scrollPanel.transform.position.y < 70.0)
        {            
            scrollPanel.transform.Translate((Vector3.up * (scrollSpeed * Time.deltaTime)));         
        }
        
    }

    public void ReniciarScroll()
    {
        scrollPanel.transform.position = new Vector3(scrollPanel.transform.position.x, -100, scrollPanel.transform.position.z);
    }
}
