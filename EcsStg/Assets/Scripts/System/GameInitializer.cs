using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace GAME
{
    using GAME.DATA;

    public class GameInitializer : Singleton<GameInitializer>
    {
        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====
        [Header("カメラ")]
        [SerializeField]
        private Camera m_mainCam = null;


        [Header("DATA")]
        [SerializeField, Tooltip("Playerデータ")]
        private PlayerScriptableObject m_playerParam = null;

        [SerializeField, Tooltip("敵データ")]
        private EnemyScriptableObject[] m_enemyParams = null;


        World m_prevWorld;

        private bool m_isInitialized = false;
        public bool IsInitialized => m_isInitialized;

        Entity m_playerEntity;
        public Entity PlayerEntity => m_playerEntity;

        #endregion //) ===== MEMBER_VARIABLES =====

        //------------------------------------------
        // 初期化
        //------------------------------------------
        #region ===== INITIALIZE =====
        protected override void Init()
        {
            base.Init();

            // 世界の構築
            var newWorld = InitializeWorld();


            CreatePlayer( m_playerParam, newWorld);
            var system = newWorld.GetOrCreateSystem<PlayerComponentSystem>();
            // var system = World.Active.GetOrCreateSystem<PlayerComponentSystem>();
            system.Initialize( m_mainCam );

            for (int i = 0; i < m_enemyParams.Length; i++)
            {
                CreateEnemy( m_enemyParams[i], newWorld, i );
            }

            m_isInitialized = true;
        }

        World InitializeWorld()
        {
            m_prevWorld = World.Active;
            // InitWorld("GameWorld");
            return World.Active;
            // World world = World.Active = new World("GameWorld");
            // world.CreateSystem<DebugStream>();
            // world.CreateSystem<TransformSystemGroup>();
            // world.CreateSystem<EndFrameParentSystem>();
            // world.CreateSystem<EndFrameTRSToLocalToParentSystem>();
            // world.CreateSystem<EndFrameTRSToLocalToWorldSystem>();
            // world.CreateSystem<EndFrameLocalToParentSystem>();

            // ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);

            // return world;
        }

        void CreatePlayer( PlayerScriptableObject _data, World _currentWorld )
        {
            EntityManager manager = _currentWorld.EntityManager;
            m_playerEntity = manager.Instantiate( GameObjectConversionUtility.ConvertGameObjectHierarchy(_data.ObjectModel, _currentWorld));

            Translation t = new Translation();
            t.Value = new float3(_data.DefaultPosition);

            var moveData = new ObjectMoveData()
            {
                Speed = 1.0f,
                Direction = t,
            };

            var playerData = new PlayerData()
            {
                Life = _data.HP,
                PrevScreenPos = new float2( Screen.width * 0.5f, Screen.height * 0.5f),
            };
            var bulletFactoryData = new BulletFactoryData()
            {
                ParentEntity = m_playerEntity,
                PositionOffset = float3.zero,
                RotationOffset = float3.zero,

                SpawnCycle = _data.SpawnCycle,
                SpawnTimer = _data.SpawnCycle,

                /* バレットパラメーター */
                Speed = _data.BuleltSpeed,
                MoveDirection = new float3(0f, 0f, 1f ),
                Damage = _data.BulletDamage,
                BulletType = 0,
            };


            var bulletFactorySys = _currentWorld.GetOrCreateSystem<BulletFactorySystem>();
            if( bulletFactorySys != null )
            {
                Camera cam = Camera.main;
                float frustumHeight = cam.transform.position.y * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                float frustumWidth = frustumHeight / Screen.height * Screen.width;

                float maxSize = math.max(frustumHeight*2.0f, frustumWidth*2.0f);
                int count = BulletFactorySystem.CalcPreloadObjectCount( maxSize, bulletFactoryData.Speed, 1.0f, bulletFactoryData.SpawnCycle);

                bulletFactoryData.BulletListHandler = bulletFactorySys.CreateBulletObject(count, _data.BulletModel, "MyBullet");
            }

            manager.SetComponentData<Translation>( m_playerEntity, t);
            manager.AddComponentData(m_playerEntity, moveData);
            manager.AddComponentData(m_playerEntity, playerData);
            manager.AddComponentData(m_playerEntity, bulletFactoryData);


            BulletCollisionSystem bulletSys = _currentWorld.GetOrCreateSystem<BulletCollisionSystem>();
            bulletSys.Initialize( m_playerEntity );
            
            _currentWorld.GetOrCreateSystem<BulletComponentSystem>().Initialize( Camera.main );
        }

        void CreateEnemy( EnemyScriptableObject _data, World _currentWorld, int index )
        {
            EntityManager manager = _currentWorld.EntityManager;

            Entity model = manager.Instantiate( GameObjectConversionUtility.ConvertGameObjectHierarchy(_data.ObjectModel, _currentWorld));
            #if UNITY_EDITOR
            manager.SetName( model, $"Enemy_{index:D4}");
            #endif

            // Entity 管理に登録
            EnemyComponentSystem enemySys = _currentWorld.GetOrCreateSystem<EnemyComponentSystem>();
            int enemyId = enemySys.GetInstanceId();
            enemySys.AddEnemyEntity( enemyId, model );

            // Enemy の基本データ
            EnemyData enemyData = new EnemyData()
            {
                Id          = enemyId,
                HP          = _data.HP,
                Score       = _data.Score
            };
            manager.AddComponentData(model, enemyData);
            Translation initPos = new Translation();
            initPos.Value = new float3(_data.DefaultPosition);
            manager.SetComponentData<Translation>(model, initPos);


            Translation t = new Translation();
            t.Value = new float3(0f, 0f, 0.0f);

            var moveData = new ObjectMoveData()
            {
                Speed = 0.0f,
                Direction = t,
            };
            manager.AddComponentData(model, moveData);




            var bulletFactorySys = _currentWorld.GetOrCreateSystem<BulletFactorySystem>();
            Camera cam = Camera.main;
            float frustumHeight = cam.transform.position.y * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight / Screen.height * Screen.width;

            float maxSize = math.max(frustumHeight*2.0f, frustumWidth*2.0f);

            for (int i = 0; i < _data.Nway; i++)
            {
                var factoryEntity = manager.CreateEntity();
                var bulletFactoryData = new BulletFactoryData()
                {
                    ParentEntity = model,
                    PositionOffset = float3.zero,
                    RotationOffset = float3.zero,

                    SpawnCycle = _data.SpawnCycle,
                    SpawnTimer = _data.SpawnCycle,

                    /* バレットパラメーター */
                    Speed = _data.BuleltSpeed,
                    MoveDirection = Utils.CalcDirection( i, _data.Nway-1, new float3(0f, 0f, -1f ), _data.Angle),
                    Damage = 1,
                    BulletType = 1,
                };

                if( bulletFactorySys != null )
                {
                    int count = BulletFactorySystem.CalcPreloadObjectCount( maxSize, bulletFactoryData.Speed, 1.0f, bulletFactoryData.SpawnCycle);
                    bulletFactoryData.BulletListHandler = bulletFactorySys.CreateBulletObject(count, _data.BulletModel);

                    manager.AddComponentData(factoryEntity, bulletFactoryData);

                    // Enemy 管理システムにも登録
                    enemySys?.AddBulletFactoryRelation( enemyData.Id, factoryEntity );
                }
            }
        }
        
        
        #endregion //) ===== INITIALIZE =====


        public void WorldDispose()
        {
            m_isInitialized = false;

            World world = World.Active;
            world.GetExistingSystem<BulletCollisionSystem>().ResetSystem();
            world.GetExistingSystem<PlayerComponentSystem>().ResetSystem();
            world.GetExistingSystem<UI.EcsUISystem>().ResetSystem();

            EntityQuery query = world.EntityManager.CreateEntityQuery( new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(Translation) },
            });

            world.EntityManager.DestroyEntity(query);

            query = world.EntityManager.CreateEntityQuery( new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(BulletFactoryData) },
            });

            world.EntityManager.DestroyEntity(query);
        }
    }
}
