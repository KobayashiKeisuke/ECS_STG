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

            m_spawner = new BulletFactory(
                m_bulletModel,
                this.transform,
                1.0f, Vector3.forward, 1
            );
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
