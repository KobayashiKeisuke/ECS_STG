using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Entities;

namespace GAME.UI
{
    using GAME.INPUT;
    using GAME.DATA;
    using GAME.INGAME.UI;

    public class UISystemManager : Singleton<UISystemManager>
    {
        enum STATE
        {
            PLAY,
            GAME_OVER,

        }

        // [SerializeField]
        // private InputSystem m_inputSystem = null;
        [SerializeField]
        private Camera m_uiCamera = null;
        [SerializeField]
        private Transform m_canvasRoot = null;
        [SerializeField]
        private HUDController m_hud = null;
        [SerializeField]
        private GameObject m_basePrefab = null;
        [SerializeField]
        private GameObject m_stickPrefab = null;
        [SerializeField]
        private GameObject m_gameOverDialog = null;


        EntityManager m_entityManager = null;
        EntityManager EntityManager { get { return m_entityManager ?? (m_entityManager = World.Active.EntityManager); } }

        Entity m_playerEntity;
        int m_prevLife = GameConst.DEFAULT_LIFE_COUNT;

        STATE m_state = STATE.PLAY;

        private int m_currentScore = 0;

        protected override void Init()
        {
            base.Init();

            // ECS の設定を呼ぶ
            Entity e = EntityManager.CreateEntity();
            #if UNITY_EDITOR
            EntityManager.SetName( e, "InputDataEntity");
            #endif
            EntityManager.AddComponentData( e, new InputData() );

            var uiSystem = World.Active.GetOrCreateSystem<EcsUISystem>();
            uiSystem.Initialize( m_uiCamera, m_basePrefab, m_stickPrefab);

            m_hud.Initialize( GameConst.DEFAULT_LIFE_COUNT );

            var effectSystem = World.Active.GetOrCreateSystem<EffectSystem>();
            effectSystem.AddScoreEvent( this.AddScore );
            effectSystem.AddGameOverEvent( this.OnGameOver );

            SetPlayerEntity( GameInitializer.I.PlayerEntity );
        }


        public void SetPlayerEntity(Entity _e ){m_playerEntity = _e;}

        void Update()
        {
            if( EntityManager == null || !GameInitializer.I.IsInitialized )
            {
                return;
            }
            var playerData = EntityManager.GetComponentData<PlayerData>( m_playerEntity );
            if( m_prevLife != playerData.Life)
            {
                m_hud.UpdateLifeText( playerData.Life);
                m_prevLife = playerData.Life;
            }
        }

        private void AddScore(int _score)
        {
            m_currentScore += _score;
            m_hud.AddScore( m_currentScore );
        }

        private void OnGameOver( int _handler, Vector3 _deathPosition)
        {
            if( m_state == STATE.GAME_OVER )
            {
                return;
            }

            m_state = STATE.GAME_OVER;

            var go = (GameObject)Instantiate( m_gameOverDialog, m_canvasRoot );
            var ctrl = go.GetComponent<GameOverDialogController>();
            ctrl.Initialize(m_currentScore);
        }
    }
}
