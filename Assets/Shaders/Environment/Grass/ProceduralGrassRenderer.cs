using UnityEngine;
using UnityEngine.Rendering;

namespace Dreamscape.Grass
{
    ///-/////////////////////////////////////////////////////////////////////////////////////////////////
    ///
    /// Holds grass settings
    /// 
    [System.Serializable]
    public class GrassSettings
    {
        [Header("Blades")]
        [Tooltip("Max number of grass segments.")] 
        [Range(1, 5)]
        public int maxSegments = 3;
        [Tooltip("A blade of grass' maximum bend angle, as a multiplier to 90 degrees.")]
        [Range(0, 1)]
        public float maxBendAngle = 0;
        [Tooltip("The blade curvature shape.")]
        [Range(0, 5)]
        public float bladeCurvature = 1;
        [Tooltip("The base height of a blade.")]
        public float bladeHeight = 1;
        [Tooltip("The height variance of a blade.")]
        public float bladeHeightVariance = 0.1f;
        [Tooltip("The base width of a blade.")]
        public float bladeWidth = 1;
        [Tooltip("The width variance of a blade.")]
        public float bladeWidthVariance = 0.1f;

        [Header("Wind")] 
        [Tooltip("The noise texture used to control the wind. Red and green channels are used for x and z offsets.")]
        public Texture2D windNoiseTexture;
        [Tooltip("The scale of the wind texture.")]
        public float windTextureScale = 1;
        [Tooltip("The time multiplier. Used when creating the wind texture's UV.")]
        public float windTimePeriod = 1;
        [Tooltip("The multplier to the world XZ position when creating the wind's UV.")]
        public float windScale = 1;
        [Tooltip("The maximum wind offset length")]
        public float windAmplitude = 0;
    }
    
    
    ///-/////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
	[ExecuteInEditMode]
    public class ProceduralGrassRenderer : MonoBehaviour
    {
        [Tooltip("The mesh to create grass from. Grass will root at every vertex on the mesh.")]
        [SerializeField] private Mesh sourceMesh;
        [SerializeField] private ComputeShader grassComputeShader;
        [SerializeField] private Material material;

        [SerializeField] private GrassSettings grassSettings = default;

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct SourceVertex
        {
            public Vector3 position;
        }

        // A state variable to track if compute buffers have been set up
        private bool computeBuffersInitialized = false;
        
        // Compute buffers
        private ComputeBuffer sourceVertBuffer;
        private ComputeBuffer sourceTriBuffer;
        private ComputeBuffer drawBuffer;
        private ComputeBuffer argsBuffer;
        private ComputeShader instantiatedGrassComputeShader;
        private Material instantiatedGrassMaterial;
        
        // ID of kernel in grass compute shader
        private int idGrassKernel;
        
        // The x dispatch size for the grass compute shader
        private int dispatchSize;
        
        // The local bounds of the generated mesh
        private Bounds localBounds;
        
        // Strides / size of one entry in the compute buffers
        private const int SOURCE_VERT_STRIDE = sizeof(float) * 3;
        private const int SOURCE_TRI_STRIDE = sizeof(int);
        private const int DRAW_STRIDE = sizeof(float) * (3 + (3 + 1) * 3);  // normal + (position + height) * 3 vertices
        private const int INDIRECT_ARGS_STRIDE = sizeof(int) * 4;

        // Reset args buffer every frame
        // 0: Vertex count per draw instance. Will only use one instance
        // 1: Instance count. Only one instance.
        // 2: Start vertex location if using a Graphics Buffer
        // 3: Start index location if using a Graphics Buffer
        private int[] argsBufferReset = new int[] { 0, 1, 0, 0 };

