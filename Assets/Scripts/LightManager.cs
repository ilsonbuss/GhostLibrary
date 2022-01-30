using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
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

    //private void Start()
    //{
    //    //start with the active state
    //    ActivateMaterial(isActiveLocal, defaultIntensity);
    //}

    public void SetInitState(bool stateFlag)
    {
        isActiveLocal = stateFlag;
        state.InitState = stateFlag;
        state.IsActive = stateFlag;
    }

    public void ActivateCallBack()
    {
        isActiveLocal = state.IsActive;

        ActivateMaterial(isActiveLocal, defaultIntensity);

        Debug.LogWarning($"Cristal: {entity.NetworkId} - Acao: callBack | State: {state.IsActive}");
    }


    public void Activate(bool on)
    {
        if (on && isActiveLocal == false)
        {
            //ActivateMaterial(on, defaultIntensity);

            //ativa o estado global do cristal
            state.IsActive = true;

            //se entrar aqui é por que o player esta próximo de um crystal e ativou ele
            var crystalEvent = CrystalHit.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            crystalEvent.HitState = true;
            crystalEvent.CrystalInstance = entity;
            crystalEvent.Send(); //avisa o servidor e demais players para registrar o evento de hit do cristal

            Debug.LogWarning($"Cristal: {entity.NetworkId} - Acao: {on} | State: {state.IsActive}");
        }
        else if(on == false && isActiveLocal == true)
        {
            //ActivateMaterial(on, defaultIntensity);

            //desativa o estado global do cristal
            state.IsActive = false;

            //se entrar aqui é por que o player esta próximo de um crystal e ativou ele
            var crystalEvent = CrystalHit.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            crystalEvent.HitState = false;
            crystalEvent.CrystalInstance = entity;
            crystalEvent.Send(); //avisa o servidor e demais players para registrar o evento de hit do cristal

            Debug.LogWarning($"Cristal: {entity.NetworkId} - Acao: {on} | State: {state.IsActive}");
        }
    }

    //public void AcativateOnly(bool shouldActivate)
    //{
    //    Debug.LogWarning($"Cristal Via evento - Acao: {shouldActivate} | State: {state.IsActive}");

    //    ActivateMaterial(shouldActivate, defaultIntensity);
    //}

    private void ActivateMaterial(bool shouldActivate, float intensity)
    {
        var light = GetComponentInChildren<Light>();
        var material = GetComponentInChildren<Renderer>().material;

        if (shouldActivate)
        {
            light.intensity = intensity;
            material.EnableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            material.SetColor("_EmissionColor", emissionColor * intensity);
            DynamicGI.SetEmissive(GetComponent<Renderer>(), emissionColor * intensity);
        }
        else
        {
            light.intensity = 0f;
            material.DisableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;

            material.SetColor("_EmissionColor", Color.black);
            DynamicGI.SetEmissive(GetComponent<Renderer>(), Color.black);
        }
    }
}
