using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphRenderer : MonoBehaviour
{
  public Mesh cubeMesh; // Assign a cube mesh
  public Material cubeMaterial; // Assign the InstancedCubes material
  public int resolution = 10;
  private int instanceCount;

  private Matrix4x4[] matrices;
  private Vector4[] positions;
  private MaterialPropertyBlock materialPropertyBlock;

  [SerializeField]
  FunctionLibrary.FunctionName function;

  public enum TransitionMode { Cycle, Random }

  [SerializeField]
  TransitionMode transitionMode = TransitionMode.Cycle;

  [SerializeField, Min(0f)]
  float functionDuration = 1f, transitionDuration = 1f;
  float duration;

  bool transitioning;

  FunctionLibrary.FunctionName transitionFunction;

  void Start()
  {
    instanceCount = resolution * resolution;
    // Initialize data arrays
    matrices = new Matrix4x4[instanceCount];
    positions = new Vector4[instanceCount];
    materialPropertyBlock = new MaterialPropertyBlock();

    float scale = Mathf.Clamp(30.0f / Mathf.Sqrt(instanceCount), 0.1f, 1.0f);

    // Generate random positions
    for (int i = 0; i < instanceCount; i++)
    {
      Vector3 position = new Vector3(
          Random.Range(-10f, 10f),
          Random.Range(-10f, 10f),
          Random.Range(-10f, 10f)
      );

      positions[i] = new Vector4(position.x, position.y, position.z, 0);
      matrices[i] = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
    }

    // Set positions in material property block
    materialPropertyBlock.SetVectorArray("_InstancePosition", positions);
    cubeMaterial.SetFloat("_Scale", scale);
  }

  void Update()
  {
    UpdateTransition();
    UpdatePositions();
  }

  void UpdateTransition()
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
  }

  void PickNextFunction()
  {
    function = transitionMode == TransitionMode.Cycle ?
        FunctionLibrary.GetNextFunctionName(function) :
        FunctionLibrary.GetRandomFunctionNameOtherThan(function);
  }

  void UpdatePositions()
  {
    float t = Time.time; // Use time for animation
    int gridSize = Mathf.CeilToInt(Mathf.Sqrt(instanceCount));
    float spacing = 0.1f;

    FunctionLibrary.Function transitionFunction = FunctionLibrary.GetFunction(function);

    // Update positions with time-based movement
    for (int i = 0; i < instanceCount; i++)
    {
      int x = i % resolution;
      int z = i / resolution;

      float u = (x - resolution / 2) * spacing;
      float v = (z - resolution / 2) * spacing;

      Vector3 newPosition = transitionFunction(u, v, t);
      positions[i] = new Vector4(newPosition.x, newPosition.y, newPosition.z, 0);
      //new Vector4(Mathf.Sin(Time.time + i), 0, Mathf.Cos(Time.time + i), 0) * Time.deltaTime;
      matrices[i] = Matrix4x4.TRS(positions[i], Quaternion.identity, Vector3.one);
    }

    // Update material properties
    materialPropertyBlock.SetVectorArray("_InstancePosition", positions);

    // Draw all instances
    Graphics.DrawMeshInstanced(cubeMesh, 0, cubeMaterial, matrices, instanceCount, materialPropertyBlock);
  }
}
