using System.Collections;
using UnityEngine;

namespace SpellOfLust.Manager
{
    public class AethraManager : MonoBehaviour
    {
        public static AethraManager Instance { private set; get; }

        [SerializeField]
        private Animator _anim;

        [SerializeField]
        private GameObject _mist;

        [SerializeField]
        private GameObject _censorBar;

        [SerializeField]
        private Transform[] _censorLocations;

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
            if (level == max / 2)
            {
                AudioManager.Instance.IncreaseFemIndex();
            }
            if (level == max - 2)
            {
                _mist.SetActive(true);
            }
            else if (level == max - 1)
            {
                _switchAuto = false;
                _anim.SetTrigger("Face3");
            }
        }

        public void Censor()
        {
            var p = _censorLocations[Random.Range(0, _censorLocations.Length)];
            var go = Instantiate(_censorBar, (Vector2)p.transform.position + Random.insideUnitCircle * 2f, Quaternion.identity);
            go.transform.localScale = new(Random.Range(.1f, .5f), Random.Range(.25f, .5f), 1f);
        }
    }
}
