using Framework.Engine;

public class Puyo : GameObject
{
    private (int X, int Y) _position;
    private bool _isPivot;
    private bool _isBlocked;
    private bool _isChained;

    public readonly ConsoleColor Color;
    public readonly List<Puyo> Chain = new();
    public int ChainCount
    {
        get
        {
            int sum = Chain.Count;
            foreach (var chained in Chain)
            {
                sum += chained.ChainCount;
            }
            return sum;
        }
    }

    public Puyo(Scene scene, int x, int y, bool isPivot) : base(scene)
    {
        Name = "Puyo";
        _position = (x, y);
        _isPivot = isPivot;
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.SetCell(
            _position.X, _position.Y,
            _isChained ? '■' : '●',
            ConsoleColor.Magenta);
    }

    public static bool operator ==(Puyo a, Puyo b)
    {
        if (a is null)
        {
            if (b is null)
            {
                return true;
            }

            return false;
        }
        return a.Equals(b);
    }
    public static bool operator !=(Puyo a, Puyo b) => !(a == b);
    public override bool Equals(object? obj) => Color.Equals((obj as Puyo)?.Color);
    public override int GetHashCode() => Color.GetHashCode();
}