        ///-/////////////////////////////////////////////////////////////////////////////////////////////////
        /// 
        private void OnEnable()
        {
            Debug.Assert(grassComputeShader != null, "The grass compute shader is null.", gameObject);
            Debug.Assert(material != null, "The grass material is null.", gameObject);

            // If initialized, call OnDisable() for clean up
            if (computeBuffersInitialized)
            {
                OnDisable();
            }
            computeBuffersInitialized = true;
            
            // Instantiate shader so that each grass renderer has its own buffer
            instantiatedGrassComputeShader = Instantiate(grassComputeShader);
            instantiatedGrassMaterial = Instantiate(material);
            
            // Get data from source mesh
            Vector3[] positions = sourceMesh.vertices;
            int[] tris = sourceMesh.triangles;
            
            // Create the data to upload to the source vert buffer
            SourceVertex[] vertices = new SourceVertex[positions.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new SourceVertex()
                {
                    position = positions[i],
                };
            }
            int numSourceTriangles = tris.Length / 3;
            int maxBladeTriangles = (grassSettings.maxSegments - 1) * 2 + 1;
            
            // Create compute buffer
            sourceVertBuffer = new ComputeBuffer(vertices.Length, SOURCE_VERT_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            sourceVertBuffer.SetData(vertices);
            sourceTriBuffer = new ComputeBuffer(tris.Length, SOURCE_TRI_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            sourceTriBuffer.SetData(tris);
            drawBuffer = new ComputeBuffer(numSourceTriangles * maxBladeTriangles, DRAW_STRIDE, ComputeBufferType.Append);
            drawBuffer.SetCounterValue(0);
            argsBuffer = new ComputeBuffer(1, INDIRECT_ARGS_STRIDE, ComputeBufferType.IndirectArguments);
            
            // Cache kernel ID that is being dispatched
            idGrassKernel = grassComputeShader.FindKernel("Main");
            
            // Set buffers and other values on compute shader
            instantiatedGrassComputeShader.SetBuffer(idGrassKernel, "_SourceVertices", sourceVertBuffer);
            instantiatedGrassComputeShader.SetBuffer(idGrassKernel, "_SourceTriangles", sourceTriBuffer);
            instantiatedGrassComputeShader.SetBuffer(idGrassKernel, "_DrawTriangles", drawBuffer);
            instantiatedGrassComputeShader.SetBuffer(idGrassKernel, "_IndirectArgsBuffer", argsBuffer);
            instantiatedGrassComputeShader.SetInt("_NumSourceTriangles", numSourceTriangles);
            instantiatedGrassComputeShader.SetInt("_MaxBladeSegments", grassSettings.maxSegments);
            instantiatedGrassComputeShader.SetFloat("_MaxBendAngle", grassSettings.maxBendAngle);
            instantiatedGrassComputeShader.SetFloat("_BladeCurvature", grassSettings.bladeCurvature);
            instantiatedGrassComputeShader.SetFloat("_BladeHeight", grassSettings.bladeHeight);
            instantiatedGrassComputeShader.SetFloat("_BladeHeightVariance", grassSettings.bladeHeightVariance);
            instantiatedGrassComputeShader.SetFloat("_BladeWidth", grassSettings.bladeWidth);
            instantiatedGrassComputeShader.SetFloat("_BladeWidthVariance", grassSettings.bladeWidthVariance);
            instantiatedGrassComputeShader.SetTexture(idGrassKernel, "_WindNoiseTexture", grassSettings.windNoiseTexture);
            instantiatedGrassComputeShader.SetFloat("_WindTimeMultiplier", grassSettings.windTimePeriod);
            instantiatedGrassComputeShader.SetFloat("_WindTextureMultiplier", grassSettings.windTextureScale);
            instantiatedGrassComputeShader.SetFloat("_WindPositionMultiplier", grassSettings.windScale);
            instantiatedGrassComputeShader.SetFloat("_WindAmplitude", grassSettings.windAmplitude);
            
            // Send drawn triangles to the actual shader/material
            instantiatedGrassMaterial.SetBuffer("_DrawTriangles", drawBuffer);
            
            // Calculate num threads to use. Get thread size from kernel.
            // Divide num triangles by that size
            grassComputeShader.GetKernelThreadGroupSizes(idGrassKernel, out uint threadGroupSize, out _, out _);
            dispatchSize = Mathf.CeilToInt((float)numSourceTriangles / threadGroupSize);
            
            // Get the local bounds and expand it by the maximum blade width and height
            localBounds = sourceMesh.bounds;
            localBounds.Expand(1);
        }

        ///-/////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnDisable()
        {
            // Release the buffers and copied shaders
            if (computeBuffersInitialized)
            {
                // Destroy immediate or regular destroy depending on if the game is playing or not
                if (Application.isPlaying)
                {
                    Destroy(instantiatedGrassComputeShader);
                    Destroy(instantiatedGrassMaterial);
                }
                else
                {
                    DestroyImmediate(instantiatedGrassComputeShader);
                    DestroyImmediate(instantiatedGrassMaterial);
                }
                // Release each buffer
                sourceVertBuffer.Release();
                sourceTriBuffer.Release();
                drawBuffer.Release();
                argsBuffer.Release();
            }
            computeBuffersInitialized = false;
        }

        ///-/////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        /// Converts a bounds in local space to world space
        /// 
        private Bounds TransformBounds(Bounds boundsOS)
        {
            Vector3 center = transform.TransformPoint(boundsOS.center);
            
            // Convert extents to WS
            Vector3 extents = boundsOS.extents;
            Vector3 xAxis = transform.TransformVector(extents.x, 0, 0);
            Vector3 yAxis = transform.TransformVector(0, extents.y, 0);
            Vector3 zAxis = transform.TransformVector(0, 0, extents.z);
            
            // Sum absolute values to get the world extents
            extents.x = Mathf.Abs(xAxis.x) + Mathf.Abs(yAxis.x) + Mathf.Abs(zAxis.x);
            extents.y = Mathf.Abs(xAxis.y) + Mathf.Abs(yAxis.y) + Mathf.Abs(zAxis.y);
            extents.z = Mathf.Abs(xAxis.z) + Mathf.Abs(yAxis.z) + Mathf.Abs(zAxis.z);
            
            return new Bounds { center = center, extents = extents };
        }
        
        ///-/////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void LateUpdate()
        {
            // If in editor, update shaders each update to make sure settings are applied.
            if (Application.isPlaying == false)
            {
                OnDisable();
                OnEnable();
            }
            
            // Clear the draw and indirect args buffer of previous frame's data
            drawBuffer.SetCounterValue(0);
            argsBuffer.SetData(argsBufferReset);
            
            // Transform bounds to world space
            Bounds bounds = TransformBounds(localBounds);
            
            // Update shader with frame specific data
            instantiatedGrassComputeShader.SetVector("_Time", new Vector4(0, Time.timeSinceLevelLoad, 0, 0));
            instantiatedGrassComputeShader.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
            
            // Dispatch grass shader to run on GPU
            instantiatedGrassComputeShader.Dispatch(idGrassKernel, dispatchSize, 1, 1);
            
            // DrawProceduralIndirect queues a draw call up for the generated mesh
            Graphics.DrawProceduralIndirect(instantiatedGrassMaterial, bounds, MeshTopology.Triangles, argsBuffer,
                0, null, null,ShadowCastingMode.Off, true, gameObject.layer);
        }
    }
}
