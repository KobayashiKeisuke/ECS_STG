using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GAME.UI
{
    using GAME.INPUT;
    using GAME.DATA;

    /// <summary>
    /// まっさきに入力を受け付けてから物理世界を更新かけるため更新順を定義
    /// </summary>
    [UpdateAfter(typeof(InputControlSystem))]
    public class EcsUISystem : JobComponentSystem
    {
        //------------------------------------------
        // 定数関連
        //------------------------------------------
        #region ===== CONSTS =====

        /// <summary> 初期化パラメータ </summary>
        public class InitParameter
        {
            /// <summary> 初期化パラメータ </summary>
            public InputSystem InputSys;
            /// <summary> 初期化パラメータ </summary>
            public Camera MainCam;
            /// <summary> 初期化パラメータ </summary>
            public IPlayerMove PlayerMove;
        }

        public struct SpriteParam
        {
            public float PixelPerUnit;
            public float Radius;
        }

        /// <summary>
        /// ユーザー入力データのUIレイヤー補正を加えたデータ群
        /// </summary>
        public struct UIInputData
        {
            /// <summary> 動く方向 </summary>
            public float Angle;
            /// <summary> 正規化された動く距離[0 1] </summary>
            public float Range;
        }

        #endregion //)===== CONSTS =====

        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====

        /// <summary> ユーザー入力データのUIレイヤー補正を加えたデータ群 </summary>
        private UIInputData m_uiData;
        public  UIInputData UiData => m_uiData;

        private INPUT.InputControlSystem m_inputCtrlSys;
        protected INPUT.InputControlSystem InputCtrlSystem => m_inputCtrlSys;
        
        EntityManager E_Manager;

        [Header("操作パラメータ")]
        [SerializeField, Tooltip("BaseUI移動速度[1/frame]")]
        private float m_maxBaseSpriteMovePerFrame = .05f;
        public float MaxBaseSpriteMovePerFrame => m_maxBaseSpriteMovePerFrame;


        /// <summary> Base の可動域 </summary>
        Vector2 m_baseMoveRange_X = Vector2.zero;
        public Vector2 BaseMoveRange_X => m_baseMoveRange_X;
        Vector2 m_baseMoveRange_Z = Vector2.zero;
        public Vector2 BaseMoveRange_Y => m_baseMoveRange_Z;

        /// <summary> Stickの可動域 </summary>
        float m_stickMoveRange = 0.0f;
        public float StickMoveRange => m_stickMoveRange;

        /// <summary> Base 画像の設定 </summary>
        SpriteParam m_baseSpriteParam;
        /// <summary> Sticker 画像の設定 </summary>
        SpriteParam m_stickerSpriteParam;

        /// <summary> Base 画像の半径 </summary>
        public float BaseSpriteRadius => m_baseSpriteParam.Radius;
        public float BaseSpriteSqrRange => BaseSpriteRadius * BaseSpriteRadius;


        Camera m_mainCamera;
        Camera MainCam => m_mainCamera;

        EcsUIData m_uiTransData;

        float m_posY = 0f;
        #endregion //) ===== MEMBER_VARIABLES =====

        //------------------------------------------
        // 初期化
        //------------------------------------------
        #region ===== INITIALIZE =====

        /// <summary>
        /// World 座標系で座標を扱うので注意!!
        /// </summary>
        /// <param name="_sys"></param>
        public void Initialize(Camera _mainCam, GameObject _mainPrefab, GameObject _stickPrefab )
        {
            m_mainCamera = _mainCam;
            E_Manager = World.Active.EntityManager;
            // UI生成
            var parentEntity    = E_Manager.Instantiate( GameObjectConversionUtility.ConvertGameObjectHierarchy( _mainPrefab, World.Active ) );
            E_Manager.SetComponentData(parentEntity, new Translation {Value = new float3(0f, 0f, 0f)});
            E_Manager.SetName( parentEntity, "Base");
            var childEntity     = E_Manager.Instantiate( GameObjectConversionUtility.ConvertGameObjectHierarchy( _stickPrefab, World.Active ) );
            E_Manager.SetComponentData(childEntity, new Translation {Value = new float3(0f,0f,0f)});
            E_Manager.SetName( childEntity, "Stick");
            var relationEntity = E_Manager.CreateEntity();
            E_Manager.SetName( relationEntity, "UI_Relation");

            m_uiTransData = new EcsUIData()
            {
                Parent      = parentEntity,
                Child       = childEntity,
            };
            E_Manager.AddComponentData( relationEntity, m_uiTransData);



            // SpriteRender.bounds に該当するやつどうやってとるんだ？
            m_baseSpriteParam.PixelPerUnit      = _mainCam.orthographicSize;
            m_baseSpriteParam.Radius            = 5;
            m_stickerSpriteParam.PixelPerUnit   = _mainCam.orthographicSize;
            m_stickerSpriteParam.Radius         = 2;
            // Base の可動域の設定
            Vector3 leftBottom = MainCam.ScreenToWorldPoint(new Vector3(0f, 0f, 0f) );
            Vector3 rightTop = MainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f) );
            Debug.Log($"LB:{leftBottom}. RT:{rightTop}");
            m_baseMoveRange_X = new Vector2( leftBottom.x, rightTop.x);
            m_baseMoveRange_Z = new Vector2( leftBottom.z, rightTop.z);

            // Stick の可動域
            m_stickMoveRange = (BaseSpriteRadius - m_stickerSpriteParam.Radius);// Radius
            m_posY = leftBottom.y - 100;
            HideUI();
            Debug.Log($"X:{m_baseMoveRange_X.x}<->{m_baseMoveRange_X.y}");
            Debug.Log($"Z:{m_baseMoveRange_Z.x}<->{m_baseMoveRange_Z.y}");
        }

        protected override void OnCreate()
        {
            m_uiData = new UIInputData();

            m_inputCtrlSys = World.Active.GetOrCreateSystem<INPUT.InputControlSystem>();
            E_Manager = World.Active.EntityManager;

        }

        #endregion //) ===== INITIALIZE =====

        // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
        [BurstCompile]
        struct UiInputJob : IJobForEachWithEntity<EcsUIData>
        {
            public void Execute(Entity entity, int index, ref EcsUIData uiData )
            {
            }
        }

        // OnUpdate runs on the main thread.
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            Translation parentPos = E_Manager.GetComponentData<Translation>( m_uiTransData.Parent );
            Translation childPos = E_Manager.GetComponentData<Translation>( m_uiTransData.Child );

            var inputData = m_inputCtrlSys.GetCurrentInputData();

            switch( inputData.State )
            {
                case INPUT.TOUCH_STATE.BEGIN:
                {
                    OnDragStart( );
                }
                break;
                case INPUT.TOUCH_STATE.MOVED:
                {
                    OnDrag( inputData.ScreenPosition, ref parentPos, ref childPos );
                }
                break;
                case INPUT.TOUCH_STATE.ENDED:
                {
                    OnDragEnd( ref parentPos, ref childPos );
                }
                break;
            }
            E_Manager.SetComponentData<Translation>( m_uiTransData.Parent, parentPos );
            E_Manager.SetComponentData<Translation>( m_uiTransData.Child, childPos );

            var job = new UiInputJob()
            {
            };

            return job.Schedule(this, inputDependencies);
        }

        //------------------------------------------
        // Drag 処理
        //------------------------------------------
        #region ===== DRAG =====

        public void OnDragStart( )
        {
            m_uiData.Range = 0f;
            ShowUI();
        }

        /// <summary>
        /// Mouse 移動イベント
        /// Screen 座標系のX-YとWorld座標系のX-YだとY軸が反転してるので注意
        /// </summary>

        public void OnDrag( float2 _screenPos, ref Translation _basePos, ref Translation _stickPos)
        {
            _basePos.Value.y = m_posY;
            _stickPos.Value.y = m_posY;
            if( _screenPos.x < 0 || Screen.width < _screenPos.x
                || _screenPos.y < 0 ||Screen.height< _screenPos.y )
            {
                return;
            }
            // World 座標系で表現
            float3 nextStickPosition = GetScreenToTranslation( MainCam, _screenPos );
            nextStickPosition.y = _basePos.Value.y;
            float3 diffPos = nextStickPosition - _basePos.Value;
            float theta = Mathf.Atan2( diffPos.z , diffPos.x );
            float sqrMagnitude = GetSqrMagnitude(diffPos);

            // Stickerが範囲内なら普通にUIを動かす
            if( sqrMagnitude < m_stickMoveRange * m_stickMoveRange)
            {
                // Sprite 設定に合わせる
                _stickPos.Value = nextStickPosition;
                // パラメータ更新
                m_uiData.Angle  = theta;
                m_uiData.Range  = sqrMagnitude / m_stickMoveRange / m_stickMoveRange;
                return;
            }
            // パラメータ更新(最大レンジで移動)
            m_uiData.Angle  = theta;
            m_uiData.Range  = 1.0f;

            // MoveRange 外ならとりあえず目一杯外まで動かす
            float cosT = Mathf.Cos( theta );
            float sinT = Mathf.Sin( theta );
            nextStickPosition.x = _basePos.Value.x + m_stickMoveRange * cosT;
            nextStickPosition.z = _basePos.Value.z + m_stickMoveRange * sinT;
            _stickPos.Value = nextStickPosition;

            // m_stickMoveRange ~ BaseSpriteRadius ならまだBaseは維持
            if( sqrMagnitude < BaseSpriteSqrRange )
            {
                return;
            }

            // 範囲外なので最大限までStick位置は保持して、Baseを現在地点の方向へ可能な限り近づける
            float diffRange = math.sqrt(sqrMagnitude) - BaseSpriteRadius;
            float3 nextBasePos = _basePos.Value;
            nextBasePos.x = Mathf.Clamp( nextBasePos.x + diffRange * cosT, m_baseMoveRange_X.x, m_baseMoveRange_X.y);
            nextBasePos.y = m_posY;
            nextBasePos.z = Mathf.Clamp( nextBasePos.z + diffRange * sinT, m_baseMoveRange_Z.x, m_baseMoveRange_Z.y);

            _basePos.Value = nextBasePos;
        }


        public void OnDragEnd(ref Translation _basePos, ref Translation _stickPos )
        {
            _stickPos.Value = _basePos.Value;
            m_uiData.Range = 0f;
            HideUI();
        }

        public float3 GetScreenToTranslation( Camera _cam, float2 _screenPos )
        {
            Vector3 pos = _cam.ScreenToWorldPoint(new Vector3(_screenPos.x, _screenPos.y, 0f) );
            return new float3( pos.x, pos.y, pos.z);
        }
        public float GetSqrMagnitude( float3 _vec ){    return _vec.x * _vec.x + _vec.y * _vec.y + _vec.z * _vec.z; }
        
        #endregion //) ===== DRAG =====


        //------------------------------------------
        // UI アクティブ制御
        //------------------------------------------
        #region ===== UI_ACTIVTION =====

        void ShowUI(){ SwitchUIActivation( true );}
        void HideUI(){ SwitchUIActivation( false );}
        private void SwitchUIActivation( bool _isActive )
        {
            // if( m_baseSpriteTrans.gameObject.activeInHierarchy != _isActive )
            // {
            //     m_baseSpriteTrans.gameObject.SetActive( _isActive );
            // }
        }

        #endregion //) ===== UI_ACTIVTION =====
    }
}
