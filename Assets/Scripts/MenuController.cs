using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MenuController : MonoBehaviour
{
    public GameObject telaMenu;
    public GameObject telaCreditos;
    public GameObject telaCutCene;
    public GameObject CutCene;
    public VideoPlayer Video;
    
    // Start is called before the first frame update
    void Start()
    {
        telaMenu.SetActive(true);
        telaCreditos.SetActive(false);
        telaCutCene.SetActive(false);
        CutCene.SetActive(false);
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


    public void ShowGameRoms()
    {               

        StartCoroutine(AfterCene());

    }

    IEnumerator AfterCene()
    {
        telaCutCene.SetActive(true);
        CutCene.SetActive(true);
        Video.frame = 0;
        Video.Play();

        yield return new WaitForSeconds(7);
        UnityEngine.SceneManagement.SceneManager.LoadScene("PhotonLobby");

    }
    public void QuitGame()
    {
        Application.Quit();
    }


    public void ReturnMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuInicial");
    }
   
}
