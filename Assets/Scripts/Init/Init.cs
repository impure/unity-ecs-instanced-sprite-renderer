using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

/// <summary>
/// Illustrates that we can interact with ECS from the traditional monobehaviour model
/// </summary>
public class Init : MonoBehaviour {

	private int counter = 0;
	public static readonly Dictionary<Tuple<Mesh, Material>, List<Matrix4x4>> toDraw = new Dictionary<Tuple<Mesh, Material>, List<Matrix4x4>>();
	private const int numSprites = 3000;
	
	private void Start() {
		//QualitySettings.vSyncCount = 0;
		generate();
	}

	private void addDictionaryEntry(Texture2D texture, List<Matrix4x4> positions) {

		Mesh mesh = MeshUtils.GenerateQuad(texture.width, new float2(0.5f, 0.5f));
		Material material = new Material(Shader.Find("Sprites/Instanced")) {
			enableInstancing = true,
			mainTexture = texture
		};
		
		toDraw[new Tuple<Mesh, Material>(mesh, material)] = positions;
	}

	private void generate() {
        
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		//Load all the sprites we need
		Texture2D[] animalSprites = {
			AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/elephant.png"),
			AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/giraffe.png"),
			AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/zebra.png")
		};
		
		List<Matrix4x4> positions1 = new List<Matrix4x4>(1023);
		List<Matrix4x4> positions2 = new List<Matrix4x4>(1023);
		List<Matrix4x4> positions3 = new List<Matrix4x4>(1023);

		for (int i = 0; i < numSprites; i++) {
			if (i % 3 == 0) {
				positions1.Add(Matrix4x4.Translate(new Vector3(Random.value * 50, Random.value * 50, 0)));
			} else if (i % 3 == 1) {
				positions2.Add(Matrix4x4.Translate(new Vector3(Random.value * 50, Random.value * 50, 0)));
			} else {
				positions3.Add(Matrix4x4.Translate(new Vector3(Random.value * 50, Random.value * 50, 0)));
			}
		}
		addDictionaryEntry(animalSprites[0], positions1);
		addDictionaryEntry(animalSprites[1], positions2);
		addDictionaryEntry(animalSprites[2], positions3);
		stopwatch.Stop();
		Debug.Log("Instantiating objects took " + stopwatch.ElapsedMilliseconds + "ms.");
		
		/*
		stopwatch.Reset();
		stopwatch.Start();
        
		
		//Assign loaded sprites to sprite renderers
		SpriteInstanceRenderer[] renderers = {
			new SpriteInstanceRenderer(animalSprites[0], animalSprites[0].width, new float2(0.5f, 0.5f)),
			new SpriteInstanceRenderer(animalSprites[1], animalSprites[1].width, new float2(0.5f, 0.5f)),
			new SpriteInstanceRenderer(animalSprites[2], animalSprites[2].width, new float2(0.5f, 0.5f))
		};
		
		EntityManager entityManager = World.Active.GetOrCreateManager<EntityManager>();
		for (int i = 0; i < 3000; i++) {
			
			//SpriteInstanceRendererSystem.toRender.Add(renderers[i % 3]);
			
			Entity entity = entityManager.CreateEntity(ComponentType.Create<Position2D>(),
				ComponentType.Create<TransformMatrix>());

			entityManager.SetComponentData(entity, new Position2D {
				Value = new float2(Random.value * 50, Random.value * 25)
			});

			entityManager.AddSharedComponentData(entity, renderers[i % 3]);
			
		}
		
		stopwatch.Stop();
		Debug.Log("Instantiating objects took " + stopwatch.ElapsedMilliseconds + "ms.");
		*/
	}
}
