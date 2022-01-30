using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    new Renderer renderer;
    Material material;
    Color emissionColor;

    public bool active;

    void Start()
    {
        renderer = GetComponentInChildren<Renderer>();
        material = renderer.material;

        emissionColor = material.GetColor("_EmissionColor");
    }


    void Update()
    {
 
    }

    public void Activate(bool on, float intensity = 1f)
    {
        if (on && active == false)
        {
            active = true;
            material.EnableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            material.SetColor("_EmissionColor", emissionColor * intensity);

            RendererExtensions.UpdateGIMaterials(renderer);

            DynamicGI.SetEmissive(renderer, emissionColor * intensity);
            DynamicGI.UpdateEnvironment();

            //ativa o estado global do cristal
            if (TryGetComponent(out BoltEntity crystalEntity))
            {
                //se entrar aqui é por que o player esta próximo de um crystal e ativou ele
                var crystalEvent = CrystalHit.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
                crystalEvent.HitState = true;
                crystalEvent.Send(); //avisa o servidor e demais players para registrar o evento de hit do cristal

                if (crystalEntity is ICrystalState)
                {
                    (crystalEntity as ICrystalState).IsActive = true;
                }
            }

            Debug.Log("Ativou cristal");
        }
        else if(on == false && active == true)
        {

            active = false;
            material.DisableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;

            material.SetColor("_EmissionColor", Color.black);
            RendererExtensions.UpdateGIMaterials(renderer);

            DynamicGI.SetEmissive(renderer, Color.black);
            DynamicGI.UpdateEnvironment();

            //desativa o estado global do cristal
            if (TryGetComponent(out BoltEntity crystalEntity))
            {
                //se entrar aqui é por que o player esta próximo de um crystal e ativou ele
                var crystalEvent = CrystalHit.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
                crystalEvent.HitState = false;
                crystalEvent.Send(); //avisa o servidor e demais players para registrar o evento de hit do cristal

                if (crystalEntity is ICrystalState)
                {
                    (crystalEntity as ICrystalState).IsActive = false;
                }
            }

            Debug.Log("Desativou cristal");
        }
    }
}
