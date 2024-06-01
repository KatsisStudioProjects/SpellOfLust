using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpellOfLust
{
    public class MineButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private Color _normalColor, _hoverColor, _disabledColor;

        public UnityEvent OnLeftClick { private set; get; } = new();
        public UnityEvent OnRightClick { private set; get; } = new();

        private Image _image;

        private bool _interactable = true;
        public bool Interactable
        {
            set
            {
                _interactable = value;
                if (value)
                {
                    _image.color = _isHovered ? _hoverColor : _normalColor;
                }
                else
                {
                    _image.color = _disabledColor;
                }
            }
            get => _interactable;
        }

        private bool _isHovered;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.color = _normalColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Interactable)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    OnLeftClick?.Invoke();
                }
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    OnRightClick?.Invoke();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            if (Interactable)
            {
                _image.color = _hoverColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            if (Interactable)
            {
                _image.color = _normalColor;
            }
        }
    }
}
