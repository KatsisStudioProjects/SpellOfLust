using SpellOfLust.SO;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SpellOfLust.Manager
{
    public class MinesweeperManager : MonoBehaviour
    {
        public static MinesweeperManager Instance { private set; get; }

        [SerializeField]
        private Transform _mainContainer;

        [SerializeField]
        private GameObject _hLinePrefab, _tilePrefab;

        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private AudioClip _clickMine, _flag, _openTile;

        private TileData[,] _grid;

        public float Timer { private set; get; }
        public int CensorCount { set; get; }

        public bool CanInteract { private set; get; } = true;

        private int Size => _info.Levels[CurrentLevel].Size;
        private int MineCount => _info.Levels[CurrentLevel].MineCount;

        /// <summary>
        /// Current level index for our SO
        /// </summary>
        public int CurrentLevel { private set; get; }

        /// <summary>
        /// Were the mines already placed
        /// </summary>
        private bool _isGenerated = false;

        public int MaxLevel => _info.Levels.Length;

        private void Awake()
        {
            Instance = this;

            Assert.IsTrue(MineCount <= Size * Size);

            RegenerateBoard();

            Timer = 0f;
            CensorCount = 0;
        }

        private void Update()
        {
            if (CanInteract)
            {
                Timer += Time.deltaTime;
            }
        }

        public void PlaySfxLoose() => AudioManager.Instance.PlayOneShot(_clickMine);
        public void PlaySfxFlag() => AudioManager.Instance.PlayOneShot(_flag);
        public void PlaySfxTile() => AudioManager.Instance.PlayOneShot(_openTile);

        public void NewBoard()
        {
            CanInteract = false;
            StartCoroutine(ShowValidation());
        }
        private IEnumerator ShowValidation()
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    _grid[x, y].ValidateIfMine();
                }
            }
            yield return new WaitForSeconds(1f);
            CanInteract = true;
            RegenerateBoard();
        }

        public void RegenerateBoard()
        {
            for (int i = 0; i < _mainContainer.childCount; i++) Destroy(_mainContainer.GetChild(i).gameObject);

            _grid = new TileData[Size, Size];

            for (int y = 0; y < Size; y++)
            {
                var line = Instantiate(_hLinePrefab, _mainContainer);
                for (int x = 0; x < Size; x++)
                {
                    var go = Instantiate(_tilePrefab, line.transform);
                    var data = new TileData(go, x, y);

                    _grid[x, y] = data;
                }
            }

            _isGenerated = false;
        }

        public void IncreaseLevel()
        {
            CurrentLevel++; 
        }

        public bool DidReachVictory()
        {
            return CurrentLevel == MaxLevel - 1;
        }

        public void GenerateIfNeeded(int ignoreX, int ignoreY)
        {
            if (_isGenerated) return;

            _isGenerated = true;

            int mineLeft = MineCount;
            while (mineLeft > 0)
            {
                var randX = Random.Range(0, Size);
                var randY = Random.Range(0, Size);

                if (randX == ignoreX && randY == ignoreY) continue;

                var tile = _grid[randX, randY];
                if (tile.HasMine) continue;

                tile.HasMine = true;
                mineLeft--;
            }

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    int count = 0;
                    if (_grid[x, y].HasMine) continue;

                    for (int yi = -1; yi <= 1; yi++)
                    {
                        for (int xi = -1; xi <= 1; xi++)
                        {
                            var fx = x + xi;
                            var fy = y + yi;
                            if ((xi == 0 && yi == 0) || fx < 0 || fy < 0 || fx >= Size || fy >= Size) continue;

                            if (_grid[fx, fy].HasMine) count++;
                        }
                    }
                    _grid[x, y].AdjacentMines = count;
                }
            }
        }

        public void ShowContent(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Size || y >= Size || _grid[x, y].IsShown)
            {
                return;
            }

            _grid[x, y].Show();
            if (_grid[x, y].AdjacentMines == 0)
            {
                for (int yi = -1; yi <= 1; yi++)
                {
                    for (int xi = -1; xi <= 1; xi++)
                    {
                        ShowContent(x + xi, y + yi);
                    }
                }
            }
        }

        public bool IsGameWon()
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (!_grid[x, y].DoesSatisfyVictory())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void OnMainClick(InputAction.CallbackContext value)
        {
            if (value.performed && _grid.Length == Size * Size)
            {
                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                        if (_grid[x, y].Button.IsHovered)
                        {
                            if (_grid[x, y].Button.ButtonEnabled) _grid[x, y].Button.MainClick();
                            return;
                        }
                    }
                }
            }
        }

        public void OnAltClick(InputAction.CallbackContext value)
        {
            if (value.performed && _grid.Length == Size * Size)
            {
                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                        if (_grid[x, y].Button.IsHovered)
                        {
                            if (_grid[x, y].Button.ButtonEnabled) _grid[x, y].Button.AltClick();
                            return;
                        }
                    }
                }
            }
        }
    }

    public class TileData
    {
        public TileData(GameObject go, int x, int y)
        {
            Go = go;
            _x = x;
            _y = y;
            _text = Go.GetComponentInChildren<TMP_Text>();
            Button = Go.GetComponent<MineButton>();
            Button.OnLeftClick.AddListener(Click);
            Button.OnRightClick.AddListener(Flag);
        }

        private void PreCheck()
        {
            MinesweeperManager.Instance.GenerateIfNeeded(_x, _y);
        }

        public void Click()
        {
            if (!MinesweeperManager.Instance.CanInteract) return;
            PreCheck();
            if (!HasFlag)
            {
                if (HasMine)
                {
                    AethraManager.Instance.Censor();
                    MinesweeperManager.Instance.PlaySfxLoose();

                    HasFlag = true;
                    Button.ShowMine();
                    MinesweeperManager.Instance.CensorCount++;
                    CheckVictory();
                    return;
                }
                MinesweeperManager.Instance.ShowContent(_x, _y);
                MinesweeperManager.Instance.PlaySfxTile();
            }
        }

        public void Show()
        {
            IsShown = true;
            HasFlag = false;
            Button.Disable();

            if (AdjacentMines > 0)
            {
                _text.text = $"<color=#{_colors[AdjacentMines - 1].ToHexString()}>{AdjacentMines}</color>";
            }
        }

        public void Flag()
        {
            if (!MinesweeperManager.Instance.CanInteract) return;
            PreCheck();

            HasFlag = !HasFlag;
            Button.Flagged = HasFlag;
            MinesweeperManager.Instance.PlaySfxFlag();

            CheckVictory();
        }

        private void CheckVictory()
        {
            if (MinesweeperManager.Instance.IsGameWon())
            {
                if (MinesweeperManager.Instance.DidReachVictory())
                {
                    SceneManager.LoadScene("Victory");
                }
                else
                {
                    MinesweeperManager.Instance.NewBoard();
                    MinesweeperManager.Instance.IncreaseLevel();
                    AethraManager.Instance.NextJerk(MinesweeperManager.Instance.CurrentLevel);
                }
            }
        }

        public void ValidateIfMine()
        {
            if (HasMine)
            {
                Button.Validate();
            }
        }

        public bool DoesSatisfyVictory()
        {
            return (HasFlag && HasMine) || (!HasFlag && !HasMine);
        }

        private readonly int _x, _y;
        public GameObject Go { private set; get; }
        private readonly TMP_Text _text;
        public readonly MineButton Button;
        public int AdjacentMines { set; get; }
        public bool HasMine { set; get; }
        public bool HasFlag { set; get; }
        public bool IsShown { private set; get; }

        private static readonly Color[] _colors = new[]
        {
            Color.blue, new(0f, 139f / 255f, 0f), Color.red,
            new(0f, 0f, 139f / 255f), new(165f / 255f, 42f / 255f, 42f / 255f), Color.cyan,
            Color.black, Color.gray
        };
    }
}
