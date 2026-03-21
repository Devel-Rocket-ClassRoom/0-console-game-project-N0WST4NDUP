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
    private float _moveTimer;
    private bool _canMove;
    private bool _isPivot;

    public (int X, int Y) Position => _position;
    private LinkedList<Puyo> _chain;
    public int Line => _position.X - _board.StartWidth;

    public bool CanMove => _canMove;
    public int ChainCount => _chain?.Count ?? 1;
    public static int s_MovedCount { get; private set; }

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
            SetPosition(newPos).NextStepCheck();
        }
        else
        {
            _board[Position.X - _board.StartWidth].Add(this);
            PivotFlag(false);
            MoveFlag(false);
        }

        _moveTimer = 0;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.SetCell(
            _position.X, _position.Y,
            // ChainCount > 1 ? ChainCount < 4 ? '■' : ' ' : '●',
            ChainCount > 1 ? '■' : '●',
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

    public void ChainingTo(Puyo target)
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

    public Puyo NextStepCheck()
    {
        var nextStep = (_position.X, _position.Y + 1);
        if (!_board.CanPlacePuyo(nextStep))
        {
            _board[Position.X - _board.StartWidth].Add(this);
            PivotFlag(false);
            MoveFlag(false);
        }

        return this;
    }

    public Puyo MoveFlag(bool canMove = true)
    {
        _canMove = canMove;

        if (_canMove)
        {
            _board[Line].Remove(this);
            _chain?.Remove(this);
            _chain = null;
            s_MovedCount++;
        }
        else
        {
            _chain ??= new LinkedList<Puyo>();
            _chain.AddLast(this);

            (int x, int y)[] offset = { (0, 1), (-1, 0), (1, 0) }; // 아래, 왼쪽, 오른쪽
            foreach (var off in offset)
            {
                var position = GetPositionFromOffset(off);
                //해당 위치가 보드 밖이면 패스
                if (position.x < _board.StartWidth || position.x > _board.EndWidth ||
                    position.y < _board.StartHeight || position.y > _board.EndHeight) continue;

                var targetLine = _board[position.x - _board.StartWidth];
                var targetIdx = _board.EndHeight - position.y;
                // 해당 위치에 puyo 없으면 패스
                if (targetIdx >= targetLine.Count) continue;

                var targetPuyo = targetLine[targetIdx];
                // 색상이 다르면 패스
                if (targetPuyo.Color != this.Color) continue;

                // 같은 참조면 패스
                if (targetPuyo._chain == this._chain) continue;
                if (targetPuyo.ChainCount > this.ChainCount) // 체인 수가 더 많은 쪽에 붙기
                {
                    ChainingTo(targetPuyo);
                }
                else
                {
                    _chain.AddLast(targetPuyo); // 내가 가지고 있는 체인에 타겟 추가 후
                    targetPuyo._chain = _chain; // 참조 복사
                }
            }

            if (_chain.Count >= 4) PlayScene.ChainOfPuyos.Add(_chain);

            s_MovedCount--;
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
        _chain = null;

        return this;
    }

}