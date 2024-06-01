using UnityEngine;

namespace SpellOfLust.Manager
{
    public class AethraManager : MonoBehaviour
    {
        public static AethraManager Instance { private set; get; }

        [SerializeField]
        private Animator _anim;

        private int _nextJerk;

        private void Awake()
        {
            Instance = this;
        }

        public void NextJerk()
        {
            _nextJerk++;
            _anim.Play($"Jerk{_nextJerk}");
        }
    }
}
