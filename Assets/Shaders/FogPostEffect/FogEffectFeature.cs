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
        if (renderingData.cameraData.postProcessEnabled)
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

            //var cameraData = renderingData.cameraData;
            //if (cameraData.isSceneViewCamera) return;
        
            CommandBuffer cmd = CommandBufferPool.Get("FogEffectFeature");
            var stack = UnityEngine.Rendering.VolumeManager.instance.stack;
            var fogEffect = stack.GetComponent<FogEffectComponent>();

            if (fogEffect == null || !fogEffect.IsActive())
            {
                CommandBufferPool.Release(cmd);
                return;
            }

            if (fogEffect != null && fogEffect.IsActive())
            {
                material.SetColor("_PFogColor", fogEffect.primaryFogColor.value);
                material.SetColor("_SFogColor", fogEffect.secondaryFogColor.value);
                material.SetColor("_SkyBoxFogColor", fogEffect.skyBoxFogColor.value);
                material.SetFloat("_FogDensity", fogEffect.fogDensity.value);
                material.SetFloat("_SkyBoxFogDensity", fogEffect.skyBoxFogDensity.value);
                material.SetFloat("_FogOffset", fogEffect.fogOffset.value);
                material.SetFloat("_PrimaryFogColorOffset", fogEffect.primaryFogColorOffset.value);
                material.SetFloat("_SecondaryFogColorOffset", fogEffect.secondaryFogColorOffset.value);
                material.SetFloat("_GradientStrength", fogEffect.gradientStrength.value);
                material.SetFloat("_FogScattering", fogEffect.fogScattering.value);
                material.SetTexture("_NoiseTex", fogEffect.noiseTexture.value);
                material.SetFloat("_FogNoiseScale", fogEffect.fogNoiseScale.value);
                material.SetFloat("_FogNoiseVelocity", fogEffect.fogNoiseVelocity.value);
                material.SetFloat("_SkyBoxNoiseScale", fogEffect.skyBoxNoiseScale.value);
                material.SetFloat("_SkyBoxNoiseVelocity", fogEffect.skyBoxNoiseVelocity.value);
                material.SetFloat("_SkyBoxNoiseTransparency", fogEffect.skyBoxNoiseTransparency.value);
                material.SetFloat("_RotateFogNoise", fogEffect.rotateFogNoise.value);
                material.SetFloat("_RotateSkyBoxNoise", fogEffect.rotateSkyBoxNoise.value);
            }

            RenderTargetIdentifier tempTexture = new RenderTargetIdentifier("_TemporaryTexture");
            cmd.GetTemporaryRT(Shader.PropertyToID("_TemporaryTexture"), renderingData.cameraData.cameraTargetDescriptor);

            // Blit from source to the temporary texture, then back to the source
            cmd.Blit(source, tempTexture, material, 0);
            cmd.Blit(tempTexture, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            cmd.ReleaseTemporaryRT(Shader.PropertyToID("_TemporaryTexture"));

            // Blit from source to destination
            //cmd.Blit(source, source, material, 0);
            //context.ExecuteCommandBuffer(cmd);
            //CommandBufferPool.Release(cmd);
        }
    }
}
