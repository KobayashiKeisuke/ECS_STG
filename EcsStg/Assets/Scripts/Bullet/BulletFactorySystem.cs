using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GAME
{
    using GAME.DATA;

    /// <summary>
    /// 弾幕生成エンジン
    /// </summary>
    [UpdateBefore(typeof(BulletComponentSystem))]
    public class BulletFactorySystem : ComponentSystem
    {
        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====
        
        protected EntityManager m_entityManager;

        protected Dictionary<int, List<Entity>> m_bulletsList = new Dictionary< int, List<Entity>>();
        #endregion //) ===== MEMBER_VARIABLES =====

        //------------------------------------------
        // 初期化
        //------------------------------------------
        #region ===== INITIALIZE =====

        protected override void OnCreate()
        {
            m_entityManager = World.Active.EntityManager;

        }
        #endregion //) ===== INITIALIZE =====

        // OnUpdate runs on the main thread.
        protected override void OnUpdate()
        {
            float dt = Time.deltaTime;
            Debug.Assert( this.m_entityManager != null );
            Debug.Assert( this.m_bulletsList != null );

            Entities.ForEach( ( ref BulletFactoryData factoryData )=>
            {
                factoryData.SpawnTimer -= dt;
                if( factoryData.SpawnTimer < 0 )
                {
                    factoryData.SpawnTimer = factoryData.SpawnCycle;

                    Translation currentWorldPos = m_entityManager.GetComponentData<Translation>( factoryData.ParentEntity );
                    ObjectMoveData moveData = new ObjectMoveData(){ Speed = factoryData.Speed, Direction = new Translation(){ Value = factoryData.MoveDirection} };
                    m_bulletsList.TryGetValue( factoryData.BulletListHandler, out var list );

                    Spawn( m_entityManager, list, currentWorldPos.Value, factoryData.Damage, factoryData.BulletType, moveData );
                }
            });
        }

        public void Spawn( EntityManager _entityManager, List<Entity> list, float3 _parentPos, int _damage, int _bulletType, ObjectMoveData moveData )
        {
            Debug.Assert( list != null );
            if( list == null )
            {
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                BulletData bulletInfo = _entityManager.GetComponentData<BulletData>( list[i]);
                if( !bulletInfo.IsInitialized )
                {
                    bulletInfo.IsInitialized    = true;
                    bulletInfo.IsCollide        = false;
                    bulletInfo.Damage           = _damage;
                    bulletInfo.BulletType       = _bulletType;
                    _entityManager.SetComponentData(list[i], new Translation{Value = _parentPos});
                    _entityManager.SetComponentData<ObjectMoveData>( list[i], moveData );
                    _entityManager.SetComponentData<BulletData>( list[i], bulletInfo);
                    break;
                }
            }
        }

        /// <summary>
        /// 事前に生成するオブジェクト数を計算
        /// </summary>
        /// <param name="ScreenLongerSize">画面の最大長</param>
        /// <param name="moveSpeed">バレットの移動速度</param>
        /// <param name="moveDirSize">移動方向のノルム</param>
        /// <param name="spawnCycle">生成間隔[sec]</param>
        /// <returns></returns>
        public static int CalcPreloadObjectCount( float ScreenLongerSize, float moveSpeed, float moveDirSize, float spawnCycle )
        {
            float lifeTime = ScreenLongerSize / ( moveSpeed * GameConst.FPS) / moveDirSize;
            return Mathf.FloorToInt( lifeTime /spawnCycle ) + 1;
        }

        /// <summary>
        /// バレットリストのPreLoad
        /// </summary>
        /// <param name="_instanceCount"></param>
        /// <param name="_prefab"></param>
        /// <returns></returns>
        public int CreateBulletObject(int _instanceCount, GameObject _prefab, string entityName = null )
        {
            var list = new List<Entity>(_instanceCount);
            const float INIT_POS = 10000f;

            var entityManager = World.Active.EntityManager;
            var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy( _prefab, World.Active );
            for (int i = 0; i < _instanceCount; i++)
            {
                var entity = entityManager.Instantiate( prefab );
                entityManager.SetComponentData(entity, new Translation {Value = new float3(INIT_POS,INIT_POS,INIT_POS)});
                entityManager.SetComponentData<BulletData>( entity, new BulletData(){IsInitialized = false} );
                entityManager.SetComponentData<ObjectMoveData>( entity, new ObjectMoveData(){ Speed = 100f, Direction = new Translation(){ Value = float3.zero} } );

                #if UNITY_EDITOR
                if( !string.IsNullOrEmpty(entityName))
                {
                    entityManager.SetName( entity, $"{entityName}_{i:D4}");
                }
                #endif

                list.Add( entity );
            }

            int handler = m_bulletsList.Count + 1;

            m_bulletsList.Add( handler, list);

            return handler;
        }


    }
}
