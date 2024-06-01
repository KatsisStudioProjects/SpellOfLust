using System.Collections;
using UnityEngine;

namespace SpellOfLust.Manager
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { private set; get; }

        [SerializeField]
        private AudioClip[] _moans;

        private AudioSource _source;

        public bool IsMoaning { set; private get; } = true;

        private void Awake()
        {
            Instance = this;
            _source = GetComponentInChildren<AudioSource>();

            StartCoroutine(PlayMoans());
        }

        private IEnumerator PlayMoans()
        {
            yield return new WaitForSeconds(1f);
            while (IsMoaning)
            {
                _source.PlayOneShot(_moans[Random.Range(0, _moans.Length)]);
                yield return new WaitForSeconds(Random.Range(4f, 5f));
            }
        }
    }
}