using System;
using System.Collections.Generic;
using Framework.Engine;

public class Puyo : GameObject
{
    private const float k_moveInterval = 1.0f;
    private readonly Board _board;
    private readonly ConsoleColor[] _colorSet = {
        ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Magenta
    };
    public readonly ConsoleColor Color;

    private (int X, int Y) _position;
    private double _moveTimer;
    private bool _canMove;
    private bool _isPivot;

    public (int X, int Y) Position => _position;
    private LinkedList<Puyo> _chain;

    public bool CanMove => _canMove;
    public int ChainCount => _chain?.Count ?? 0;

    public Puyo(Scene scene, Board board, int colorIndex) : base(scene)
    {
        Name = "Puyo";
        Color = _colorSet[colorIndex];
        _board = board;
    }

    public override void Update(float deltaTime)
    {
        if (!_canMove) return;

        _moveTimer += deltaTime;
        if (_moveTimer < k_moveInterval) return;

        var newPos = (_position.X, y: _position.Y + 1);
        if (MoveCheck(newPos))
        {
            SetPosition(_position.X, _position.Y + 1);
        }
        else
        {
            _canMove = false;
            _isPivot = false;
            _board[Position.X - _board.StartWidth]++;
        }

        _moveTimer = 0;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.SetCell(
            _position.X, _position.Y,
            ChainCount > 0 ? '■' : '●',
            _isPivot ? Color : Color - 8);
    }

    public bool MoveCheck((int x, int y) position) => _board.CanPlacePuyo(position);

    public (int x, int y) GetPositionFromOffset((int x, int y) offset) => (Position.X + offset.x, Position.Y + offset.y);

    public Puyo SetPosition(int x, int y) => SetPosition((x, y));
    public Puyo SetPosition((int x, int y) position)
    {
        _position = position;

        return this;
    }

    public Puyo MoveFlag(bool canMove = true)
    {
        _canMove = canMove;

        if (_canMove)
        {
            _chain?.Remove(this);
            _chain = null;
        }

        return this;
    }

    public Puyo PivotFlag(bool isPivot = true)
    {
        _isPivot = isPivot;

        return this;
    }

    public Puyo Chaining(Puyo puyo)
    {


        return this;
    }

    public Puyo Reset(int x = 0, int y = 0)
    {
        _position = (x, y);
        _canMove = false;
        _isPivot = false;
        _chain?.Clear();

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