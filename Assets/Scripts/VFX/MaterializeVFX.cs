using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class MaterializeVFX : MonoBehaviour
{
    [SerializeField] private VisualEffect[] _vfxObjects;
    [SerializeField] private Material _materializeMaterial;

    private float materializeSpeed = 0.675f;

    public UnityEvent onMaterializationComplete = new UnityEvent();

    // FOR TESTING
    //
    /*[SerializeField] private SkinnedMeshRenderer _meshRenderer;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        
        TriggerMaterializeVFX(_meshRenderer);
    }*/

    ///-////////////////////////////////////////////////////////////////////////
    /// 
    public void TriggerMaterializeVFX(SkinnedMeshRenderer meshRenderer, Texture2D texture = null)
    {
        foreach (VisualEffect vfxObject in _vfxObjects)
        {
            // Play the VFX object
            vfxObject.Play();
        }
        
        StartCoroutine(MaterializeSkinnedMesh(meshRenderer, texture));
    }
    
    ///-////////////////////////////////////////////////////////////////////////
    /// 
    public void TriggerMaterializeVFX(MeshRenderer meshRenderer, Texture2D texture = null)
    {
        foreach (VisualEffect vfxObject in _vfxObjects)
        {
            // Play the VFX object
            vfxObject.Play();
        }
        
        StartCoroutine(MaterializeMesh(meshRenderer, texture));
    }

    ///-////////////////////////////////////////////////////////////////////////
    /// 
    private IEnumerator MaterializeSkinnedMesh(SkinnedMeshRenderer meshRenderer, Texture2D texture)
    {
        float clip = 1;

        // Get materials on the material renderer
        List<Material> defaultMaterials = new List<Material>();
        meshRenderer.GetSharedMaterials(defaultMaterials);

        List<Material> newMaterials = new List<Material>();
        // Add the materialize material to the renderer
        newMaterials.Add(_materializeMaterial);
        meshRenderer.SetSharedMaterials(newMaterials);
            
        // Get an instance of the instantiated materialize material
        Material materializeMaterial = meshRenderer.material;

        // Set texture if not null
        if (texture != null)
        {
            materializeMaterial.SetTexture("_Texture", texture);
        }
        
        while (clip > 0)
        {
            // Set the clip value in the material
            materializeMaterial.SetFloat("_Clip", clip);
            clip -= Time.deltaTime * materializeSpeed;

            yield return null;
        }
        
        meshRenderer.material.SetFloat("_Clip", 0);
        
        // Remove the materialize material
        meshRenderer.SetSharedMaterials(defaultMaterials);

        onMaterializationComplete.Invoke();
    }
    
    ///-////////////////////////////////////////////////////////////////////////
    /// 
    private IEnumerator MaterializeMesh(MeshRenderer meshRenderer, Texture2D texture)
    {
        float clip = 1;

        // Get materials on the material renderer
        List<Material> defaultMaterials = new List<Material>();
        meshRenderer.GetSharedMaterials(defaultMaterials);

        List<Material> newMaterials = new List<Material>();
        // Add the materialize material to the renderer
        newMaterials.Add(_materializeMaterial);
        meshRenderer.SetSharedMaterials(newMaterials);
            
        // Get an instance of the instantiated materialize material
        Material materializeMaterial = meshRenderer.material;

        // Set texture if not null
        if (texture != null)
        {
            materializeMaterial.SetTexture("_Texture", texture);
        }
        
        while (clip > 0)
        {
            // Set the clip value in the material
            materializeMaterial.SetFloat("_Clip", clip);
            clip -= Time.deltaTime * materializeSpeed;

            yield return null;
        }
        
        meshRenderer.material.SetFloat("_Clip", 0);
        
        // Remove the materialize material
        meshRenderer.SetSharedMaterials(defaultMaterials);

        onMaterializationComplete.Invoke();
    }
}
