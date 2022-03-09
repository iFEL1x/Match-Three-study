
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [SerializeField] private int xSize;
    [SerializeField] private int ySize;
    [SerializeField] private GameObject tile;
    [SerializeField] private List<Sprite> characters = new List<Sprite>();
    
    private GameObject[,] tiles;
    

    public bool IsShifting { get; set; }



    private void Awake()
    {
        Instance = this;
        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size; //Берем размер границ tile
        CreateBoard(offset.x, offset.y);
    }


    private void CreateBoard(float xOffset, float yOffset)
    {
        tiles = new GameObject[xSize, ySize];  //Задаем размер игрового поля
        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newTile = 
                    Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
                tiles[x, y] = newTile; //Положение в двумерном массиве

                newTile.transform.parent = transform; //Задаем\Перекладываем в родителя созданных tile

                List<Sprite> possibleCharackters = new List<Sprite>(); //Создаем новый лист
                possibleCharackters.AddRange(characters); //Помещаем в него всех наши "плитки"

                possibleCharackters.Remove(previousLeft[y]); //При полном цикле x, заполняем "таблицу эллементов" с лева (при первом проходе "таблица" пустая)
                possibleCharackters.Remove(previousBelow); //Всегда знаем наше последне значение

                Sprite newSprite = possibleCharackters[Random.Range(0, possibleCharackters.Count)]; //Выбираем рандомный спрайт
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite; //Устаналиваем рандомный спрайт на tile

                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }

    //////////////////////////////////////////////////////////////

    public IEnumerator FindNullTiles() //Ищем все пустые ячейки и проводим комбо
    {
        for (int x = 0; x < xSize; x++) //Поиск
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y)); //Проводим комбо
                    break;
                }
            }
        }

        for (int x = 0; x < xSize; x++) //Комбо
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].GetComponent<Tile>().ClearAllMatches();
            }
        }
    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f) //Сдвигаем ячейки вниз в пустые поля, оставляя сверх пустым.
    {
        int nullCount = 0;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();

        IsShifting = true;

        for (int y = yStart; y < ySize; y++)
        {
            SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
            if (render.sprite == null)
            {
                nullCount++; //Считаем количество пустых объектов, если комбо было по горизонтали, то это 1 объект, если по вертикале, от 3 - 5.
            }
            renders.Add(render); //Считаем количество объектов со Спрайтами.
        }

        for (int i = 0; i < nullCount; i++)
        {
            yield return new WaitForSeconds(shiftDelay);
            for (int k = 0; k < renders.Count - 1; k++)
            {
                renders[k].sprite = renders[k + 1].sprite; //Заполняем пустые плитки с плитками спрайта выше, обнуляя каждый верхний скопированный спрайт.
                renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
            }
            if (renders.Count == 1) //Проверка на последний эллемент в таблице, без него плитка будет пустая
            {
                for (int k = 0; k < renders.Count; k++)
                {
                    renders[k].sprite = GetNewSprite(x, ySize - 1);
                }
            }

        }
        IsShifting = false;
    }

    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleCharackters = new List<Sprite>();
        possibleCharackters.AddRange(characters);

        if(x > 0)
        {
            possibleCharackters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }

        if (x < xSize - 1)
        {
            possibleCharackters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }

        if( y > 0)
        {
            possibleCharackters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possibleCharackters[Random.Range(0, possibleCharackters.Count)];
    }
}
