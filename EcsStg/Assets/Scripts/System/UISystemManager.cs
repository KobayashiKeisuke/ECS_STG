using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Entities;

namespace GAME.UI
{
    using GAME.INPUT;
    using GAME.DATA;

    public class UISystemManager : Singleton<UISystemManager>
    {
        // [SerializeField]
        // private InputSystem m_inputSystem = null;
        [SerializeField]
        private Camera m_uiCamera = null;
        [SerializeField]
        private HUDController m_hud = null;
        [SerializeField]
        private GameObject m_basePrefab = null;
        [SerializeField]
        private GameObject m_stickPrefab = null;

        EntityManager m_entityManager = null;
        EntityManager EntityManager { get { return m_entityManager ?? (m_entityManager = World.Active.EntityManager); } }

        Entity m_playerEntity;
        int m_prevLife = GameConst.DEFAULT_LIFE_COUNT;

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
        }

        public void SetPlayerEntity(Entity _e ){m_playerEntity = _e;}

        void Update()
        {
            var playerData = EntityManager.GetComponentData<PlayerData>( m_playerEntity );
            if( m_prevLife != playerData.Life)
            {
                m_hud.UpdateLifeText( playerData.Life);
                m_prevLife = playerData.Life;
            }
        }

        private void AddScore(int _score)
        {
            Debug.LogWarning($"ADD:{_score}");
            m_hud.AddScore( _score );
        }
    }
}
