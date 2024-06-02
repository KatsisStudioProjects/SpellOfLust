using System.Collections;
using UnityEngine;

namespace SpellOfLust.Manager
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { private set; get; }

        [SerializeField]
        private AudioClip[] _malemoans;

        [SerializeField]
        private Moans[] _femMoans;
        private int _femIndex;

        private AudioSource _source;

        private void Awake()
        {
            Instance = this;
            _source = GetComponentInChildren<AudioSource>();

            StartCoroutine(PlayMaleMoans());
            StartCoroutine(PlayFemaleMoans());
        }

        public void IncreaseFemIndex()
        {
            _femIndex = 1;
        }

        private IEnumerator PlayMaleMoans()
        {
            yield return new WaitForSeconds(1f);
            while (true)
            {
                _source.PlayOneShot(_malemoans[Random.Range(0, _malemoans.Length)]);
                yield return new WaitForSeconds(Random.Range(4f, 5f));
            }
        }

        private IEnumerator PlayFemaleMoans()
        {
            yield return new WaitForSeconds(1.5f);
            while (true)
            {
                var m = _femMoans[_femIndex].Clips;
                _source.PlayOneShot(m[Random.Range(0, m.Length)]);
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            }
        }

        public void PlayOneShot(AudioClip audioClip)
        {
            _source.PlayOneShot(audioClip);
        }
    }

    [System.Serializable]
    public class Moans
    {
        public AudioClip[] Clips;
    }
}