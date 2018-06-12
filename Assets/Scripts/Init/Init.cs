using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Illustrates that we can interact with ECS from the traditional monobehaviour model
/// </summary>
public class Init : MonoBehaviour {

	void Start () {
        
		var entityManager = World.Active.GetOrCreateManager<EntityManager>();

		//Load all the sprites we need
		var animalSprites = new[] {
			AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/elephant.png"),
			AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/giraffe.png"),
			AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/zebra.png")
		};
        
		//Assign loaded sprites to sprite renderers
		var renderers = new[] {
			new SpriteInstanceRenderer(animalSprites[0], animalSprites[0].width, new float2(0.5f, 0.5f)),
			new SpriteInstanceRenderer(animalSprites[1], animalSprites[1].width, new float2(0.5f, 0.5f)),
			new SpriteInstanceRenderer(animalSprites[2], animalSprites[2].width, new float2(0.5f, 0.5f)),
		};
        
		//Spawn 10,000 entities with random positions/rotations
		for (int i = 0; i < 10000; i++) {
            
			var entity = entityManager.CreateEntity(ComponentType.Create<Position2D>(),
				ComponentType.Create<TransformMatrix>());

			entityManager.SetComponentData(entity, new Position2D {
				Value = new float2(Random.value * 50, Random.value * 25)
			});

			entityManager.AddSharedComponentData(entity, renderers[i % 3]);
		}
	}
}
