namespace CREBlazorApp.Pages;

public class Save
{
    public Save() { }

    public Save(int levelCount)
    {
        levelSaves = new LevelSave[levelCount];
        for (int i = 0; i < levelCount; i++)
            levelSaves[i] = new();
    }

    public LevelSave[] levelSaves = [];
}