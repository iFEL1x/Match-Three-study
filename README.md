# <p align="center"> Match-3 DEMO</p>

<div align="Center">
    <img src = https://github.com/iFEL1x/iFEL1x/blob/main/Resources/Screenshots/Screen(Match-3)(0).png width="600">
</div>


## Описание проекта

Данный проект является изучением статьи 
***"How to Make a Match 3 Game in Unity"*** на сайте сообщества [Unity3DSchool.com](https://www.raywenderlich.com/)

Проект собран в Unity3D с использованием языка программирование C# и принципов ООП

___
## Скачивание и установка
Для того что бы запустить проект на своем ПК

* [Скачайте](https://unity3d.com/ru/get-unity/download) и [установите](https://docs.unity3d.com/2018.2/Documentation/Manual/InstallingUnity.html) Unity3D последней версии с официального сайта.
* Скачайте проект по [ссылке](https://github.com/iFEL1x/Platformer2D_Android_Demo_Level/archive/refs/heads/main.zip) или с текущей странице "Code\Download ZIP".
    + Распакуйте архив на своем ПК.
* Запустите Unity3D
    + Рядом с кнопкой "Open" нажмите на стрелочку :arrow_down_small:, в открывшимся списке выберете "Add project from disk"
    + Выберете путь распаковки проекта, нажмите "Add Project"

___
## В данном проекте применяется
* Взаимодействие объектов с Raycast.
* Массивы, списки с циклами.
* Последовательный вызов цепочки событий через анимацию объектов

*Демонсрация кода:*

```C#
    private void CreateBoard(float xOffset, float yOffset)
    {
        tiles = new GameObject[xSize, ySize];  
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
                tiles[x, y] = newTile; 

                newTile.transform.parent = transform;

                List<Sprite> possibleCharackters = new List<Sprite>(); //Создаем новый лист
                possibleCharackters.AddRange(characters);

                possibleCharackters.Remove(previousLeft[y]);
                possibleCharackters.Remove(previousBelow);

                Sprite newSprite = possibleCharackters[Random.Range(0, possibleCharackters.Count)];
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;

                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }
```

**Основная задача проекта** - Изучение возможностей Unity3D и языка программирования С# и принципов ООП.

*Демонстрация финальной части игрового процесса:*

![Mathch-3](https://github.com/iFEL1x/iFEL1x/blob/main/Resources/Image/Gif/mp4%20to%20GIH(Mathch-3).gif)
