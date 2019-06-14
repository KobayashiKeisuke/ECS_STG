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
    /// <summary> Time.realTime,  </summary>
    System.Action<float> m_onDragStartEvent = null;
    /// <summary> Time.realTime,  </summary>
    System.Action<float> m_onDragEndEvent = null;

    bool m_onPressed = false;

    private void Awake()
    {
        DragStart.performed += this.OnDragStart;
        mouseInput.performed += this.OnDrag;
        DragEnd.performed += this.OnDragEnd;


    }

    private void OnDragStart(InputAction.CallbackContext _context )
    {
        m_onPressed = true;
        float t = (float)_context.time;
        m_onDragStartEvent?.Invoke( t );
    }
    private void OnDrag(InputAction.CallbackContext _context )
    {
        var value = _context.ReadValue<Vector2>();
        float t = (float)_context.time;
        m_screenPosition.x = value.x / Screen.width;
        m_screenPosition.y = value.y / Screen.height;

        if( m_onPressed )
        {
            m_onDragEvent?.Invoke( m_screenPosition, m_screenPosition - m_prevScreenPos, t, t - m_prevTime );
        }

        m_prevTime = t;
        m_prevScreenPos = m_screenPosition;
    }
    private void OnDragEnd(InputAction.CallbackContext _context )
    {
        m_onPressed = false;
        float t = (float)_context.time;
        m_onDragEndEvent?.Invoke( t );
    }

    public void AddOnDragStartEvent(  System.Action<float> e){ m_onDragStartEvent += e; }
    public void RemoveOnDragStartEvent(  System.Action<float> e){ m_onDragStartEvent -= e; }
    public void AddOnDragEvent(  System.Action<Vector2, Vector2, float, float> e){ m_onDragEvent += e; }
    public void RemoveOnDragEvent(  System.Action<Vector2, Vector2, float, float> e){ m_onDragEvent -= e; }
    public void AddOnDragEndEvent(  System.Action<float> e){ m_onDragEndEvent += e; }
    public void RemoveOnDragEndEvent(  System.Action<float> e){ m_onDragEndEvent -= e; }
}
