using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace GAME
{
    public class EffectManager : MonoBehaviour
    {
        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====

        [SerializeField]
        private GameObject m_explosionEffect = null;
        #endregion //) ===== MEMBER_VARIABLES =====

        //------------------------------------------
        // 初期化周り
        //------------------------------------------
        #region ===== INITIALIZE =====

        void Awake()
        {
            var sys = World.Active.GetOrCreateSystem<EffectSystem>();
            sys.AddDeadEvent( this.OnEmitExplosionEffect );
        }

        #endregion //) ===== INITIALIZE =====

        private void OnEmitExplosionEffect( int _handler, Vector3 _worldPosition)
        {
            if( m_explosionEffect == null )
            {
                return;
            }

            Instantiate( m_explosionEffect, _worldPosition, Quaternion.identity );
        }
    }
}