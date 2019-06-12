using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputSystem : MonoBehaviour
{
    public InputAction mouseInput;  // マウスの入力
    public InputAction DragStart;   // ボタンを押したときのイベント
    public InputAction DragEnd;     // ボタンを離したときのイベント

    private void OnEnable()
    {
        mouseInput.Enable();
        DragStart.Enable();
        DragEnd.Enable();
    }

    private void OnDisable()
    {
        mouseInput.Disable();
        DragStart.Disable();
        DragEnd.Disable();
    }

    /// <summary>前回イベントに変化があった時間</summary>
    float m_prevTime;
    /// <summary>前回イベントに変化があった場所</summary>
    Vector2 m_prevScreenPos = Vector2.zero;
    /// <summary>現在のScreen座標系のposition</summary>
    Vector2 m_screenPosition;

    /// <summary> ScreenPosition, diffVec Time.realTime, deltaTime </summary>
    System.Action<Vector2, Vector2, float, float> m_onDragEvent = null;

    bool m_onPressed = false;

    private void Awake()
    {
        mouseInput.performed += _ =>
        {
            var value = _.ReadValue<Vector2>();
            float t = (float)_.time;
            m_screenPosition.x = value.x / Screen.width;
            m_screenPosition.y = value.y / Screen.height;

            Debug.Log($"[{this.GetType()}] Viewport:{m_screenPosition}, ScreenPos:{value},ScreenPos:({Screen.width}, {Screen.height}) ");
            if( m_onPressed )
            {
                m_onDragEvent?.Invoke( m_screenPosition, m_screenPosition - m_prevScreenPos, t, t - m_prevTime );
            }

            m_prevTime = t;
            m_prevScreenPos = m_screenPosition;
        };

        DragStart.performed += _ =>{ m_onPressed = true; m_prevTime = (float)_.time; };
        DragEnd.performed += _ =>{ m_onPressed = false; };

    }


    public void AddOnDragEvent(  System.Action<Vector2, Vector2, float, float> e){ m_onDragEvent += e; }
    public void RemoveOnDragEvent(  System.Action<Vector2, Vector2, float, float> e){ m_onDragEvent -= e; }
}
