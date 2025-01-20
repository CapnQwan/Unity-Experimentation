using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    const int maxResolution = 1000;


    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;

    [SerializeField, Range(10, maxResolution)]
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    TransitionMode transitionMode = TransitionMode.Cycle;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    //static readonly int 
    //    positionsId = Shader.PropertyToID("_Positions"),
    //    resolutionId = Shader.PropertyToID("_Resolution"),
    //	  stepId = Shader.PropertyToID("_Step"),
    //	  timeId = Shader.PropertyToID("_Time"),
    //    transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    float duration;

    bool transitioning;

    FunctionLibrary.FunctionName transitionFunction;

    ComputeBuffer positionsBuffer;

    void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }

    void Update()
    {
        duration += Time.deltaTime;

        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }

        UpdateFunctionOnGPU();
    }

    void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;

        Debug.Log(resolution);
        Debug.Log(step);
        Debug.Log((int)transitionMode);

        // Pass parameters to the material
        material.SetInt("_Resolution", resolution);
        material.SetFloat("_Step", step);
        material.SetInt("_Function", (int)transitionMode);

        // Define the bounds of the rendering area
        var bounds = new Bounds(Vector3.zero, Vector3.one * 2f);

        // Total number of instances = resolution * resolution
        int instanceCount = resolution * resolution;

        // Draw the instanced mesh
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, instanceCount);
    }

    void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunctionName(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }
}
