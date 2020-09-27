using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KDScore
{

    public class Scoreboard : MonoBehaviour
    {
        public int maxScoreboardEntries = 32;
        public Transform scoreboardHolderTransform = null;
        public GameObject scoreboardEntryObject = null;
        public GameObject playerUI;

        bool _viewScore = false;

        private void Update()
        {
            if (PauseMenu.instance.inPauseMenu)
            {
                return;
            }

            if (Input.GetButtonDown("Score"))
            {
                if (_viewScore)
                {
                    HideScore();
                }
                else
                {
                    SeeScore();
                }
            
            }

            if (_viewScore)
            {
                List<ScoreboardEntryData> KDData = GetKDData();
                UpdateUI(KDData);
            }

        }

        private void SeeScore()
        {
            playerUI.SetActive(false);
            scoreboardHolderTransform.gameObject.SetActive(true);

            List<ScoreboardEntryData> KDData = GetKDData();
            UpdateUI(KDData);
            _viewScore = true;
        }

        private void HideScore()
        {
            playerUI.SetActive(true);
            scoreboardHolderTransform.gameObject.SetActive(false);
            _viewScore = false;
        }


        private List<ScoreboardEntryData> GetKDData()
        {
            List<ScoreboardEntryData> dataList = new List<ScoreboardEntryData>();

            for(int i=1; i < GameManager.players.Count+1; i++)
            {
                ScoreboardEntryData newItem;
                newItem.entryName = GameManager.players[i].username;
                newItem.entryKill = GameManager.players[i].kills;
                newItem.entryDeath = GameManager.players[i].death;
                dataList.Add(newItem);
            }
            return dataList;
        }

        private void UpdateUI(List<ScoreboardEntryData> _kdData)
        {
            foreach(Transform child in scoreboardHolderTransform)
            {
                Destroy(child.gameObject);
            }
            
            foreach(ScoreboardEntryData score in _kdData)
            {
                Instantiate(scoreboardEntryObject, scoreboardHolderTransform)
                    .GetComponent<ScoreboardEntryUI>().Initialise(score);
            }
        }

    }
}
