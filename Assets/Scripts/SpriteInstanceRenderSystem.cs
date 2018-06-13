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
	
	private ComponentGroup instanceRendererGroup;
	private const int gpuInstancingMagicNumber = 1023;

	/// <summary>
	/// Renders everything.
	/// </summary>
	protected override void OnUpdate() {

		foreach (var entry in Init.toDraw) {

			if (entry.Value.Count > gpuInstancingMagicNumber) {
				throw new Exception("Too many sprites! There are " + entry.Value.Count + " sprites!");
			}
			
			Graphics.DrawMeshInstanced(entry.Key.Item1, 0, entry.Key.Item2, entry.Value);
		}
		
	}
	
}