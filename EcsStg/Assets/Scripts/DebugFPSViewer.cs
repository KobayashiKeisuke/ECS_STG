using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugFPSViewer : MonoBehaviour
{
    const int FONT_SIZE = 30;

    int m_prevFps = 0;
    int m_frameCount = 0;
    float m_elapsedTime = 0.0f;


    Vector2 size;
    GUIStyle m_style = new GUIStyle();
    void Awake()
    {
        size.x = Screen.width;
        size.y = Screen.height;

        m_style.fontSize = FONT_SIZE;
    }

    void Update()
    {
        m_elapsedTime += Time.deltaTime;
        m_frameCount++;

        if( m_elapsedTime > 1.0f )
        {
            m_prevFps = Mathf.RoundToInt( m_frameCount / m_elapsedTime );
            m_frameCount = 0;
            m_elapsedTime = 0.0f;
        }
    }

    void OnGUI()
    {
        GUILayout.Label($"FPS:{ m_prevFps}", m_style, GUILayout.Width( size.x * 0.3f), GUILayout.Height( size.y * 0.1f));
    }
}
