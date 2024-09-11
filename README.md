# CodeWidget

Демонстрация исходного кода проекта в RealTime (типо) студентам для проведения практических занятий

Фишки апплета:
1. Подсветка синтаксиса кода
2. Возможность запрета копиирование кода студентам
3. Черный список папок и файлов с расширением для оптимизации поиска файлов
4. Гибкая настройка конфига
5. Не приходится поднимать OBS со стримом
6. Работает на костылях и коленках

# Быстрый старт

Соберите проект из исходников
```ps
dotnet publish -r win-x64 -c Release
```

Настройкте appsetings.json:
Админ пароль, теги отслеживания. Все остально необходимости.
```JSON
{
  "Password": "Qwerty",
  "tagForTrack" : "// track",
  "tagForTrackAndNoCopy" : "// nocopy",
  "HostPort" : 83,
  "TimerForFetchFiles" : 2000,
  "BlackContainerPaths": [".git", ".idea", "obj", "bin", ".vs"],
  "BlackContainerExtensions": ["exe", "db", "db-shm", "db-wal", "png", "ico", "jpg"]
}
```
Не рекомендуется и будьте осторожными:
1. Менять параметр TimerForFetchFiles ниже 1000 мс.
2. Сокращать черный список папой и файлов расширений BlackContainerPaths и BlackContainerExtensions

# Пример
![alt text](https://github.com/TheCrazyWolf/CodeWidget/blob/master/example.png?raw=true)

