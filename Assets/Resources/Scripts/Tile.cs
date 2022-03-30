/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    List<GameObject> matchingTiles = new List<GameObject>();

    //private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private static Tile previousSelected = null;
    private LayerMask mask;

    private Animator animator;
    private SpriteRenderer render;
    private bool isSelected = false;
    private bool matchFound = false;

    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private void Awake()
    {
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        mask = LayerMask.GetMask("Tile");
    }

    //////////////////////////////////////////////////////////////

    private void Selected() 
    {
        isSelected = true;
        //render.color = selectedColor;
        animator.SetBool("Selected", true);
        previousSelected = gameObject.GetComponent<Tile>();
        //SFXManager.Instance.PlaySFX(AnimationClip.Select);
    }

    private void Deselect()
    {
        isSelected = false;

        previousSelected.animator.SetBool("Selected", false);
        //render.color = Color.white;
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

    ////////////////////////////////////////////////////////////////

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

        if (matchingTiles.Count >= 2) //Очищаем плитки и начисляем очки
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
