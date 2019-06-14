using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GAME.INPUT
{
    public enum TOUCH_STATE
    {
        /// <summary> タッチなし </summary>
        NONE = 99 ,
        /* TouchPhase 準拠 */
        /// <summary> タッチ開始 </summary>
        BEGIN = 0,
        /// <summary> タッチ中 </summary>
        MOVED,
        /// <summary> 指を離す </summary>
        ENDED,
        /// <summary> タッチ追跡不可能 </summary>
        CANCEL,
    }
    /// <summary>
    /// 入力データ保持Component
    /// </summary>
    public struct InputData : IComponentData
    {
        /// <summary> スクリーン座標 </summary>
        public float2 ScreenPosition;
        /// <summary> 状態 </summary>
        public TOUCH_STATE State;
        /// <summary> 現在時刻 </summary>
        public float CurrentTime;
        /// <summary> 差分時刻[sec] </summary>
        public float DeltaTime;
    }
}
