using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    new Renderer renderer; 
    Material material;
    Color emissionColor;
    
    public bool active;

    void Start() {
        renderer = GetComponentInChildren<Renderer>();
        material = renderer.material;

        emissionColor = material.GetColor("_EmissionColor");
    }


    void Update() {
        Activate(active);
    }

    public void Activate(bool on, float intensity = 1f) {
        if (on) {
            material.EnableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            material.SetColor("_EmissionColor", emissionColor * intensity);

            RendererExtensions.UpdateGIMaterials(renderer);

            DynamicGI.SetEmissive(renderer, emissionColor * intensity);
            DynamicGI.UpdateEnvironment();

        } else {

            material.DisableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;

            material.SetColor("_EmissionColor", Color.black);
            RendererExtensions.UpdateGIMaterials(renderer);

            DynamicGI.SetEmissive(renderer, Color.black);
            DynamicGI.UpdateEnvironment();

        }
    }
}
