using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using GAME.Entity;

namespace GAME
{
    public class BulletFactory : ISpawner
    {
        const int FPS = 30;

        /// <summary> 生成モデル </summary>
        GameObject m_prefab = null;

        /// <summary> 初期化チェックフラグ </summary>
        bool m_isInitialized = false;
        public bool IsInitialized => m_isInitialized;

        /// <summary> Bullet の初期値 </summary>
        BulletData m_initData;

        Transform m_initSpawnPos;

        Unity.Entities.Entity[] m_entities;
        public BulletFactory(
            GameObject _bulletPrefab,
            Transform _initPosition,
            float _cycle,
            float _speed, Vector3 _direction, int _damage )
        {
            m_prefab = _bulletPrefab;

            m_initData = new BulletData()
            {
                IsInitialized = true,
                Speed = _speed,
                Direction = new Translation{ Value = new float3(_direction.x, _direction.y, _direction.z) },
                Damage = _damage,
            };

            m_initSpawnPos = _initPosition;

            float lifeTime = 40.0f / (_speed * FPS) / _direction.magnitude;
            int totalSetPerLifeTime = Mathf.FloorToInt( lifeTime / _cycle ) + 1;
            Debug.Log($"Speed:{_speed}, Dir:{_direction}\nLifeTime:{lifeTime}, SetCount:{totalSetPerLifeTime}");
            PreloadObject( totalSetPerLifeTime );

            m_isInitialized = true;
        }

        void PreloadObject(int _instanceCount )
        {
            m_entities = new Unity.Entities.Entity[_instanceCount];
            const float INIT_POS = 10000f;

            var entityManager = World.Active.EntityManager;
            var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy( m_prefab, World.Active );
            for (int i = 0; i < _instanceCount; i++)
            {
                var instance = entityManager.Instantiate( prefab );
                entityManager.SetComponentData(instance, new Translation {Value = new float3(INIT_POS,INIT_POS,INIT_POS)});
                entityManager.SetComponentData<BulletData>( instance, new BulletData(){IsInitialized = false} );

                m_entities[i] = instance;
            }
        }


        void ISpawner.Spawn( )
        {
            if( !IsInitialized )
            {
                return;
            }
            var entityManager = World.Active.EntityManager;
            Vector3 currentPos = m_initSpawnPos.position;

            for (int i = 0; i < m_entities.Length; i++)
            {
                BulletData data = entityManager.GetComponentData<BulletData>( m_entities[i]);
                if( !data.IsInitialized )
                {
                    entityManager.SetComponentData(m_entities[i], new Translation{Value = new float3(currentPos.x, currentPos.y, currentPos.z)});
                    entityManager.SetComponentData<BulletData>( m_entities[i], m_initData);
                    break;
                }
            }
        }
    }
}
