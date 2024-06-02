using SpellOfLust.Manager;
using UnityEngine;

namespace SpellOfLust
{
    public class AethraProxy : MonoBehaviour
    {
        public void PlaySoundFromEvent(AudioClip audioClip)
        {
            AudioManager.Instance.PlayOneShot(audioClip);
        }
    }
}
