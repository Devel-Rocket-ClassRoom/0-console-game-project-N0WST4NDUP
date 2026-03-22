using System;
using Framework.Engine;

public class PuyoPuyoApp : GameApp
{
    private static GameApp _instance;
    public static GameApp Instance => _instance ??= new PuyoPuyoApp();

    private readonly SceneManager<Scene> _scenes = new();

    public PuyoPuyoApp(int width = 100, int height = 20) : base(width, height) { }

    protected override void Initialize()
    {
        ChangeToTitle();
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
        TitleScene scene = new();
        scene.StartRequested += ChangeToPlay;
        _scenes.ChangeScene(scene);
    }

    private void ChangeToPlay()
    {
        PlayScene scene = new();
        _scenes.ChangeScene(scene);
    }

}