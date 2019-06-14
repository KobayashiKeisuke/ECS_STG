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

        protected override void Init()
        {
            base.Init();

            m_stickCtrl?.Initialize( m_inputSystem, m_uiCamera, m_player.MoveSys);
        }
    }
}
