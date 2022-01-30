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

    public override void Attached()
    {
        defaultIntensity = 3f;
    }

    private void Start()
    {
        //start with the active state
        ActivateCallBack(state.InitState);
    }

    public void SetInitState(bool stateFlag)
    {
        state.InitState = stateFlag;
        state.IsActive = stateFlag;
    }

    public void Activate(bool on)
    {
        if (on && state.IsActive == false)
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
        else if(on == false && state.IsActive == true)
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

    public void ActivateCallBack(bool shouldActivate)
    {
        Debug.LogWarning($"Cristal Via evento - Acao: {shouldActivate} | State: {state.IsActive}");

        ActivateMaterial(shouldActivate, defaultIntensity);
    }

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
