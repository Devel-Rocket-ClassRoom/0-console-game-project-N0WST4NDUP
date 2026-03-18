using Framework.Engine;

public class GameManager : GameApp
{
    private static GameApp _instance;
    public static GameApp Instance => _instance ??= new GameManager();

    private readonly SceneManager<Scene> _scenes = new();

    public GameManager(int width = 100, int height = 50) : base(width, height) { }

    protected override void Initialize()
    {
        ChangeToPlay();
    }

    protected override void Update(float deltaTime)
    {
    }

    protected override void Draw()
    {
    }

    private void ChangeToTitle()
    {
    }

    private void ChangeToPlay()
    {
        // PlayScene scene = new();
        // scene.PlayAgainRequested += ChangeToTitle;
        // _scenes.ChangeScene(scene);
    }
}