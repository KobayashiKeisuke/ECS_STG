using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GAME.UI
{

    public class UISystemManager : Singleton<UISystemManager>
    {
        [SerializeField]
        private InputSystem m_inputSystem;
        [SerializeField]
        private Camera m_uiCamera;
        [SerializeField]
        private UIStickController m_stickCtrl;

        protected override void Init()
        {
            base.Init();

            m_stickCtrl?.Initialize( m_inputSystem, m_uiCamera);
        }
    }
}
