using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KDScore
{

    public class ScoreboardEntryUI : MonoBehaviour
    {
        public TextMeshProUGUI entryNameText;
        public TextMeshProUGUI entryKillText;
        public TextMeshProUGUI entryDeathText;

        public void Initialise(ScoreboardEntryData scoreboardEntryData)
        {
            entryNameText.text = scoreboardEntryData.entryName;
            entryKillText.text = "Kill:" + scoreboardEntryData.entryKill.ToString();
            entryDeathText.text = "Deaths: " + scoreboardEntryData.entryDeath.ToString();
        }

    }

}