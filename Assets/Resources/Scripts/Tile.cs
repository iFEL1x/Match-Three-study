using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private static Tile previousSelected = null;
    private LayerMask mask;

    private SpriteRenderer render;
    private bool isSelected = false;
    private bool matchFound = false;

    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };


    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        mask = LayerMask.GetMask("Tile");
    }

    //////////////////////////////////////////////////////////////

    private void Selected() 
    {
        isSelected = true;
        render.color = selectedColor;
        previousSelected = gameObject.GetComponent<Tile>();
        //SFXManager.Instance.PlaySFX(AnimationClip.Select);
    }

    private void Deselect()
    {
        isSelected = false;
        render.color = Color.white;
        previousSelected = null;
    }

    private void OnMouseDown()
    {
        if (render.sprite == null || BoardManager.Instance.IsShifting)
            return;

        if (isSelected) //Отменяем выделение объекта по выделенному объекту
        {
            Deselect();
        }

        else
        {
            if (previousSelected == null) //Нет выбранных объектов
            {
                Selected();
            }

            else
            {
                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); //Игнорируем родителя!

                if (GetAllAdjacentTiles().Contains(previousSelected.gameObject)) //Получаем список объектов в контейнере и сравниваем его с уже выбранным
                {
                    SwapSprite(previousSelected.render); //Меняем объекты местами
                    previousSelected.ClearAllMatches();
                    previousSelected.Deselect(); //Снимаем выделение (если был выбран другой объект)
                    ClearAllMatches();
                    GUIManager.Instance.MoveCounter--; //Уменьшаем колличество ходов
                }
                else
                {
                    previousSelected.GetComponent<Tile>().Deselect();
                    Selected();
                }
                gameObject.layer = LayerMask.NameToLayer("Tile");
            }
        }
    }

    //////////////////////////////////////////////////////////////

    public void SwapSprite(SpriteRenderer render2) //Меняем выделенные объекты местами если они были разные
    {
        if (render.sprite == render2.sprite)
            return;

        Sprite tempSprite = render2.sprite;
        render2.sprite = render.sprite;
        render.sprite = tempSprite;
        //SFXManager.Instance.PlaySFX(AnimationClip.Swap);
    }

    //////////////////////////////////////////////////////////////

    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), castDir, 1.0f, mask);

        if(hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }

    }



    private List<GameObject> GetAllAdjacentTiles() //Метод возвращает список объекто которые находятся вокруг выделенного объекта
    {
        List<GameObject> adjacentTiles = new List<GameObject>();

        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));

        }
        return adjacentTiles;
    }

    //////////////////////////////////////////////////////////////

    private List<GameObject> FindMatch(Vector2 castDir) //Ищем одинаковые плитки
    {
        List<GameObject> matchingTiles = new List<GameObject>();

        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); //Игнорируем родителя!
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), castDir, 1.0f, mask);
        gameObject.layer = LayerMask.NameToLayer("Tile");

        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite) //Если есть Тайл и он такой же, добавляем в массив
        {
            matchingTiles.Add(hit.collider.gameObject);

            hit.collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir, 1.0f, mask); //Проверяем нет ли еще одного такого Тайла рядом с найденным Тайлом, пока не найдет другой Тайл или его отсутствие

            //if (matchingTiles.Count > 5)
            //    break;
        }

        for (int i = 0; i < matchingTiles.Count; i++)
        {
            matchingTiles[i].gameObject.layer = LayerMask.NameToLayer("Tile");
        }

        return matchingTiles;
    }


    private void ClearMatch(Vector2[] paths) //Ищем одинаковые плитки и очищаем их
    {
        List<GameObject> matchingTiles = new List<GameObject>(); 
        for (int i = 0; i < paths.Length; i++) //Ищем
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }

        if(matchingTiles.Count >= 2) //Очищаем плитки и начисляем очки
        {
            for (int i = 0; i < matchingTiles.Count; i++) //Очищаем
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;

            if (matchingTiles.Count == 2) //Начисляем очки
                GUIManager.Instance.Score += 25; 
            if (matchingTiles.Count == 3)
                GUIManager.Instance.Score += 50;
            if (matchingTiles.Count == 4)
                GUIManager.Instance.Score += 100;
        }
    }


    public void ClearAllMatches()
    {
        if (render.sprite == null)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
        if (matchFound)
        {
            render.sprite = null;
            matchFound = false;
            StopCoroutine(BoardManager.Instance.FindNullTiles());
            StartCoroutine(BoardManager.Instance.FindNullTiles());
            //SFXManger.Instance.PlaySFX(AnimationClip.Clear);
        }
    }
}
