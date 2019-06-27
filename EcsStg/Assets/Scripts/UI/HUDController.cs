using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GAME.UI
{
    public class HUDController : MonoBehaviour
    {
        //------------------------------------------
        // 定数関連
        //------------------------------------------
        #region ===== CONSTS =====
        static readonly string SCORE_TEXT = "SCORE: ";
        static readonly string LIFE_TEXT = "LIFE: ";

        #endregion //) ===== CONSTS =====
        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====
        [SerializeField]
        private Text m_scoreText = null;
        [SerializeField]
        private Text m_lifeText = null;

        private int m_currentScore = 0;
        #endregion //) ===== MEMBER_VARIABLES =====

        //------------------------------------------
        // 初期化周り
        //------------------------------------------
        #region ===== INITIALIZE =====

        public void Initialize( int _startLifeCount )
        {
            UpdateText( m_scoreText, $"{SCORE_TEXT}{m_currentScore}");
            UpdateLifeText( _startLifeCount);
        }

        #endregion //) ===== INITIALIZE =====

        //------------------------------------------
        // Text更新
        //------------------------------------------
        #region ===== TEXT =====

        public void AddScore( int _score )
        {
            m_currentScore += _score;
            UpdateText( m_scoreText, $"{SCORE_TEXT}{m_currentScore}");
        }
        public void UpdateLifeText( int _score ){ UpdateText( m_lifeText, $"{LIFE_TEXT}{_score}"); }

        private void UpdateText( Text _t, string _msg)
        {
            if( _t == null )
            {
                return;
            }
            _t.text = _msg;
        }
        #endregion //) ===== TEXT =====
    }
}
