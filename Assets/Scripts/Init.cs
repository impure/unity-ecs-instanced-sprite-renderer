using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

/// <summary>
/// Illustrates that we can interact with ECS from the traditional monobehaviour model
/// </summary>
public class Init : MonoBehaviour {

	public static readonly Dictionary<Tuple<Mesh, Material>, List<Matrix4x4>> toDraw = new Dictionary<Tuple<Mesh, Material>, List<Matrix4x4>>();
	private const int numSprites = 3000;
	public static volatile bool listDone = false;
	private Random rng;
	private Tuple<Mesh, Material>[] keys;

	//Load all the sprites we need
	private Texture2D[] animalSprites;
	private int[] animalSpriteWidths;
	
	private void Start() {
		
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		
		//Load all the sprites we need
		animalSprites = new [] {
			AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/elephant.png"),
			AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/giraffe.png"),
			AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/zebra.png")
		};
		animalSpriteWidths = new [] {
			animalSprites[0].width,
			animalSprites[1].width,
			animalSprites[2].width
		};
		
		rng = new Random();
		
		QualitySettings.vSyncCount = 0;
		
		stopwatch.Stop();
		Debug.Log("Time to load stuff: " + stopwatch.ElapsedMilliseconds + "ms");
		
		generateECSThreadedMain();
		//generateECS();
		//generateNormal();
	}

	
	private float getRandom() {
		lock (this) {
			return (float)rng.NextDouble();
		}
	}


	/// <summary>
	/// Generates all the data the ECS system will use
	/// </summary>
	private void generateECSThreadedMain() {
		
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		
		//Load all the sprites we need
		keys = new [] {
			makeKey(animalSprites[0], animalSpriteWidths[0]),
			makeKey(animalSprites[1], animalSpriteWidths[1]),
			makeKey(animalSprites[2], animalSpriteWidths[2])
		};

		rng = new Random();
		
		QualitySettings.vSyncCount = 0;
		
		Thread thread = new Thread(generateECSThreadedThread);
		thread.Start();
		
		stopwatch.Stop();
		Debug.Log("Meshes: " + stopwatch.ElapsedMilliseconds + "ms");
	}


	/// <summary>
	/// Generates all the data the ECS system will use
	/// </summary>
	private void generateECSThreadedThread() {
        
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		
		// Starting the lists at size 1023 should help in theory but in practise doesn't really help.
		List<Matrix4x4> positions1 = new List<Matrix4x4>(1023);
		List<Matrix4x4> positions2 = new List<Matrix4x4>(1023);
		List<Matrix4x4> positions3 = new List<Matrix4x4>(1023);

		for (int i = 0; i < numSprites; i++) {
			if (i % 3 == 0) {
				positions1.Add(Matrix4x4.TRS(new Vector3((getRandom() - 0.5f) * 50, (getRandom() - 0.5f) * 50, i * 0.1f),
					Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)), Vector3.one * (1f / animalSpriteWidths[0])));
			} else if (i % 3 == 1) {
				positions2.Add(Matrix4x4.TRS(new Vector3((getRandom() - 0.5f) * 50, (getRandom() - 0.5f) * 50, i * 0.1f),
					Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)), Vector3.one * (1f / animalSpriteWidths[1])));
			} else {
				positions3.Add(Matrix4x4.TRS(new Vector3((getRandom()- 0.5f) * 50, (getRandom() - 0.5f) * 50, i * 0.1f),
					Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)), Vector3.one * (1f / animalSpriteWidths[2])));
			}
		}
		
		toDraw[keys[0]] = positions1;
		toDraw[keys[1]] = positions2;
		toDraw[keys[2]] = positions3;
		
		stopwatch.Stop();
		Debug.Log("Create: " + stopwatch.ElapsedMilliseconds + "ms.");
		listDone = true;
	}
	
	
	/// <summary>
	/// Generates all the data the ECS system will use
	/// </summary>
	private void generateECS() {
        
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		
		// Starting the lists at size 1023 should help in theory but in practise doesn't really help.
		List<Matrix4x4> positions1 = new List<Matrix4x4>(1023);
		List<Matrix4x4> positions2 = new List<Matrix4x4>(1023);
		List<Matrix4x4> positions3 = new List<Matrix4x4>(1023);

		for (int i = 0; i < numSprites; i++) {
			if (i % 3 == 0) {
				positions1.Add(Matrix4x4.TRS(new Vector3((getRandom() - 0.5f) * 50, (getRandom() - 0.5f) * 50, i * 0.1f),
					Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)), Vector3.one * (1f / animalSpriteWidths[0])));
			} else if (i % 3 == 1) {
				positions2.Add(Matrix4x4.TRS(new Vector3((getRandom() - 0.5f) * 50, (getRandom() - 0.5f) * 50, i * 0.1f),
					Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)), Vector3.one * (1f / animalSpriteWidths[1])));
			} else {
				positions3.Add(Matrix4x4.TRS(new Vector3((getRandom()- 0.5f) * 50, (getRandom() - 0.5f) * 50, i * 0.1f),
					Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)), Vector3.one * (1f / animalSpriteWidths[2])));
			}
		}
		
		addDictionaryEntry(animalSprites[0], animalSpriteWidths[0], positions1);
		addDictionaryEntry(animalSprites[1], animalSpriteWidths[1], positions2);
		addDictionaryEntry(animalSprites[2], animalSpriteWidths[2], positions3);
		
		stopwatch.Stop();
		Debug.Log("Creating meshes took " + stopwatch.ElapsedMilliseconds + "ms.");
		listDone = true;
	}


	private void addDictionaryEntry(Texture2D texture, int width, List<Matrix4x4> positions) {

		Mesh mesh = MeshUtils.GenerateQuad(width, new Vector2(0.5f, 0.5f));
		Material material = new Material(Shader.Find("Sprites/Instanced")) {
			enableInstancing = true,
			mainTexture = texture
		};
		
		toDraw[new Tuple<Mesh, Material>(mesh, material)] = positions;
	}

	
	private Tuple<Mesh, Material> makeKey(Texture2D texture, int width) {
		
		Mesh mesh = MeshUtils.GenerateQuad(width, new Vector2(0.5f, 0.5f));
		Material material = new Material(Shader.Find("Sprites/Instanced")) {
			enableInstancing = true,
			mainTexture = texture
		};

		return new Tuple<Mesh, Material>(mesh, material);
	}
	

	/// <summary>
	/// Generates all the data the ECS system will use
	/// </summary>
	private void generateNormal() {
        
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		for (int i = 0; i < numSprites; i++) {
			if (i % 3 == 0) {
				Instantiate(Resources.Load("Sprite1"), 
					new Vector3((getRandom() - 0.5f) * 50, (getRandom() - 0.5f) * 50, i * 0.1f), Quaternion.identity);
			} else if (i % 3 == 1) {
				Instantiate(Resources.Load("Sprite2"), 
					new Vector3((getRandom() - 0.5f) * 50, (getRandom() - 0.5f) * 50, i * 0.1f), Quaternion.identity);
			} else {
				Instantiate(Resources.Load("Sprite3"), 
					new Vector3((getRandom() - 0.5f) * 50, (getRandom() - 0.5f) * 50, i * 0.1f), Quaternion.identity);
			}
		}
		stopwatch.Stop();
		Debug.Log("Instantiating objects took " + stopwatch.ElapsedMilliseconds + "ms.");
	}
}
