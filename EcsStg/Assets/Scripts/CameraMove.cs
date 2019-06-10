using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraMove : MonoBehaviour
{
    enum CAM_TYPE
    {
        /// <summary> X-Y 平面描画カメラ </summary>
        Z_ANGLE,
        /// <summary> X-Z 平面描画カメラ </summary>
        Y_ANGLE,
    }

    [System.Serializable]
    public class CameraParam
    {
        /// <summary> 位置</summary>
        public Vector3 Position;
        /// <summary> 回転 </summary>
        public Vector3 Rotation;
    }

    /// <summary> 周期[sec] </summary>
    [SerializeField, Range( 0.0f, 10.0f)]
    private float m_cycle = 5.0f;
    public float Cycle => m_cycle;

    /// <summary> 移動時間[sec] </summary>
    [SerializeField, Range( 0.1f, 10.0f)]
    private float m_moveTime = 1.0f;
    public float MoveTime => m_moveTime;

    /// <summary> X-Y平面 </summary>
    [SerializeField]
    private CameraParam m_z_angleParam;
    public CameraParam Z_angleParam => m_z_angleParam;

    /// <summary> X-Z平面 </summary>
    [SerializeField]
    private CameraParam m_y_angleParam;
    public CameraParam Y_angleParam => m_y_angleParam;
    

    private float m_rotationTimer = 0.0f;
    private bool m_isTweening = false;

    private CAM_TYPE CamType = CAM_TYPE.Z_ANGLE;

    void Start()
    {
        m_rotationTimer = Cycle;
    }

    // Update is called once per frame
    void Update()
    {
        if( m_isTweening )
        {
            return;
        }
        float deltaTime = Time.deltaTime;

        m_rotationTimer -= deltaTime;

        if( m_rotationTimer < 0 )
        {
            m_rotationTimer = Cycle;
            m_isTweening = true;
            SetCameraMoveSettings();
        }
    }

    void SetCameraMoveSettings()
    {
        Vector3 nextPos = Vector3.zero, nextRot=Vector3.zero;
        switch( CamType )
        {
            case CAM_TYPE.Z_ANGLE:
            {
                nextPos = Y_angleParam.Position;
                nextRot = Y_angleParam.Rotation;

                CamType = CAM_TYPE.Y_ANGLE;
            }
            break;
            case CAM_TYPE.Y_ANGLE:
            {
                nextPos = Z_angleParam.Position;
                nextRot = Z_angleParam.Rotation;

                CamType = CAM_TYPE.Z_ANGLE;
            }
            break;
        }

        this.transform.DOMove( nextPos, MoveTime, false ).OnComplete( ()=>{ m_isTweening = false; } );
        this.transform.DORotate( nextRot, MoveTime );
    }
}
