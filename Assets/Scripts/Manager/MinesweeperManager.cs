using NUnit.Framework;
using TMPro;
using UnityEngine;

namespace SpellOfLust
{
    public class MinesweeperManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _mainContainer;

        [SerializeField]
        private GameObject _hLinePrefab, _tilePrefab;

        private TileData[,] _grid;

        private const int Size = 20;
        private const int MineCount = 30;

        private void Awake()
        {
            Assert.True(MineCount <= Size * Size);
            _grid = new TileData[Size, Size];

            for (int y = 0; y < Size; y++)
            {
                var line = Instantiate(_hLinePrefab, _mainContainer);
                for (int x = 0; x < Size; x++)
                {
                    var go = Instantiate(_tilePrefab, line.transform);
                    var data = new TileData(go);

                    _grid[x, y] = data;
                }
            }

            int mineLeft = MineCount;
            while (mineLeft > 0)
            {
                var randX = Random.Range(0, Size);
                var randY = Random.Range(0, Size);

                var tile = _grid[randX, randY];
                if (tile.HasMine) continue;

                tile.HasMine = true;
                tile.Text.text = "X";
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
                    _grid[x, y].Text.text = count.ToString();
                }
            }
        }
    }

    public class TileData
    {
        public TileData(GameObject go)
        {
            Go = go;
            Text = Go.GetComponentInChildren<TMP_Text>();
        }

        public GameObject Go { private set; get; }
        public TMP_Text Text { private set; get; }
        public int AdjacentMines { set; get; }
        public bool HasMine { set; get; }
    }
}
