using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Renders all Entities containingå both SpriteInstanceRenderer & TransformMatrix components.
/// </summary>
public class SpriteInstanceRendererSystem : ComponentSystem {
	
	private readonly Dictionary<SpriteInstanceRenderer, Material> materialCache = new Dictionary<SpriteInstanceRenderer, Material>();
	private readonly Dictionary<SpriteInstanceRenderer, Mesh> meshCache = new Dictionary<SpriteInstanceRenderer, Mesh>();

	// Instance renderer takes only batches of 1023
	private readonly Matrix4x4[] matricesArray = new Matrix4x4[1023];
	private readonly List<SpriteInstanceRenderer> cacheduniqueRendererTypes = new List<SpriteInstanceRenderer>();
	private ComponentGroup instanceRendererGroup;
	
	public static List<SpriteInstanceRenderer> toRender = new List<SpriteInstanceRenderer>();

	/// <summary>
	/// Gets a container for all of our meshes and transforms
	/// </summary>
	protected override void OnCreateManager(int capacity) {
		instanceRendererGroup = GetComponentGroup(typeof(SpriteInstanceRenderer), typeof(TransformMatrix));
	}


	/// <summary>
	/// Renders everything.
	/// </summary>
	protected override void OnUpdate() {
		
		drawOriginal();
	}


	private void drawOriginal() {
		
		EntityManager.GetAllUniqueSharedComponentDatas(cacheduniqueRendererTypes);
		int numNull = 0;
		
		int beginIndex = 0;
		var transforms = instanceRendererGroup.GetComponentDataArray<TransformMatrix>();
		while (beginIndex < transforms.Length) {
			int length = math.min(matricesArray.Length, transforms.Length - beginIndex);
			copyMatrices(transforms, beginIndex, length, matricesArray);

			beginIndex += length;
		}
		
		for (int i = 0; i < 2; i++) {
			
			SpriteInstanceRenderer renderer = cacheduniqueRendererTypes[i];
			
			// The first sprite in the array is null for some reason
			if (renderer.sprite == null) {
				Debug.Log("Null sprite " + i);
				numNull++;
				continue;
			}

			instanceRendererGroup.SetFilter(renderer);

			// Draw this mesh at the given position
			Mesh mesh;
			Material material;
			float size = math.max(renderer.sprite.width, renderer.sprite.height) / (float) renderer.pixelsPerUnit;
			float2 meshPivot = renderer.pivot * size;
			
			// All renderers share one mesh
			if (!meshCache.TryGetValue(renderer, out mesh)) {
				mesh = MeshUtils.GenerateQuad(size, meshPivot);
				meshCache.Add(renderer, mesh);
			}

			// All renderers share one material
			if (!materialCache.TryGetValue(renderer, out material)) {

				material = new Material(Shader.Find("Sprites/Instanced")) {
					enableInstancing = true,
					mainTexture = renderer.sprite
				};

				materialCache.Add(renderer, material);
			}

			// We can only batch 1023 things at once. So keep on batching 1023 things and then batch the remainder.
			Graphics.DrawMeshInstanced(mesh, 0, material, matricesArray);
			//Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
			/*
			int beginIndex = 0;
			while (beginIndex < transforms.Length) {
				int length = math.min(matricesArray.Length, transforms.Length - beginIndex);
				copyMatrices(transforms, beginIndex, length, matricesArray);
				Graphics.DrawMeshInstanced(mesh, 0, material, matricesArray);

				beginIndex += length;
			}
			*/
		}

		cacheduniqueRendererTypes.Clear();
		
	}


	/// <summary>
	/// This copies a 4x4 matrix really fast. How does it work? No one knows.
	/// </summary>
	private static unsafe void copyMatrices(ComponentDataArray<TransformMatrix> transforms, 
		int beginIndex, int length, Matrix4x4[] outMatrices) {
		
		// @TODO: This is using unsafe code because the Unity DrawInstances API takes a Matrix4x4[] instead of NativeArray.
		// We want to use the ComponentDataArray.CopyTo method
		// because internally it uses memcpy to copy the data,
		// if the nativeslice layout matches the layout of the component data. It's very fast...
		fixed (Matrix4x4* matricesPtr = outMatrices) {
			
			if (sizeof(Matrix4x4) != sizeof(TransformMatrix)) {
				throw new Exception("Unsafe copying: matrix sizes are not equal");
			}
			
			var matricesSlice = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<TransformMatrix>(matricesPtr, sizeof(Matrix4x4), length);
			
			#if ENABLE_UNITY_COLLECTIONS_CHECKS
			NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref matricesSlice, AtomicSafetyHandle.GetTempUnsafePtrSliceHandle());
			#endif
			
			transforms.CopyTo(matricesSlice, beginIndex);
		}
	}
}