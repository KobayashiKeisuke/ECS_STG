using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using System.Collections.Generic;
using GAME.DATA;

namespace GAME
{
    [UpdateAfter(typeof(BulletComponentSystem))]
    public class BulletCollisionSystem : ComponentSystem
    {

        private Entity m_playerEntity;
        private List<EnemyData> enemyDataList = new List<EnemyData>();
        private List<Translation> enemyPosList = new List<Translation>();

        private EntityManager m_manager;
        private EnemyComponentSystem m_enemySys;

        private bool m_isInitialized = false;
        public void ResetSystem()
        {
            m_isInitialized = false;
            m_manager = null;
        }
        
        public void Initialize( Entity _playerEntity )
        {
            m_playerEntity = _playerEntity;
            m_manager = World.Active.EntityManager;
            m_enemySys = World.Active.GetOrCreateSystem<EnemyComponentSystem>();

            m_isInitialized = true;
        }

        // OnUpdate runs on the main thread.
        protected override void OnUpdate()
        {
            if( m_manager == null || !m_isInitialized )
            {
                return;
            }
            var Data = m_manager.GetComponentData<PlayerData>( m_playerEntity );
            var PlayerPos = m_manager.GetComponentData<Translation>( m_playerEntity );
            List<Entity> enemies = m_enemySys.GetCurrentAllEnemies();
            enemyDataList.Clear();
            enemyPosList.Clear();
            foreach (var e in enemies )
            {
                enemyDataList.Add( m_manager.GetComponentData<EnemyData>( e ));
                enemyPosList.Add( m_manager.GetComponentData<Translation>( e ) );
            }

            Entities.ForEach( ( ref Translation _pos, ref RenderBounds _bounds, ref BulletData _bullet )=>
            {
                // 多段ヒット防止
                if( _bullet.IsCollide )
                {
                    return;
                }
                switch( _bullet.BulletType )
                {
                    case 0:
                    {
                        for (int i = 0; i < enemyDataList.Count; i++)
                        {
                            var eData = enemyDataList[i];
                            EnemyCollision( ref eData, enemyPosList[i], ref _pos, ref _bounds, ref _bullet );
                            enemyDataList[i] = eData;
                        }
                    }break;
                    case 1:     PlayerCollision( ref Data, PlayerPos, ref _pos, ref _bounds, ref _bullet ); break;
                }
            });

            m_manager.SetComponentData( m_playerEntity, Data );
            for (int i = 0; i < enemyDataList.Count; i++)
            {
                m_manager.SetComponentData( enemies[i], enemyDataList[i] );
            }
        }

        private void PlayerCollision( ref PlayerData _playerData, Translation _targetPos, ref Translation _bulletPos, ref RenderBounds _bounds, ref BulletData _bullet)
        {
            float3 bound = _bounds.Value.Extents;
            if( _bulletPos.Value.x -bound.x <= _targetPos.Value.x && _targetPos.Value.x <= _bulletPos.Value.x +bound.x
            && _bulletPos.Value.y - bound.y <= _targetPos.Value.y && _targetPos.Value.y <= _bulletPos.Value.y + bound.y
            && _bulletPos.Value.z - bound.z <= _targetPos.Value.z && _targetPos.Value.z <= _bulletPos.Value.z + bound.z)
            {
                // バレットを消すように見せかける
                _bulletPos.Value.x = GameConst.SCREEN_WIDTH;
                _bulletPos.Value.z = GameConst.SCREEN_HEIGHT;
                _playerData.Life-= _bullet.Damage;
                _bullet.IsCollide = true;
            }
        }
        private void EnemyCollision( ref EnemyData _enemyData, Translation _targetPos, ref Translation _bulletPos, ref RenderBounds _bounds, ref BulletData _bullet)
        {
            float3 bound = _bounds.Value.Extents;
            if( _bulletPos.Value.x -bound.x <= _targetPos.Value.x && _targetPos.Value.x <= _bulletPos.Value.x +bound.x
            && _bulletPos.Value.y - bound.y <= _targetPos.Value.y && _targetPos.Value.y <= _bulletPos.Value.y + bound.y
            && _bulletPos.Value.z - bound.z <= _targetPos.Value.z && _targetPos.Value.z <= _bulletPos.Value.z + bound.z)
            {
                // バレットを消すように見せかける
                Debug.Log($"EnemyHit\nRange ({_bulletPos.Value.x -bound.x}, {_bulletPos.Value.x +bound.x})\n({_bulletPos.Value.y -bound.y}, {_bulletPos.Value.y +bound.y})\n({_bulletPos.Value.z -bound.z}, {_bulletPos.Value.z +bound.z})\n");
                _bulletPos.Value.x  = GameConst.SCREEN_WIDTH;
                _bulletPos.Value.z  = GameConst.SCREEN_HEIGHT;
                _enemyData.HP       -= _bullet.Damage;
                _bullet.IsCollide   = true;
            }
        }

    }
}
