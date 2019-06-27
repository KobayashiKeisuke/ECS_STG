using UnityEngine;
using Unity.Transforms;
using Unity.Entities;

namespace GAME
{
    using GAME.DATA;


    [UpdateAfter(typeof(BulletCollisionSystem))]
    public class EffectSystem : ComponentSystem
    {
        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====

        /* イベント */
        /// <summary> 死んだ時のイベント< Handler, WorldPosition> </summary>
        private System.Action<int, Vector3> m_onDeadEvent=null;
        /// <summary> 死んだ時のイベント< Handler, WorldPosition> </summary>
        private System.Action<int> m_onAddScoreEvent=null;

        /// <summary> EntityManager Cache </summary>
        private EntityManager m_entityManager=null;

        private EnemyComponentSystem m_enemySys = null;
        #endregion //) ===== MEMBER_VARIABLES =====

        //------------------------------------------
        // 初期化周り
        //------------------------------------------
        #region ===== INITIALIZE =====

        protected override void OnCreate()
        {
            m_entityManager = World.Active.EntityManager;

            m_enemySys = World.Active.GetOrCreateSystem<EnemyComponentSystem>();
        }

        #endregion //) ===== INITIALIZE =====

        public void AddDeadEvent( System.Action<int, Vector3> _e ){ m_onDeadEvent += _e;}
        public void RemoveDeadEvent( System.Action<int, Vector3> _e ){ m_onDeadEvent -= _e;}

        public void AddScoreEvent( System.Action<int> _e ){ m_onAddScoreEvent += _e;}
        public void RemoveScoreEvent( System.Action<int> _e ){ m_onAddScoreEvent -= _e;}


        // OnUpdate runs on the main thread.
        protected override void OnUpdate()
        {
            Entities.ForEach( ( ref EnemyData _enemy, ref Translation _pos )=>
            {
                if( _enemy.HP < 0 )
                {
                    m_onDeadEvent?.Invoke( _enemy.Id, _pos.Value );
                    m_onAddScoreEvent?.Invoke( _enemy.Score );
                    m_enemySys?.ReserveDestroyEntity( _enemy.Id );
                }
            });
        }


    }
}
