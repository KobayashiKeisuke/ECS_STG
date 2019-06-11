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

        /// <summary> 弾生成器 </summary>
        private List<ISpawner> m_spawners;

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
            m_spawners = new List<ISpawner>();
            m_spawners.Add( new BulletFactory(
                m_bulletModel,
                this.transform,
                m_spawnCycle,
                0.03f, new Vector3( -SQRT_TWO, 0f, -SQRT_TWO), 1
            ));
            m_spawners.Add( new BulletFactory(
                m_bulletModel,
                this.transform,
                m_spawnCycle,
                0.03f, Vector3.back, 1
            ));
            m_spawners.Add( new BulletFactory(
                m_bulletModel,
                this.transform,
                m_spawnCycle,
                0.03f, new Vector3( SQRT_TWO, 0f, -SQRT_TWO), 1
            ));
        }

        // Update is called once per frame
        void Update()
        {
            m_spawnTimer -= Time.deltaTime;
            if( m_spawnTimer < 0 )
            {
                m_spawnTimer = m_spawnCycle;
                foreach (var item in m_spawners)
                {
                    item?.Spawn();
                }
            }
        }
    }
}
