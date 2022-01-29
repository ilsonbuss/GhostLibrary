using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject telaMenu;
    public GameObject telaCreditos;

    // Start is called before the first frame update
    void Start()
    {
        telaMenu.SetActive(true);
        telaCreditos.SetActive(false);
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowCreditos()
    {
        telaCreditos.SetActive(true);
        telaMenu.SetActive(false);
    }


    public void CloseCreditos()
    {
        telaCreditos.SetActive(false);
        telaMenu.SetActive(true);
    }

}
