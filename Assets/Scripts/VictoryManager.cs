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

        private string Format(int nb)
        {
            if (nb < 10) return $"0{nb}";
            return nb.ToString();
        }

        private void Awake()
        {
            _source = GetComponentInChildren<AudioSource>();

            var time = (int)MinesweeperManager.Instance.Timer;
            _text.text = $"Time - {(int)(time / 60f)}:{Format(time % 60)}\n{MinesweeperManager.Instance.CensorCount} mine{(MinesweeperManager.Instance.CensorCount > 1 ? "s" : string.Empty)} clicked";
        }

        public void PlayOneShot(AudioClip clip)
        {
            _source.PlayOneShot(clip);
        }
    }
}