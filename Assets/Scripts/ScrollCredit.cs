using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCredit : MonoBehaviour
{
   
    public float scrollSpeed;
    public GameObject textToScroll;    // Start is called before the first frame update
    public GameObject textArea;
    
    public void Start()
    {
            
    }


    public void Update()
    {
        if (textToScroll.transform.position.y < 50.0)
        {
            textToScroll.transform.Translate((Vector3.up * (scrollSpeed * Time.deltaTime)));         
        }
        
    }

    public void ReniciarScroll()
    {
        textToScroll.transform.position = new Vector3(textToScroll.transform.position.x, -120, textToScroll.transform.position.z);
    }
}
