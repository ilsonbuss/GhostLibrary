using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : EntityBehaviour<ICrystalState>
{
    new Renderer renderer;
    Material material;
    new Light light;
    public Color emissionColor;

    public float defaultIntensity = 3f;

    public override void Attached()
    {
        renderer = GetComponentInChildren<Renderer>();
        light = GetComponentInChildren<Light>();
        material = renderer.material;
    }

    private void Start()
    {
        //start with the active state
        ActivateCallBack(state.InitState);
    }

    public void Activate(bool on)
    {
        if (on && state.IsActive == false)
        {
            ActivateMaterial(on, defaultIntensity);

            //ativa o estado global do cristal
            state.IsActive = true;

            //se entrar aqui é por que o player esta próximo de um crystal e ativou ele
            var crystalEvent = CrystalHit.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            crystalEvent.HitState = true;
            crystalEvent.CrystalInstance = entity;
            crystalEvent.Send(); //avisa o servidor e demais players para registrar o evento de hit do cristal
        }
        else if(on == false && state.IsActive == true)
        {
            ActivateMaterial(on, defaultIntensity);

            //desativa o estado global do cristal
            state.IsActive = false;

            //se entrar aqui é por que o player esta próximo de um crystal e ativou ele
            var crystalEvent = CrystalHit.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            crystalEvent.HitState = false;
            crystalEvent.CrystalInstance = entity;
            crystalEvent.Send(); //avisa o servidor e demais players para registrar o evento de hit do cristal
        }
    }

    public void ActivateCallBack(bool shouldActivate)
    {
        ActivateMaterial(shouldActivate, defaultIntensity);
    }

    private void ActivateMaterial(bool shouldActivate, float intensity)
    {
        if (shouldActivate)
        {
            light.intensity = intensity;
            material.EnableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            material.SetColor("_EmissionColor", emissionColor * intensity);
            DynamicGI.SetEmissive(renderer, emissionColor * intensity);
        }
        else
        {
            light.intensity = 0f;
            material.DisableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;

            material.SetColor("_EmissionColor", Color.black);
            DynamicGI.SetEmissive(renderer, Color.black);
        }
    }
}
