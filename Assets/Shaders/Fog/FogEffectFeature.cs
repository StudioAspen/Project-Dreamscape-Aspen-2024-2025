using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class FogEffectFeature : ScriptableRendererFeature
{
    private RenderPass renderPass;

    public override void Create()
    {
        renderPass = new RenderPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.postProcessEnabled) // Only add if post-processing is enabled
        {
            renderer.EnqueuePass(renderPass);
        }
    }

    [System.Serializable]
    class RenderPass : ScriptableRenderPass
    {
        private Material material;
        private RenderTargetIdentifier source;
        private RenderTargetIdentifier destination;

        public RenderPass()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            Shader fogShader = Shader.Find("Custom/FogEffect");
            if (fogShader == null)
            {
                Debug.LogError("Fog shader not found!");
                return;
            }
            if (material == null)
            {
                material = new Material(fogShader);
            }

            source = renderingData.cameraData.renderer.cameraColorTargetHandle;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("FogEffectFeature");
            var stack = VolumeManager.instance.stack;
            var fogEffect = stack.GetComponent<FogEffectComponent>();

            // Apply fog properties if fog effect is active
            if (fogEffect != null && fogEffect.IsActive())
            {
                material.SetColor("_PFogColor", fogEffect.primaryFogColor.value);
                material.SetColor("_SFogColor", fogEffect.secondaryFogColor.value);
                material.SetFloat("_FogDensity", fogEffect.fogDensity.value);
                material.SetFloat("_SkyBoxFogDensity", fogEffect.skyBoxFogDensity.value);
                material.SetFloat("_FogOffset", fogEffect.fogOffset.value);
                material.SetFloat("_SecondaryOffset", fogEffect.secondaryOffset.value);
                material.SetFloat("_GradientStrength", fogEffect.gradientStrength.value);
            }

            // Blit from source to destination
            cmd.Blit(source, source, material, 0);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
