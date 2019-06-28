using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace GAME.DATA
{
    public class PlayerScriptableObject : ScriptableObject
    {
        #if UNITY_EDITOR
            const string DEFAULT_DIR = "Assets/Resources/Data/Player";
            [MenuItem ("Assets/ScriptableObject/Player")]
            static void CreateEnemyInstance ()
            {
                var newData = CreateInstance<PlayerScriptableObject> ();
                if( !System.IO.Directory.Exists(DEFAULT_DIR))
                {
                    System.IO.Directory.CreateDirectory(DEFAULT_DIR);
                }
                AssetDatabase.CreateAsset (newData, $"{DEFAULT_DIR}/NewPlayer.asset");
                AssetDatabase.Refresh ();
            }
        #endif


        /// <summary> モデルプレハブ </summary>
        [SerializeField]
        private GameObject m_objectModel = null;
        public  GameObject ObjectModel => m_objectModel;

        /// <summary> 弾プレハブ </summary>
        [SerializeField]
        private GameObject m_bulletModel = null;
        public  GameObject BulletModel => m_bulletModel;

        /// <summary> 初期位置 </summary>
        [SerializeField]
        private Vector3 m_defaultPosition = Vector3.zero;
        public  Vector3 DefaultPosition => m_defaultPosition;

        /// <summary> 初期回転 </summary>
        [SerializeField]
        private Vector3 m_defaultRotation = Vector3.zero;
        public  Vector3 DefaultRotation => m_defaultRotation;

        /// <summary> 初期Scale </summary>
        [SerializeField]
        private Vector3 m_defaultScale = Vector3.one;
        public  Vector3 DefaultScale => m_defaultScale;



        /// <summary> LIFE </summary>
        [SerializeField, Range(1, 100)]
        private int m_hitPoint = GameConst.DEFAULT_LIFE_COUNT;
        public  int HP => m_hitPoint;

        /// <summary> バレットの威力 </summary>
        [SerializeField, Range(1, 10)]
        private int m_bulletDamage = 1;
        public  int BulletDamage => m_bulletDamage;

        /// <summary> N-way数 </summary>
        [SerializeField]
        private int m_nway = 3;
        public  int Nway => m_nway;

        /// <summary> 発射角度 </summary>
        [SerializeField, Range( 0f, 360f)]
        private float m_angle = 180f;
        public  float Angle => m_angle;

        /// <summary> バレットの移動スピード </summary>
        [SerializeField, Range(0.01f, 5.0f)]
        private float m_buleltSpeed = 0.3f;
        public  float BuleltSpeed => m_buleltSpeed;

        /// <summary> バレット生成周期 </summary>
        [SerializeField, Range(0.1f, 5.0f)]
        private float m_spawnCycle = 1.0f;
        public  float SpawnCycle => m_spawnCycle;
    }
}
