using System;
using Framework.Engine;

public partial class TitleScene : Scene
{
    public event GameAction StartRequested;

    public override void Load()
    {
    }

    public override void Unload()
    {
    }

    public override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Enter)) StartRequested?.Invoke();
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.WriteText(8, 5, "ぷ요ぷ요", ConsoleColor.Yellow);
        buffer.WriteText(8, 6, "뿌よ뿌よ", ConsoleColor.Yellow);
        buffer.WriteText(4, 9, "ENTER to Start.");
    }
}