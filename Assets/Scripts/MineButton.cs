using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpellOfLust
{
    public class MineButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private Color _normalColor, _hoverColor, _disabledColor, _flaggedColor;

        public UnityEvent OnLeftClick { private set; get; } = new();
        public UnityEvent OnRightClick { private set; get; } = new();

        private Image _image;

        private bool _interactable = true;
        private bool _flagged;
        public void Disable()
        {
            _interactable = false;
            _image.color = _disabledColor;
        }

        private bool CanInteract => _interactable && !Flagged;

        public bool Flagged
        {
            set
            {
                _flagged = value;
                if (value)
                {
                    _image.color = _flaggedColor;
                }
                else
                {
                    _image.color = _isHovered ? _hoverColor : _normalColor;
                }
            }
            get => _flagged;
        }

        private bool _isHovered;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.color = _normalColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_interactable)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    if (!Flagged)
                    {
                        OnLeftClick?.Invoke();
                    }
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
            if (CanInteract)
            {
                _image.color = _hoverColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            if (CanInteract)
            {
                _image.color = _normalColor;
            }
        }
    }
}
