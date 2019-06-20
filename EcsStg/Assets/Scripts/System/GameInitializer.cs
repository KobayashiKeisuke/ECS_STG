using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

using GAME;

    public class GameInitializer : MonoBehaviour
    {
        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====
        [Header("カメラ")]
        [SerializeField]
        private Camera m_mainCam = null;
        #endregion //) ===== MEMBER_VARIABLES =====

        //------------------------------------------
        // 初期化
        //------------------------------------------
        #region ===== INITIALIZE =====
        void Awake()
        {
            var system = World.Active.GetOrCreateSystem<PlayerComponentSystem>();
            system.Initialize( m_mainCam );
        }

        #endregion //) ===== INITIALIZE =====
    }
