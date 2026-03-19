using System;
using System.Collections.Generic;
using Framework.Engine;

public class Puyo : GameObject
{
    private (int X, int Y) _position;
    private bool _canMove;
    private bool _isPivot;
    private bool _isChained;

    public (int X, int Y) Position => _position;

    private readonly ConsoleColor[] _colorSet = { ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Magenta };
    public readonly ConsoleColor Color;
    public readonly LinkedList<Puyo> Chain = new();
    public int ChainCount => Chain.Count;

    public Puyo(Scene scene, int colorIndex) : base(scene)
    {
        Name = "Puyo";
        Color = _colorSet[colorIndex];
    }

    public override void Update(float deltaTime)
    {
        // if (!_canMove || _isBlocked) return;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.SetCell(
            _position.X, _position.Y,
            _isChained ? '■' : '●',
            _isPivot ? Color : Color - 8);
    }

    public (int x, int y) GetPositionFromOffset((int x, int y) offset)
    {
        int newX = _position.X + offset.x;
        int newY = _position.Y + offset.y;

        return (newX, newY);
    }

    public Puyo SetPosition(int x, int y, bool canMove = true)
    {
        _position = (x, y);
        _canMove = canMove;

        return this;
    }

    public Puyo SetPivotFlag(bool isPivot)
    {
        _isPivot = isPivot;

        return this;
    }

    public Puyo Reset(int x, int y)
    {
        _position = (x, y);
        _canMove = false;
        _isPivot = false;
        _isChained = false;
        Chain.Clear();

        return this;
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