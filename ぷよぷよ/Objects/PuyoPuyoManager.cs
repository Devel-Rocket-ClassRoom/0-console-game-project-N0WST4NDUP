using System;
using Framework.Engine;

public class PuyoPuyoManager : GameApp
{
    private static GameApp _instance;
    public static GameApp Instance => _instance ??= new PuyoPuyoManager();

    private readonly SceneManager<Scene> _scenes = new();

    public PuyoPuyoManager(int width = 50, int height = 20) : base(width, height) { }

    protected override void Initialize()
    {
        ChangeToPlay();
    }

    protected override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Escape))
        {
            Quit();
            return;
        }

        _scenes.CurrentScene?.Update(deltaTime);
    }

    protected override void Draw()
    {
        _scenes.CurrentScene?.Draw(Buffer);
    }

    private void ChangeToTitle()
    {
    }

    private void ChangeToPlay()
    {
        PlayScene scene = new();
        _scenes.ChangeScene(scene);
    }

}