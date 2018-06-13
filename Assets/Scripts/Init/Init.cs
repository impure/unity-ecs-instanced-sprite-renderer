using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

/// <summary>
/// Illustrates that we can interact with ECS from the traditional monobehaviour model
/// </summary>
public class Init : MonoBehaviour {

	public static readonly Dictionary<Tuple<Mesh, Material>, List<Matrix4x4>> toDraw = new Dictionary<Tuple<Mesh, Material>, List<Matrix4x4>>();
	private const int numSprites = 3000;
	
	private void Start() {
		//QualitySettings.vSyncCount = 0;
		generate();
	}


	private void addDictionaryEntry(Texture2D texture, List<Matrix4x4> positions) {

		Mesh mesh = MeshUtils.GenerateQuad(texture.width, new Vector2(0.5f, 0.5f));
		Material material = new Material(Shader.Find("Sprites/Instanced")) {
			enableInstancing = true,
			mainTexture = texture
		};
		
		toDraw[new Tuple<Mesh, Material>(mesh, material)] = positions;
	}


	/// <summary>
	/// Generates all the data the ECS system will use
	/// </summary>
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
				positions1.Add(Matrix4x4.TRS(new Vector3((Random.value - 0.5f) * 50, (Random.value - 0.5f) * 50, i * 0.1f),
					Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)), Vector3.one * (1f / animalSprites[2].width)));
			} else if (i % 3 == 1) {
				positions2.Add(Matrix4x4.TRS(new Vector3((Random.value - 0.5f) * 50, (Random.value - 0.5f) * 50, i * 0.1f),
					Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)), Vector3.one * (1f / animalSprites[2].width)));
			} else {
				positions3.Add(Matrix4x4.TRS(new Vector3((Random.value - 0.5f) * 50, (Random.value - 0.5f) * 50, i * 0.1f),
					Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)), Vector3.one * (1f / animalSprites[2].width)));
			}
		}
		
		addDictionaryEntry(animalSprites[0], positions1);
		addDictionaryEntry(animalSprites[1], positions2);
		addDictionaryEntry(animalSprites[2], positions3);
		stopwatch.Stop();
		Debug.Log("Instantiating objects took " + stopwatch.ElapsedMilliseconds + "ms.");
	}
}
