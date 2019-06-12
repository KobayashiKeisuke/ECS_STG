using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GAME.UI
{
    public class UIStickController : MonoBehaviour
    {
        //------------------------------------------
        // 定数関連
        //------------------------------------------
        #region ===== CONSTS =====

        /// <summary> 初期化パラメータ </summary>
        public class InitParameter : BulletFactory.InitParameter
        {
            /// <summary> Nway数 </summary>
            public int NwayCount = 1;

            /// <summary> バレット発射角度 </summary>
            public float Angle = 180f;
        }

        public struct SpriteParam
        {
            public float PixelPerUnit;
            public float Radius;
        }
        #endregion //)===== CONSTS =====

        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====

        [Header("操作パラメータ")]
        [SerializeField, Tooltip("BaseUI移動速度[1/frame]")]
        private float m_maxBaseSpriteMovePerFrame = 5f;
        protected float MaxBaseSpriteMovePerFrame => m_maxBaseSpriteMovePerFrame;


        [Header("オブジェクト")]
        [SerializeField, Tooltip("親SpriteObject")]
        private Transform m_baseSpriteTrans;
        #if UNITY_EDITOR
        public Transform BaseSpriteTrans => m_baseSpriteTrans;
        #endif
        [SerializeField, Tooltip("子SpriteObject")]
        private Transform m_stickSpriteTrans;
        #if UNITY_EDITOR
        public Transform StickSpriteTrans => m_stickSpriteTrans;
        #endif


        /// <summary> Base の可動域 </summary>
        Vector2 m_baseMoveRange_X = Vector2.zero;
        public Vector2 BaseMoveRange_X => m_baseMoveRange_X;
        Vector2 m_baseMoveRange_Y = Vector2.zero;
        public Vector2 BaseMoveRange_Y => m_baseMoveRange_Y;

        /// <summary> Stickの可動域 </summary>
        float m_stickMoveRange = 0.0f;
        public float StickMoveRange => m_stickMoveRange;

        /// <summary> Base 画像の設定 </summary>
        SpriteParam m_baseSpriteParam;
        #if UNITY_EDITOR
        public SpriteParam BaseSpriteParam => m_baseSpriteParam;
        #endif
        /// <summary> Sticker 画像の設定 </summary>
        SpriteParam m_stickerSpriteParam;
        #if UNITY_EDITOR
        public SpriteParam StickerSpriteParam => m_stickerSpriteParam;
        #endif

        /// <summary> Base 画像の半径 </summary>
        public float BaseSpriteRadius => m_baseSpriteParam.Radius;
        public float BaseSpriteSqrRange => BaseSpriteRadius * BaseSpriteRadius;

        /* Base画像の座標 */
        public float BasePosition_X => m_baseSpriteTrans.position.x;
        public float BasePosition_Y => m_baseSpriteTrans.position.y;

        /* Child画像の座標 */
        public float ChildPosition_X => m_stickSpriteTrans.position.x;
        public float ChildPosition_Y => m_stickSpriteTrans.position.y;


        Camera m_mainCamera;
        Camera MainCam => m_mainCamera;
        #endregion //) ===== MEMBER_VARIABLES =====

        //------------------------------------------
        // 初期化
        //------------------------------------------
        #region ===== INITIALIZE =====

        /// <summary>
        /// World 座標系で座標を扱うので注意!!
        /// </summary>
        /// <param name="_sys"></param>
        public void Initialize(InputSystem _sys, Camera _mainCam )
        {
            m_mainCamera = _mainCam;

            Sprite baseSprite                   = m_baseSpriteTrans.GetComponent<SpriteRenderer>().sprite;
            m_baseSpriteParam.PixelPerUnit      = baseSprite.pixelsPerUnit;
            m_baseSpriteParam.Radius            = baseSprite.bounds.size.x * 0.5f;
            Sprite childSprite                  = m_stickSpriteTrans.GetComponent<SpriteRenderer>().sprite;
            m_stickerSpriteParam.PixelPerUnit   = childSprite.pixelsPerUnit;
            m_stickerSpriteParam.Radius         = childSprite.bounds.size.x * 0.5f;

            // Base の可動域の設定
            m_baseMoveRange_X.x = MainCam.ScreenToWorldPoint(new Vector3(0f, 0f, 0f) ).x + BaseSpriteRadius;
            m_baseMoveRange_X.y = MainCam.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f) ).x  - BaseSpriteRadius;
            m_baseMoveRange_Y.y = MainCam.ScreenToWorldPoint(new Vector3(0, 0f, 0f) ).y - BaseSpriteRadius;
            m_baseMoveRange_Y.x = MainCam.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f) ).y + BaseSpriteRadius;

            // Stick の可動域
            m_stickMoveRange = (BaseSpriteRadius - m_stickerSpriteParam.Radius);// Radius

            _sys.AddOnDragEvent( this.OnDrag );

        }
        
        #endregion //) ===== INITIALIZE =====

        /// <summary>
        /// Mouse 移動イベント
        /// Screen 座標系のX-YとWorld座標系のX-YだとY軸が反転してるので注意
        /// </summary>
        /// <param name="_screenPos">現在のViewport座標</param>
        /// <param name="_diffVector">前回からのViewport座標差分</param>
        /// <param name="_currentTime">現在時刻</param>
        /// <param name="_deltaTime">差分時刻</param>
        private void OnDrag( Vector2 _screenPos, Vector2 _diffVector, float _currentTime, float _deltaTime )
        {
            if( _screenPos.x < 0 || 1.0f < _screenPos.x
                || _screenPos.y < 0 || 1.0f < _screenPos.y )
            {
                return;
            }
            Debug.Log($"Screen:{_screenPos}, ViewDiff:{_diffVector}, Time:{_currentTime}, dt:{_deltaTime}");

            Debug.Log($"Base:{m_baseSpriteTrans.position} Stick:{m_stickSpriteTrans.position}");
            // World 座標系で表現
            Vector3 nextStickPosition = MainCam.ScreenToWorldPoint(new Vector3(_screenPos.x * Screen.width, _screenPos.y* Screen.height, 0f) );
            nextStickPosition.z = m_stickSpriteTrans.position.z;
            Vector3 diffPos = m_baseSpriteTrans.position - nextStickPosition;
            Debug.Log($"NextStickPos:{nextStickPosition} diffPos:{diffPos} DiffRange:{diffPos.magnitude} ( {diffPos.sqrMagnitude} < {BaseSpriteSqrRange})");
            // Stickerが範囲内なら普通にUIを動かす
            if( diffPos.sqrMagnitude < m_stickMoveRange * m_stickMoveRange)
            {
                // Sprite 設定に合わせる
                m_stickSpriteTrans.position = nextStickPosition;
                return;
            }

            // MoveRange 外ならとりあえず目一杯外まで動かす
            float theta = Mathf.Atan2( diffPos.y, diffPos.x );
            float cosT = Mathf.Cos( theta );
            float sinT = Mathf.Sin( theta );
            Debug.LogWarning($"[Stick] theta:{theta*Mathf.Rad2Deg}\nX:{BasePosition_X}->{BasePosition_X} + {BaseSpriteRadius * cosT}\nY:{BasePosition_Y}->{BasePosition_Y} - {BaseSpriteRadius * sinT}\n");
            nextStickPosition.x = BasePosition_X + BaseSpriteRadius * cosT;
            nextStickPosition.y = BasePosition_Y - BaseSpriteRadius * sinT;
            m_stickSpriteTrans.position = nextStickPosition;

            // m_stickMoveRange ~ BaseSpriteRadius ならまだBaseは維持
            if( diffPos.sqrMagnitude < BaseSpriteSqrRange )
            {
                return;
            }

            // 範囲外なので最大限までStick位置は保持して、Baseを現在地点の方向へ可能な限り近づける
            float diffRange = diffPos.magnitude - BaseSpriteRadius;
            Vector3 nextBasePos = m_baseSpriteTrans.position;
            Debug.LogError($"[Base] X:{BasePosition_X}->{BasePosition_X} + {diffRange * cosT}\nY:{BasePosition_Y}->{BasePosition_Y} - {diffRange * sinT}\n");
            nextBasePos.x = Mathf.Clamp( BasePosition_X + diffRange * cosT, m_baseMoveRange_X.x, m_baseMoveRange_X.y);
            nextBasePos.y = Mathf.Clamp( BasePosition_Y - diffRange * sinT, m_baseMoveRange_Y.x, m_baseMoveRange_Y.y);

            m_baseSpriteTrans.position = nextBasePos;
        }
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(UIStickController))]
    public class UIStickControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var ctrl = target as UIStickController;
            EditorGUILayout.LabelField("Base Sprite");
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Vector3Field( "World Pos", ctrl.BaseSpriteTrans.position);
                EditorGUILayout.Vector3Field( "Local Pos", ctrl.BaseSpriteTrans.localPosition);
                EditorGUILayout.FloatField("Radius", ctrl.BaseSpriteParam.Radius);
                EditorGUILayout.FloatField("Pixel/Unit", ctrl.BaseSpriteParam.PixelPerUnit);

                EditorGUILayout.LabelField("Move Range");
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.FloatField( "X", ctrl.BaseMoveRange_X.x);   EditorGUILayout.FloatField(  ctrl.BaseMoveRange_X.y);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.FloatField( "Y", ctrl.BaseMoveRange_Y.x);   EditorGUILayout.FloatField(  ctrl.BaseMoveRange_Y.y);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.LabelField("Sticker Sprite");
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.FloatField("Movable Range", ctrl.StickMoveRange);

                EditorGUILayout.Vector3Field( "World Pos", ctrl.StickSpriteTrans.position);
                EditorGUILayout.Vector3Field( "Local Pos", ctrl.StickSpriteTrans.localPosition);
                EditorGUILayout.FloatField("Radius", ctrl.StickerSpriteParam.Radius);
                EditorGUILayout.FloatField("Pixel/Unit", ctrl.StickerSpriteParam.PixelPerUnit);
                EditorGUI.indentLevel--;
            }



        }
    }
    #endif
}