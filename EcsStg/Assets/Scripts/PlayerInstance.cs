using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GAME
{
    public class PlayerInstance : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_playerModel = null;
        [SerializeField]
        private GameObject m_bulletModel = null;
        [SerializeField]
        private PlayerMoveSystem m_moveSys = null;
        public PlayerMoveSystem MoveSys => m_moveSys;

        /// <summary> 弾生成器 </summary>
        private ISpawner m_spawner;

        [SerializeField, Range(0.1f, 5.0f)]
        private float m_spawnCycle = 1.0f;

        private float m_spawnTimer = 1.0f;

        void Start()
        {
            Debug.Assert( m_playerModel != null );
            Debug.Assert( m_bulletModel != null );

            Transform playerModelTrans = this.transform;
            if( m_playerModel != null )
            {
                GameObject go = GameObject.Instantiate( m_playerModel, this.transform );
                playerModelTrans = go.transform;
            }

            m_spawner = new BulletFactory( new BulletFactory.InitParameter()
            {
                    BulletPrefab    = m_bulletModel,
                    ParentObject    = playerModelTrans,
                    PositionOffset  = Vector3.zero,
                    RotationOffset  = Vector3.zero,
                    ScreenSize      = new Vector2( GameConst.SCREE_WIDTH, GameConst.SCREE_HEIGHT),
                    SpawnCycle      = m_spawnCycle,

                    Speed           = 0.3f,
                    MoveDirection   = Vector3.forward,
                    Damage          = 1,
            });
            MoveSys?.Initialize( new PlayerMoveSystem.InitParameter(){
                MoveTarget = playerModelTrans,
                MainCamera = Camera.main,
            } );
        }

        // Update is called once per frame
        void Update()
        {
            m_spawnTimer -= Time.deltaTime;
            if( m_spawnTimer < 0 )
            {
                m_spawnTimer = m_spawnCycle;
                m_spawner?.Spawn();
            }
        }
    }
}
