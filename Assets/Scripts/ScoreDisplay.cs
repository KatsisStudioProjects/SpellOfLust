using SpellOfLust.Manager;
using TMPro;
using UnityEngine;

namespace SpellOfLust
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;

        private void Awake()
        {
            var time = (int)MinesweeperManager.Instance.Timer;
            _text.text = $"Time - {(int)(time / 60f)}:{time % 60}";
        }
    }
}