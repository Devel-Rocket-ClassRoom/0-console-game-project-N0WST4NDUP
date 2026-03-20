using System;
using System.Collections.Generic;
using Framework.Engine;

public class Puyo : GameObject
{
    private const float k_moveInterval = 0.5f;
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
    public int ChainCount => _chain?.Count ?? 1;

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

        var newPos = (_position.X, _position.Y + 1);
        if (MoveCheck(newPos))
        {
            SetPosition(newPos);
        }
        else
        {
            MoveFlag(false);
            PivotFlag(false);
            _board[Position.X - _board.StartWidth].Add(this);
        }

        _moveTimer = 0;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.SetCell(
            _position.X, _position.Y,
            ChainCount > 1 ? ChainCount < 4 ? '■' : '※' : '●',
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

    public void Chaining(Puyo target)
    {
        foreach (var puyo in _chain)
        {
            if (!target._chain.Contains(puyo))
            {
                puyo._chain = target._chain;
                target._chain.AddLast(puyo);
            }
        }
        _chain = target._chain;
    }

    public Puyo MoveFlag(bool canMove = true)
    {
        _canMove = canMove;

        if (_canMove)
        {
            _chain?.Remove(this);
            _chain = null;
        }
        else
        {
            _chain ??= new LinkedList<Puyo>();
            _chain.AddLast(this);

            (int x, int y)[] offset = { (0, 1), (-1, 0), (1, 0) }; // 아래, 왼쪽, 오른쪽
            foreach (var off in offset)
            {
                var position = GetPositionFromOffset(off);
                if (position.x < _board.StartWidth || position.x > _board.EndWidth || position.y < _board.StartHeight || position.y > _board.EndHeight) continue;

                var targetLine = _board[position.x - _board.StartWidth];
                var targetIdx = _board.EndHeight - position.y;
                if (targetIdx >= targetLine.Count) continue; // 해당 위치에 puyo 없으면 패스

                var targetPuyo = targetLine[targetIdx];
                if (targetPuyo.Color != this.Color) continue; // 색상이 다르면 패스

                if (targetPuyo.ChainCount > this.ChainCount) // 체인 수가 더 많은 쪽에 붙기
                {
                    Chaining(targetPuyo);
                }
                else
                {
                    targetPuyo.Chaining(this);
                }
            }
        }

        return this;
    }

    public Puyo PivotFlag(bool isPivot = true)
    {
        _isPivot = isPivot;

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

}