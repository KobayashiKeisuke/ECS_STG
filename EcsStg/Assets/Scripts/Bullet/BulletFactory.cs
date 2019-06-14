using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using GAME.DATA;

namespace GAME
{
    public class BulletFactory : ISpawner
    {
        //------------------------------------------
        // 定数関連
        //------------------------------------------
        #region ===== CONSTS =====

        /// <summary> 初期化パラメータ </summary>
        public class InitParameter
        {
            /// <summary> 生成するPrefab </summary>
            public GameObject BulletPrefab;
            /// <summary> 生成親 </summary>
            public Transform ParentObject;
            /// <summary> 生成時のPositionOffset </summary>
            public Vector3 PositionOffset;
            /// <summary> 生成時のRotationOffset </summary>
            public Vector3 RotationOffset;
            /// <summary> 画面サイズ </summary>
            public Vector2 ScreenSize;

            /// <summary> バレットの生成サイクル </summary>
            public float SpawnCycle;
            /* バレットパラメーター */
            /// <summary> フレームあたりの移動速度 </summary>
            public float Speed;
            /// <summary> 移動方向 </summary>
            public Vector3 MoveDirection;
            /// <summary> バレットのダメージ量 </summary>
            public int Damage;
        }

        /// <summary>
        /// バレット生成時の初期化パラメータまとめ
        /// </summary>
        protected struct InitData
        {
            public ObjectMoveData   MoveInitInfo;
            public BulletData       BulletInitInfo;
        }
        #endregion //)===== CONSTS =====

        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====

        /// <summary> 生成モデル </summary>
        GameObject m_prefab = null;
        protected GameObject BulletPrefab => m_prefab;

        /// <summary> 初期化チェックフラグ </summary>
        protected bool m_isInitialized = false;
        public bool IsInitialized => m_isInitialized;

        /// <summary> Bullet の初期値 </summary>
        InitData m_initData;
        protected InitData InitInfo => m_initData;

        /// <summary> Bullet の初期生成位置 </summary>
        Transform m_initSpawnPos;
        protected Transform ParentTransform => m_initSpawnPos;

        protected Unity.Entities.Entity[] m_entities;

        #endregion //) ===== MEMBER_VARIABLES =====


        public BulletFactory( InitParameter _param )
        {
            Debug.Assert( _param != null );
            Debug.Assert( _param.BulletPrefab != null );
            Debug.Assert( _param.ParentObject != null );
            Debug.Assert( _param.ScreenSize.x > 0 && _param.ScreenSize.y > 0 );
            Debug.Assert( _param.MoveDirection.sqrMagnitude > 0 );
            Debug.Assert( _param.Speed > 0 );
            Debug.Assert( _param.SpawnCycle > 0 );
            Debug.Assert( _param != null );

            SetPrefab( _param.BulletPrefab );
            SetParentObject( _param.ParentObject );
            SetBulletInitParam( _param );

            int totalSetPerLifeTime = CalcPreloadObjectCount( Mathf.Max( _param.ScreenSize.x, _param.ScreenSize.y), _param.Speed, _param.MoveDirection.magnitude, _param.SpawnCycle);
            PreloadObject( totalSetPerLifeTime );

            m_isInitialized = true;
        }

        protected void SetPrefab( GameObject _object ){ m_prefab = _object; }
        protected void SetParentObject( Transform _parent ){ m_initSpawnPos = _parent; }
        protected void SetBulletInitParam( InitParameter _param )
        {

            m_initData = new InitData()
            {
                BulletInitInfo = new BulletData()
                {
                    IsInitialized = true,
                    Damage = _param.Damage,
                },
                MoveInitInfo = new ObjectMoveData()
                {
                    Speed = _param.Speed,
                    Direction = new Translation{ Value = new float3(_param.MoveDirection.x, _param.MoveDirection.y, _param.MoveDirection.z) },
                },
            };
        }

        /// <summary>
        /// 事前に生成するオブジェクト数を計算
        /// </summary>
        /// <param name="ScreenLongerSize">画面の最大長</param>
        /// <param name="moveSpeed">バレットの移動速度</param>
        /// <param name="moveDirSize">移動方向のノルム</param>
        /// <param name="spawnCycle">生成間隔[sec]</param>
        /// <returns></returns>
        protected virtual int CalcPreloadObjectCount( float ScreenLongerSize, float moveSpeed, float moveDirSize, float spawnCycle )
        {
            float lifeTime = ScreenLongerSize / ( moveSpeed * GameConst.FPS) / moveDirSize;
            return Mathf.FloorToInt( lifeTime /spawnCycle ) + 1;
        }

        protected void PreloadObject(int _instanceCount )
        {
            m_entities = new Unity.Entities.Entity[_instanceCount];
            const float INIT_POS = 10000f;

            var entityManager = World.Active.EntityManager;
            var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy( BulletPrefab, World.Active );
            for (int i = 0; i < _instanceCount; i++)
            {
                var instance = entityManager.Instantiate( prefab );
                entityManager.SetComponentData(instance, new Translation {Value = new float3(INIT_POS,INIT_POS,INIT_POS)});
                entityManager.SetComponentData<BulletData>( instance, new BulletData(){IsInitialized = false} );
                entityManager.SetComponentData<ObjectMoveData>( instance, new ObjectMoveData(){} );

                m_entities[i] = instance;
            }
        }


        public void Spawn( )
        {
            if( !IsInitialized )
            {
                return;
            }
            var entityManager = World.Active.EntityManager;
            Vector3 currentPos = ParentTransform.position;

            for (int i = 0; i < m_entities.Length; i++)
            {
                BulletData bulletInfo = entityManager.GetComponentData<BulletData>( m_entities[i]);
                if( !bulletInfo.IsInitialized )
                {
                    ObjectMoveData moveData = entityManager.GetComponentData<ObjectMoveData>( m_entities[i]);
                    entityManager.SetComponentData(m_entities[i], new Translation{Value = new float3(currentPos.x, currentPos.y, currentPos.z)});
                    entityManager.SetComponentData<ObjectMoveData>( m_entities[i], InitInfo.MoveInitInfo );
                    entityManager.SetComponentData<BulletData>( m_entities[i], InitInfo.BulletInitInfo);
                    break;
                }
            }
        }
    }
}
