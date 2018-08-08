﻿using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour
{
	float deltaTime = 0.0f;
	public Font font;

	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h);
		style.font = font;
		style.alignment = TextAnchor.LowerRight;
		style.fontSize = h * 4 / 100;
		style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

		GUI.Label(rect, text, style);
	}
}