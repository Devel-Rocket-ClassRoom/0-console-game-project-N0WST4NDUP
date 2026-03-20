using System;
using System.Reflection.Metadata;
using Framework.Engine;

public class PlayScene : Scene
{
    private Board _board;
    private PuyoPool _pool;

    private bool _inProgress;
    private bool _inChaining;
    private bool _isGameOver;

    private PuyoPair _pair;
    private bool _isVertical => _pair?.Pivot.Position.X == _pair?.Sub.Position.X; // true: 수직, false: 수평

    public override void Load()
    {
        _board = new(this);
        AddGameObject(_board);

        _pool = new(this);
        _pool.Initialize(100, _board);
    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        if (_isGameOver) return;

        HandleInput();
        UpdateGameObjects(deltaTime);

        if (_inProgress || _inChaining)
        {
            if (_pair.IsStuck())
            {
                // TODO: 연쇄 구현
                if (_pair.Pivot.ChainCount >= 4) _isGameOver = true;
                else if (_pair.Sub.ChainCount >= 4) _isGameOver = true;
                else _inProgress = false;
            }

        }
        else
        {
            _pair = _pool.GetNextPair();
            _pair.Pivot.SetPosition(_board.StartWidth + 2, _board.StartHeight).MoveFlag(true);
            _pair.Sub.SetPosition(_board.StartWidth + 3, _board.StartHeight).MoveFlag(true);
            _inProgress = true;
        }

    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (_pair is not null && !_pair.IsStuck())
        {
            if (_isVertical)
            {
                buffer.SetCell(_pair.Pivot.Position.X, _board.EndHeight - _board[_pair.Pivot.Position.X - _board.StartWidth].Count, '⊙', ConsoleColor.DarkGray);
                buffer.SetCell(_pair.Sub.Position.X, _board.EndHeight - _board[_pair.Sub.Position.X - _board.StartWidth].Count - 1, '⊙', ConsoleColor.DarkGray);
            }
            else
            {
                buffer.SetCell(_pair.Pivot.Position.X, _board.EndHeight - _board[_pair.Pivot.Position.X - _board.StartWidth].Count, '⊙', ConsoleColor.DarkGray);
                buffer.SetCell(_pair.Sub.Position.X, _board.EndHeight - _board[_pair.Sub.Position.X - _board.StartWidth].Count, '⊙', ConsoleColor.DarkGray);
            }
        }

        DrawGameObjects(buffer);
    }

    private void HandleInput()
    {
        if (Input.IsKeyDown(ConsoleKey.DownArrow)) _pair.MoveDown(_board);
        else if (Input.IsKeyDown(ConsoleKey.LeftArrow)) _pair.MoveLeft();
        else if (Input.IsKeyDown(ConsoleKey.RightArrow)) _pair.MoveRight();
        else if (Input.IsKeyDown(ConsoleKey.Z)) _pair.RotateCCW();
        else if (Input.IsKeyDown(ConsoleKey.X)) _pair.RotateCW();
    }
}