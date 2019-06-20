using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Entities;

namespace GAME.UI
{
    using GAME.INPUT;

    public class UISystemManager : Singleton<UISystemManager>
    {
        [SerializeField]
        private InputSystem m_inputSystem = null;
        [SerializeField]
        private Camera m_uiCamera = null;
        [SerializeField]
        private UIStickController m_stickCtrl = null;
        [SerializeField]
        private PlayerInstance m_player = null;
        [SerializeField]
        private GameObject m_basePrefab = null;
        [SerializeField]
        private GameObject m_stickPrefab = null;

        protected override void Init()
        {
            base.Init();

            // ECS の設定を呼ぶ
            EntityManager manager = World.Active.EntityManager;
            Entity e = manager.CreateEntity();
            manager.SetName( e, "InputDataEntity");
            manager.AddComponentData( e, new InputData() );

            var uiSystem = World.Active.GetOrCreateSystem<EcsUISystem>();
            uiSystem.Initialize( m_uiCamera, m_basePrefab, m_stickPrefab);
        }
    }
}
