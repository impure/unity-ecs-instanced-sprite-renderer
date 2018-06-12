using System.Diagnostics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Illustrates that we can interact with ECS from the traditional monobehaviour model
/// </summary>
public class Init : MonoBehaviour {

	private int counter = 0;
	
	private void Start() {
		generate();
	}

	private void generate() {
        
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		
		EntityManager entityManager = World.Active.GetOrCreateManager<EntityManager>();

		//Load all the sprites we need
		Texture2D[] animalSprites = {
			AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/elephant.png"),
			AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/giraffe.png"),
			AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/zebra.png")
		};
        
		//Assign loaded sprites to sprite renderers
		SpriteInstanceRenderer[] renderers = {
			new SpriteInstanceRenderer(animalSprites[0], animalSprites[0].width, new float2(0.5f, 0.5f)),
			new SpriteInstanceRenderer(animalSprites[1], animalSprites[1].width, new float2(0.5f, 0.5f)),
			new SpriteInstanceRenderer(animalSprites[2], animalSprites[2].width, new float2(0.5f, 0.5f))
		};

		stopwatch.Stop();
		Debug.Log("Getting textures took " + stopwatch.ElapsedMilliseconds + "ms.");
		stopwatch.Reset();
		stopwatch.Start();
        
		for (int i = 0; i < 1000; i++) {
			
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
	}
}
