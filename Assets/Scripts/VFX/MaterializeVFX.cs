using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MaterializeVFX : MonoBehaviour
{
    [SerializeField] private VisualEffect[] _vfxObjects;
    
    public void TriggerMaterializeVFX(Mesh mesh)
    {
        foreach (VisualEffect vfxObject in _vfxObjects)
        {
            // Set mesh to mesh input
            if (vfxObject.HasMesh("MeshToMat"))
            {
                vfxObject.SetMesh("MeshToMat", mesh);
            }
            
            // Play the VFX object
            vfxObject.Play();
        }
    }
}
