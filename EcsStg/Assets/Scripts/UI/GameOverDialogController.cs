using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Unity.Entities;


namespace GAME.INGAME.UI
{
    public class GameOverDialogController : MonoBehaviour
    {
        enum BUTTON_TYPE
        {
            TO_TITLE,
            RETRY,
        }
        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====
        [SerializeField]
        private Text m_scoreText = null;
        [SerializeField]
        private Button[] m_buttons = null;

        bool m_isButtonClicked = false;
        #endregion //) ===== MEMBER_VARIABLES =====

        //------------------------------------------
        // 初期化周り
        //------------------------------------------
        #region ===== INITIALIZE =====

        public void Initialize(int _score )
        {
            m_scoreText.text = $"SCORE:{_score}";

            m_buttons[(int)BUTTON_TYPE.TO_TITLE].onClick.AddListener( OnTitleButtonClicked);
            m_buttons[(int)BUTTON_TYPE.RETRY].onClick.AddListener( OnRetryButtonClicked);
        }

        #endregion //) ===== INITIALIZE =====
        
        private void OnRetryButtonClicked()
        {
            if( m_isButtonClicked )
            {
                return;
            }

            m_isButtonClicked = true;
            GameInitializer.I.WorldDispose();
            SceneManager.LoadScene("IngameScene", LoadSceneMode.Single);
        }
        private void OnTitleButtonClicked()
        {
            if( m_isButtonClicked )
            {
                return;
            }
            m_isButtonClicked = true;

            GameInitializer.I.WorldDispose();
            SceneManager.LoadScene("Titlescene", LoadSceneMode.Single);
        }
    }
}