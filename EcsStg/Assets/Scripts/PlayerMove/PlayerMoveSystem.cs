using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GAME
{
    public class PlayerMoveSystem : MonoBehaviour, IPlayerMove
    {
        //------------------------------------------
        // 定数関連
        //------------------------------------------
        #region ===== CONSTS =====

        /// <summary> 初期化パラメータ </summary>
        public class InitParameter 
        {
            /// <summary> 移動対象 </summary>
            public Transform MoveTarget;
            /// <summary> 描画カメラ </summary>
            public Camera MainCamera;

        }
        #endregion //)===== CONSTS =====

        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====

        [Header("操作パラメータ")]
        [SerializeField, Tooltip("移動速度[1/frame]")]
        private float m_maxMoveSpeed = 0.05f;
        protected float MaxMoveSpeed => m_maxMoveSpeed;

        [SerializeField, Tooltip("カメラからの距離")]
        private float m_cameraDistance = 40f;
        protected float CameraDistance => m_cameraDistance;

        Transform m_moveTarget;
        Transform MoveTarget => m_moveTarget;

        Camera m_mainCamera;
        Camera MainCam => m_mainCamera;


        float m_moveSpeed;
        public float MoveSpeed => m_moveSpeed;
        float m_moveDirection;
        public float MoveDirection => m_moveDirection;

        #endregion //) ===== MEMBER_VARIABLES =====

        //------------------------------------------
        // 初期化
        //------------------------------------------
        #region ===== INITIALIZE =====

        public void Initialize( InitParameter _param )
        {
            Debug.Assert( _param != null );
            Debug.Assert( _param?.MainCamera != null );
            Debug.Assert( _param?.MoveTarget != null );

            m_moveTarget = _param.MoveTarget;
            m_mainCamera = _param.MainCamera;
        }
        #endregion //) ===== INITIALIZE =====


        /// <summary>
        /// 極座標パラメータで入力を受け取る
        /// </summary>
        /// <param name="_range">正規化距離</param>
        /// <param name="_theta">角度[rad]</param>
        public void OnMove( float _range, float _theta )
        {
            m_moveSpeed = MaxMoveSpeed * _range;
            m_moveDirection =_theta;
        }

        void Update()
        {
            Vector3 dx = MainCam.transform.right * MoveSpeed* Mathf.Cos( MoveDirection );
            Vector3 dy = MainCam.transform.up * MoveSpeed * Mathf.Sin( MoveDirection );

            MoveTarget.position += dx + dy;
        }

    }
}