using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace SpellOfLust
{
    public class MinesweeperManager : MonoBehaviour
    {
        public static MinesweeperManager Instance { private set; get; }

        [SerializeField]
        private Transform _mainContainer;

        [SerializeField]
        private GameObject _hLinePrefab, _tilePrefab;

        private TileData[,] _grid;

        private const int Size = 20;
        private const int MineCount = 30;

        private bool _isGenerated = false;

        private void Awake()
        {
            Instance = this;

            Assert.True(MineCount <= Size * Size);
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
    }

    public class TileData
    {
        public TileData(GameObject go, int x, int y)
        {
            Go = go;
            _x = x;
            _y = y;
            _text = Go.GetComponentInChildren<TMP_Text>();
            _button = Go.GetComponent<MineButton>();
            _button.OnLeftClick.AddListener(Click);
            _button.OnRightClick.AddListener(Flag);
        }

        private void PreCheck()
        {
            MinesweeperManager.Instance.GenerateIfNeeded(_x, _y);
        }

        public void Click()
        {
            PreCheck();
            if (!HasFlag)
            {
                if (HasMine)
                {
                    throw new System.Exception("Game lost");
                }
                MinesweeperManager.Instance.ShowContent(_x, _y);
            }
        }

        public void Show()
        {
            IsShown = true;
            _button.Disable();

            if (HasMine)
            {
                _text.text = "X";
            }
            else if (AdjacentMines > 0)
            {
                _text.text = $"<color=#{_colors[AdjacentMines - 1].ToHexString()}>{AdjacentMines}</color>";
            }
        }

        public void Flag()
        {
            PreCheck();

            HasFlag = !HasFlag;
            _button.Flagged = HasFlag;

            if (MinesweeperManager.Instance.IsGameWon())
            {
                Debug.Log("You won!");
            }
        }

        public bool DoesSatisfyVictory()
        {
            return (HasFlag && HasMine) || (!HasFlag && !HasMine);
        }

        private int _x, _y;
        public GameObject Go { private set; get; }
        private readonly TMP_Text _text;
        private readonly MineButton _button;
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
