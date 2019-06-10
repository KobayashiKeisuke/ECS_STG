using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using GAME.Entity;

namespace GAME
{
    public class BulletFactory : ISpawner
    {
        /// <summary> 生成モデル </summary>
        GameObject m_prefab = null;

        /// <summary> 初期化チェックフラグ </summary>
        bool m_isInitialized = false;
        public bool IsInitialized => m_isInitialized;

        /// <summary> Bullet の初期値 </summary>
        BulletData m_initData;

        Transform m_initSpawnPos;

        public BulletFactory(
            GameObject _bulletPrefab,
            Transform _initPosition,
            float _speed, Vector3 _direction, int _damage )
        {
            m_prefab = _bulletPrefab;

            m_initData = new BulletData(){
                Speed = _speed,
                Direction = new Translation{ Value = new float3(_direction.x, _direction.y, _direction.z) },
                Damage = _damage,
            };

            m_initSpawnPos = _initPosition;

            m_isInitialized = true;

        }


        void ISpawner.Spawn( )
        {
            if( m_prefab == null || !IsInitialized )
            {
                return;
            }
            var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy( m_prefab, World.Active );
            var entityManager = World.Active.EntityManager;

            var instance = entityManager.Instantiate( prefab );
            Vector3 currentPos = m_initSpawnPos.position;
            entityManager.SetComponentData(instance, new Translation {Value = new float3(currentPos.x, currentPos.y, currentPos.z)});
            entityManager.SetComponentData<BulletData>( instance, m_initData);
        }
    }
}
