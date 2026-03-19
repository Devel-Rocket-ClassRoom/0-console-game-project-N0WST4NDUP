using System;
using System.Reflection.Metadata;
using Framework.Engine;

public class PlayScene : Scene
{
    private const float k_MoveInterval = 0.3f;
    private Board _board;
    private PuyoPool _pool;

    private float _moveTimer;

    private bool _inProgress;
    private bool _isPlacing;
    private bool _isGameOver;

    private (Puyo pivot, Puyo sub) _pair = (null, null);
    private bool _isVertical => _pair.pivot?.Position.X == _pair.sub?.Position.X; // true: 수직, false: 수평

    public override void Load()
    {
        _board = new(this);
        AddGameObject(_board);

        _pool = new(this, _board.EndWidth + 3, _board.StartHeight - 1);
        AddGameObject(_pool);
    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        if (_isGameOver) return;

        HandleInput();

        _moveTimer += deltaTime;
        if (_moveTimer < k_MoveInterval) return;

        UpdateGameObjects(deltaTime);

        if (_inProgress)
        {
            if (_isPlacing)
            {
                Move();
            }
            else // 터지는 동작
            {
                _inProgress = false;
            }
        }
        else
        {
            _pair = _pool.GetNextPair();
            _pair.pivot.SetPosition(_board.StartWidth + 2, _board.StartHeight);
            _pair.sub.SetPosition(_board.StartWidth + 3, _board.StartHeight);
            _inProgress = true;
            _isPlacing = true;
        }

        _moveTimer = 0;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (_pair.pivot is null || _pair.sub is null) return;

        if (_isVertical)
        {
            buffer.SetCell(_pair.pivot.Position.X, _board.EndHeight - _board[_pair.pivot.Position.X - _board.StartWidth], '⊙', ConsoleColor.DarkGray);
            buffer.SetCell(_pair.sub.Position.X, _board.EndHeight - _board[_pair.sub.Position.X - _board.StartWidth] - 1, '⊙', ConsoleColor.DarkGray);
        }
        else
        {
            buffer.SetCell(_pair.pivot.Position.X, _board.EndHeight - _board[_pair.pivot.Position.X - _board.StartWidth], '⊙', ConsoleColor.DarkGray);
            buffer.SetCell(_pair.sub.Position.X, _board.EndHeight - _board[_pair.sub.Position.X - _board.StartWidth], '⊙', ConsoleColor.DarkGray);
        }

        DrawGameObjects(buffer);
    }

    private void Move()
    {
        if (_pair.pivot is null || _pair.sub is null) return;

        Puyo targetPuyo;
        if (_isVertical) { targetPuyo = _pair.pivot.Position.Y > _pair.sub.Position.Y ? _pair.pivot : _pair.sub; }
        else { targetPuyo = _board[_pair.pivot.Position.X - _board.StartWidth] > _board[_pair.sub.Position.X - _board.StartWidth] ? _pair.pivot : _pair.sub; }

        var newPos = (x: targetPuyo.Position.X, y: targetPuyo.Position.Y + 1);
        if (_board.CanPlacePuyo(newPos))
        {
            _pair.pivot.SetPosition(_pair.pivot.Position.X, _pair.pivot.Position.Y + 1);
            _pair.sub.SetPosition(_pair.sub.Position.X, _pair.sub.Position.Y + 1);
        }
        else
        {
            _isPlacing = false;
            _pair.pivot.SetPosition(_pair.pivot.Position.X, _pair.pivot.Position.Y, canMove: false);
            _pair.sub.SetPosition(_pair.sub.Position.X, _pair.sub.Position.Y, canMove: false);
            _board[_pair.pivot.Position.X - _board.StartWidth]++; // 해당 열의 높이 증가
            _board[_pair.sub.Position.X - _board.StartWidth]++; // 해당 열의 높이 증가
        }
    }

