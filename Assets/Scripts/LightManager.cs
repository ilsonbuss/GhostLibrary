using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightManager : EntityBehaviour<ICrystalState>
{
    //new Renderer renderer;
    //Material material;
    //new Light light;
    public Color emissionColor;
    public float defaultIntensity;
    public bool isActiveLocal;

    public override void Attached()
    {
        defaultIntensity = 3f;

        state.AddCallback("IsActive", ActivateCallBack);
    }

    public void SetInitState(bool stateFlag)
    {
        isActiveLocal = stateFlag;
        state.InitState = stateFlag;
        state.IsActive = stateFlag;
    }

    public void SetState(bool stateFlag)
    {
        isActiveLocal = stateFlag;
        state.IsActive = stateFlag;
    }

    public void ActivateCallBack()
    {
        isActiveLocal = state.IsActive;

        ActivateMaterial(isActiveLocal, defaultIntensity);

       // Debug.LogWarning($"Cristal: {entity.NetworkId} - Acao: callBack | State: {state.IsActive}");
    }


    public void Activate(bool on)
    {
        if (on && isActiveLocal == false)
        {
            //ActivateMaterial(on, defaultIntensity);

            //ativa o estado global do cristal
            state.IsActive = true;

            ProduceEvent(true);
        }
        else if(on == false && isActiveLocal == true)
        {
            //ActivateMaterial(on, defaultIntensity);

            //desativa o estado global do cristal
            state.IsActive = false;

            ProduceEvent(false);
        }
    }

    private void ActivateMaterial(bool shouldActivate, float intensity)
    {
        var light = GetComponentInChildren<Light>();
        //var material = GetComponentInChildren<Renderer>().material;

        var cristal = gameObject.transform.Find("cristal");
        var cristal_black = gameObject.transform.Find("cristal_black");

        if (shouldActivate)
        {
            light.intensity = intensity;

            cristal.gameObject.SetActive(true);
            cristal_black.gameObject.SetActive(false);

            var somEfeitoOn = cristal.GetComponentsInChildren<AudioSource>().FirstOrDefault();
            if (somEfeitoOn != null)
            {
                somEfeitoOn.Play();
            }

            //material.EnableKeyword("_EMISSION");
            //material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            //material.SetColor("_EmissionColor", emissionColor * intensity);
            //DynamicGI.SetEmissive(GetComponent<Renderer>(), emissionColor * intensity);
        }
        else
        {
            light.intensity = 0f;

            cristal.gameObject.SetActive(false);
            cristal_black.gameObject.SetActive(true);

            var somEfeitoOff = cristal_black.GetComponentsInChildren<AudioSource>().FirstOrDefault();
            if (somEfeitoOff != null)
            {
                somEfeitoOff.Play();
            }

            //material.DisableKeyword("_EMISSION");
            //material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;

            //material.SetColor("_EmissionColor", Color.black);
            //DynamicGI.SetEmissive(GetComponent<Renderer>(), Color.black);
        }
    }

    private void ProduceEvent(bool flag)
    {
        //se entrar aqui é por que o player esta próximo de um crystal e ativou ele
        var crystalEvent = CrystalHit.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
        crystalEvent.HitState = flag;
        crystalEvent.CrystalInstance = entity;
        crystalEvent.Send(); //avisa o servidor e demais players para registrar o evento de hit do cristal

        //Debug.LogWarning($"Cristal: {entity.NetworkId} - Acao: {flag} | State: {state.IsActive}");
    }
}
