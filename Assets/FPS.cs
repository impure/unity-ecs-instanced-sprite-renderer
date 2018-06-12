using UnityEngine;

public class FPS : MonoBehaviour {
	private float deltaTime = 0.0f;
	private float approxSquaredDeviation = 0.0f;
	private bool firstCall = true;


	void Update() {
		approxSquaredDeviation += (Mathf.Pow(Time.unscaledDeltaTime - deltaTime, 2) - approxSquaredDeviation) * 0.05f;
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

		if (firstCall) {
			firstCall = false;
			Debug.Log("First frame time: " + Time.unscaledDeltaTime);
		}
    }


    void OnGUI() {

        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2f / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0f, 0f, 0f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps) ({2:0.}% sd) Developer build.", msec, fps, 100 * Mathf.Sqrt(approxSquaredDeviation)/deltaTime);
        GUI.Label(rect, text, style);
    }
}
