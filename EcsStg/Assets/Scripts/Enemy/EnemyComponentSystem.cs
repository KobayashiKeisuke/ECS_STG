using UnityEngine;
using Unity.Transforms;
using Unity.Entities;

using System.Collections.Generic;

namespace GAME
{
    using GAME.DATA;

    [UpdateAfter(typeof(EffectSystem))]
    public class EnemyComponentSystem : ComponentSystem
    {
        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====

        Dictionary<int, Entity> m_allEnemyList = new Dictionary<int, Entity>();
        private int m_handler =0;

        /// <summary> EntityManager Cache </summary>
        private EntityManager m_entityManager=null;

        private Queue<int> m_reserveDestroyIdQueue= new Queue<int>();

        Dictionary<int, Queue<Entity>> m_allEnemyBulletFactoryList = new Dictionary<int, Queue<Entity>>(); 
        #endregion //) ===== MEMBER_VARIABLES =====

        //------------------------------------------
        // 初期化周り
        //------------------------------------------
        #region ===== INITIALIZE =====

        protected override void OnCreate()
        {
            m_entityManager = World.Active.EntityManager;
        }

        #endregion //) ===== INITIALIZE =====

        public int GetInstanceId(){ return m_handler;}

        public void AddEnemyEntity( int _instanceId, Entity _e ){ m_allEnemyList.Add( _instanceId, _e ); m_handler++; }

        /// <summary>
        /// Enemy とBulletFactoryの関係性を追加
        /// </summary>
        /// <param name="_instanceId"></param>
        /// <param name="_e"></param>
        public void AddBulletFactoryRelation( int _instanceId, Entity _e )
        {
            if( m_allEnemyBulletFactoryList.TryGetValue( _instanceId, out Queue<Entity> queue) )
            {
                queue.Enqueue( _e );
                m_allEnemyBulletFactoryList[_instanceId] = queue;
            }
            else
            {
                var newQueue = new Queue<Entity>();
                newQueue.Enqueue( _e );
                m_allEnemyBulletFactoryList.Add( _instanceId, newQueue);
            }
        }

        public List<Entity> GetCurrentAllEnemies()
        {
            List<Entity> list = new List<Entity>(m_allEnemyList.Count);
            foreach (var item in m_allEnemyList)
            {
                list.Add(item.Value );
            }
            return list;
        }

        /// <summary>
        /// 不要Entityの破棄
        /// </summary>
        /// <param name="_instanceId"></param>
        public void ReserveDestroyEntity( int _instanceId )
        {
            m_reserveDestroyIdQueue.Enqueue( _instanceId );
        }

        // OnUpdate runs on the main thread.
        protected override void OnUpdate()
        {
            while( m_reserveDestroyIdQueue.Count > 0 )
            {
                int id = m_reserveDestroyIdQueue.Dequeue();
                if( m_allEnemyList.TryGetValue( id, out Entity e) )
                {
                    Debug.LogWarning($"DestroyEntity:{id}, Name:{m_entityManager.GetName(e)}");
                    m_allEnemyList.Remove( id );
                    m_entityManager.DestroyEntity( e );
                }
                // 関連するBullet Factory も削除
                if( m_allEnemyBulletFactoryList.TryGetValue( id, out Queue<Entity> queue) )
                {
                    while( queue.Count > 0 )
                    {
                        m_entityManager.DestroyEntity( queue.Dequeue() );
                    }
                    m_allEnemyBulletFactoryList.Remove(id);
                }
            }
        }


    }
}
