using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SmokePoofVFX : MonoBehaviour
{
    [SerializeField] private VisualEffect smokePoofVFX;
    [SerializeField] private float duration = 3f;

    private bool isActive = false;
    private float activeTimeElapsed = 0.0f;
    
    //-//////////////////////////////////////////////////////////////////////
    ///
    private void Update()
    {
        // Destroy self after time has elapsed
        if (isActive)
        {
            if (activeTimeElapsed > duration)
            {
                Destroy(gameObject);
                return;
            }
            
            activeTimeElapsed += Time.deltaTime;
        }
    }
    
    //-//////////////////////////////////////////////////////////////////////
    ///
    public void Play()
    {
        smokePoofVFX.Play();

        isActive = true;
    }
}
