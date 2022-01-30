using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class CanvasHud : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool Binded;

    public GameObject panelDark;

    public GameObject darkLosing;
    public GameObject darkWinning;
    public GameObject darkTie;

    public GameObject lightLosing;
    public GameObject lightWinning;
    public GameObject lightTie;

    //float sum = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if(!Binded && GameState.Instance != null && GameState.Instance.state != null)
        {
            Binded = true;
            GameState.Instance.state.AddCallback("LightsOn", OnUpdateLightsOn);
        }
        //sum += Time.deltaTime / 10.0f;
        //setBarraProgresso(sum);
    }

    private void FixedUpdate() {
        SyncTimeCount();    
    }

    void setBarraProgresso(float percent)
    {
        darkLosing.SetActive(false);
        darkWinning.SetActive(false);
        darkTie.SetActive(false);
        lightLosing.SetActive(false);
        lightWinning.SetActive(false);
        lightTie.SetActive(false);
        if (percent == 0.5f)
        {
            darkTie.SetActive(true);
            lightTie.SetActive(true);
        }
        else if(percent < 0.5f)
        {
            lightLosing.SetActive(true);
            darkWinning.SetActive(true);
        }
        else
        {
            lightWinning.SetActive(true);
            darkLosing.SetActive(true);
        }

        var guiRight = (percent) * (382.6071f * 2.0f);
        //Debug.Log("GuiRight: " + guiRight);

        //var panelDark = gameObject.transform.Find("PanelDark").gameObject;
        var right = panelDark.GetComponent<RectTransform>().right;

        //panelDark.GetComponent<RectTransform>().right = new Vector3(guiRight, guiRight, guiRight);
        var rt = panelDark.GetComponent<RectTransform>();
        rt.offsetMax = new Vector2(-guiRight, rt.offsetMax.y);
    }

    void SyncTimeCount() {
        var textComponent = GameObject.FindWithTag("TimeCount").GetComponent<UnityEngine.UI.Text>();    

        float timeToEnd = GameState.Instance.MaxGameTime - (BoltNetwork.ServerTime - GameState.Instance.GameStartTime);

        textComponent.text = ((int) timeToEnd).ToString();

        if(timeToEnd < 10) {
            textComponent.color = Color.red;
        }

    }

    void OnUpdateLightsOn()
    {
        var percent = GameState.Instance.state.LightsOn / (float)GameState.Instance.state.TotalCrystals;
        //Debug.Log("Mudou para " + GameState.Instance.state.LightsOn + " percent: " + percent);
        setBarraProgresso(percent);

        //gameObject.Find

        //Debug.Log("Mudou para " + GameState.Instance.state.LightsOn);
    }
}
