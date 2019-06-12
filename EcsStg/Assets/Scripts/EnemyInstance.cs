using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GAME
{
    public class EnemyInstance : MonoBehaviour
    {
        /// <summary> sqrt(2) </summary>
        const float SQRT_TWO = 0.707f;
        [SerializeField]
        private GameObject m_playerModel = null;
        [SerializeField]
        private GameObject m_bulletModel = null;
        [SerializeField]
        private int m_nway = 3;
        [SerializeField, Range( 0f, 360f)]
        private float m_angle = 180f;

        /// <summary> 弾生成器 </summary>
        private ISpawner m_spawner;

        [SerializeField, Range(0.1f, 5.0f)]
        private float m_spawnCycle = 1.0f;

        private float m_spawnTimer = 1.0f;

        void Start()
        {
            Debug.Assert( m_playerModel != null );
            Debug.Assert( m_bulletModel != null );

            if( m_playerModel != null )
            {
                GameObject.Instantiate( m_playerModel, this.transform );
            }
            m_spawner = new NwayBulletFactory( new NwayBulletFactory.InitParameter(){
                BulletPrefab    = m_bulletModel,
                ParentObject    = this.transform,
                PositionOffset  = Vector3.zero,
                RotationOffset  = Vector3.zero,
                ScreenSize      = new Vector2( GameConst.SCREE_WIDTH, GameConst.SCREE_HEIGHT),
                SpawnCycle      = m_spawnCycle,

                Speed           = 0.03f,
                MoveDirection   = Vector3.back,
                Damage          = 1,

                NwayCount       = m_nway,
                Angle           = m_angle,
            });
        }

        // Update is called once per frame
        void Update()
        {
            m_spawnTimer -= Time.deltaTime;
            if( m_spawnTimer < 0 )
            {
                m_spawnTimer = m_spawnCycle;
                m_spawner.Spawn();
            }
        }
    }
}