    private void HandleInput()
    {
        if (!_isPlacing) return;

        if (Input.IsKeyDown(ConsoleKey.LeftArrow))
        {
            if (_pair.pivot is null || _pair.sub is null) return;

            Puyo targetPuyo;
            if (_isVertical) { targetPuyo = _pair.pivot.Position.Y > _pair.sub.Position.Y ? _pair.pivot : _pair.sub; }
            else { targetPuyo = _pair.pivot.Position.X < _pair.sub.Position.X ? _pair.pivot : _pair.sub; }

            var newPos = (x: targetPuyo.Position.X - 1, y: targetPuyo.Position.Y);
            if (_board.CanPlacePuyo(newPos))
            {
                _pair.pivot.SetPosition(_pair.pivot.Position.X - 1, _pair.pivot.Position.Y);
                _pair.sub.SetPosition(_pair.sub.Position.X - 1, _pair.sub.Position.Y);
            }
        }
        else if (Input.IsKeyDown(ConsoleKey.RightArrow))
        {
            if (_pair.pivot is null || _pair.sub is null) return;

            Puyo targetPuyo;
            if (_isVertical) { targetPuyo = _pair.pivot.Position.Y > _pair.sub.Position.Y ? _pair.pivot : _pair.sub; }
            else { targetPuyo = _pair.pivot.Position.X > _pair.sub.Position.X ? _pair.pivot : _pair.sub; }

            var newPos = (x: targetPuyo.Position.X + 1, y: targetPuyo.Position.Y);
            if (_board.CanPlacePuyo(newPos))
            {
                _pair.pivot.SetPosition(_pair.pivot.Position.X + 1, _pair.pivot.Position.Y);
                _pair.sub.SetPosition(_pair.sub.Position.X + 1, _pair.sub.Position.Y);
            }
        }
        else if (Input.IsKey(ConsoleKey.DownArrow))
        {
            // TODO: 속도 증가
        }
        else if (Input.IsKeyDown(ConsoleKey.Z))
        {
            if (_pair.pivot is null || _pair.sub is null) return;

            RotateCCW();
        }
        else if (Input.IsKeyDown(ConsoleKey.X))
        {
            if (_pair.pivot is null || _pair.sub is null) return;

            RotateCW();
        }
    }

    private void RotateCCW() // 반시계
    {
        int dx = _pair.pivot.Position.X - _pair.sub.Position.X;
        int dy = _pair.pivot.Position.Y - _pair.sub.Position.Y;
        var rotated = (-dy, dx);
        var newSubPos = _pair.pivot.GetPositionFromOffset(rotated);

        if (_board.CanPlacePuyo(newSubPos))
        {
            _pair.sub.SetPosition(newSubPos.x, newSubPos.y);
        }
        else
        {
            var kicks = new[] { (1, 0), (-1, 0), (0, -1), (1, 1), (-1, -1) };
            foreach (var (kx, ky) in kicks)
            {
                var kick = (x: newSubPos.x + kx, y: newSubPos.y + ky);
                if (_board.CanPlacePuyo(kick))
                {
                    _pair.pivot.SetPosition(_pair.pivot.Position.X + kx, _pair.pivot.Position.Y + ky);
                    _pair.sub.SetPosition(newSubPos.x + kx, newSubPos.y + ky);
                    return;
                }
            }
        }
    }

    private void RotateCW() // 시계
    {
        int dx = _pair.pivot.Position.X - _pair.sub.Position.X;
        int dy = _pair.pivot.Position.Y - _pair.sub.Position.Y;
        var rotated = (dy, -dx);
        var newSubPos = _pair.pivot.GetPositionFromOffset(rotated);

        if (_board.CanPlacePuyo(newSubPos))
        {
            _pair.sub.SetPosition(newSubPos.x, newSubPos.y);
        }
        else
        {
            var kicks = new[] { (-1, 0), (1, 0), (0, -1), (1, 1), (-1, -1) };
            foreach (var (kx, ky) in kicks)
            {
                var kick = (x: newSubPos.x + kx, y: newSubPos.y + ky);
                if (_board.CanPlacePuyo(kick))
                {
                    _pair.pivot.SetPosition(_pair.pivot.Position.X + kx, _pair.pivot.Position.Y + ky);
                    _pair.sub.SetPosition(newSubPos.x + kx, newSubPos.y + ky);
                    return;
                }
            }
        }
    }
}