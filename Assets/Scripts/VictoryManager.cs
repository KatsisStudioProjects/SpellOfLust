using SpellOfLust.Manager;
using TMPro;
using UnityEngine;

namespace SpellOfLust
{
    public class VictoryManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponentInChildren<AudioSource>();

            var time = (int)MinesweeperManager.Instance.Timer;
            _text.text = $"Time - {(int)(time / 60f)}:{time % 60}";
        }

        public void PlayOneShot(AudioClip clip)
        {
            _source.PlayOneShot(clip);
        }
    }
}