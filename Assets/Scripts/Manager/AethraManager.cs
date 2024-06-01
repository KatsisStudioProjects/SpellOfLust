using System.Collections;
using UnityEngine;

namespace SpellOfLust.Manager
{
    public class AethraManager : MonoBehaviour
    {
        public static AethraManager Instance { private set; get; }

        [SerializeField]
        private Animator _anim;

        private bool _switchAuto = true;
        private int[] _availableFaces = new[] { 1, 2, 4, 5, 6 };

        private void Awake()
        {
            Instance = this;

            StartCoroutine(SwitchBetweenFaces());
        }

        private IEnumerator SwitchBetweenFaces()
        {
            while (_switchAuto)
            {
                _anim.SetTrigger($"Face{_availableFaces[Random.Range(0, _availableFaces.Length)]}");
                yield return new WaitForSeconds(Random.Range(10f, 20f));
            }
        }

        public void NextJerk(int level)
        {
            var max = MinesweeperManager.Instance.MaxLevel;

            _anim.Play($"Jerk{level}");
            _anim.SetBool("nude", level > max / 2);
            if (level == max - 1)
            {
                _switchAuto = false;
                _anim.SetTrigger("Face3");
            }
        }
    }
}
