using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using GAME;
using GAME.DATA;


[RequiresEntityConversion]
public class EnemyMonoBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{
        //------------------------------------------
        // 定数関連
        //------------------------------------------
        #region ===== CONSTS =====
        /// <summary> sqrt(2) </summary>
        const float SQRT_TWO = 0.707f;

        #endregion //) ===== CONSTS =====
        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====
        [SerializeField]
        private GameObject m_bulletModel = null;
        [SerializeField]
        private int m_nway = 3;
        [SerializeField, Range( 0f, 360f)]
        private float m_angle = 180f;

        [SerializeField, Range(0.01f, 5.0f)]
        private float m_buleltSpeed = 0.3f;

        [SerializeField, Range(0.1f, 5.0f)]
        private float m_spawnCycle = 1.0f;

        #endregion //) ===== MEMBER_VARIABLES =====


    public void Convert( Entity _entity, EntityManager dstManager, GameObjectConversionSystem _conversionSystem )
    {
        Translation t = new Translation();
        t.Value = new float3(0f, 0f, 0.0f);

        var moveData = new ObjectMoveData()
        {
            Speed = 0.0f,
            Direction = t,
        };
        dstManager.AddComponentData(_entity, moveData);



        var bulletFactorySys = World.Active.GetOrCreateSystem<BulletFactorySystem>();
        Camera cam = Camera.main;
        float frustumHeight = cam.transform.position.y * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight / Screen.height * Screen.width;

        float maxSize = math.max(frustumHeight, frustumWidth);

        for (int i = 0; i < m_nway; i++)
        {
            var factoryEntity = dstManager.CreateEntity();
            var bulletFactoryData = new BulletFactoryData()
            {
                ParentEntity = _entity,
                PositionOffset = float3.zero,
                RotationOffset = float3.zero,

                SpawnCycle = m_spawnCycle,
                SpawnTimer = m_spawnCycle,

                /* バレットパラメーター */
                Speed = m_buleltSpeed,
                MoveDirection = CalcDirection( i, m_nway-1, new float3(0f, 0f, -1f ), m_angle),
                Damage = 1,
                BulletType = 1,
            };

            if( bulletFactorySys != null )
            {
                int count = BulletFactorySystem.CalcPreloadObjectCount( maxSize, bulletFactoryData.Speed, 1.0f, bulletFactoryData.SpawnCycle);
                bulletFactoryData.BulletListHandler = bulletFactorySys.CreateBulletObject(count, m_bulletModel);

                dstManager.AddComponentData(factoryEntity, bulletFactoryData);
            }
        }
    }
    
    private float3 CalcDirection( int _index, int n_way, float3 baseDir, float angle )
    {
        // 起点まで回転
        var initVec = baseDir.Rotate_Y_Axis( -angle * 0.5f );
        return initVec.Rotate_Y_Axis(  angle * _index / n_way );
    }
}
