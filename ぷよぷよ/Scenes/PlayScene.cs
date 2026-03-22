using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Engine;

public class PlayScene : Scene
{
    private Board _board;
    private PuyoPool _pool;
    private Score _score;

    private bool _inProgress;
    private bool _isGameOver;

    private PuyoPair _pair;
    private bool _isVertical => _pair?.Pivot.Position.X == _pair?.Sub.Position.X; // true: 수직, false: 수평

    public static HashSet<LinkedList<Puyo>> ChainOfPuyos = new();
    private bool[] _afterProcessLine = new bool[6];
    private int _processCount = 0;

    public override void Load()
    {
        _board = new(this);
        AddGameObject(_board);

        _score = new(this, _board.EndWidth + 3, _board.StartHeight + 3);
        _score.Initialize();
        AddGameObject(_score);

        _pool = new(this);
        _pool.Initialize(100, _board);
    }

    public override void Unload()
    {
        _board = null;
        _pool = null; // 안에 있는 오브젝트들도 밑에서 참조 끊길 테니 이렇게만
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        if (_isGameOver) return;

        HandleInput();
        UpdateGameObjects(deltaTime);

        if (_inProgress)
        {
            if (Puyo.s_MovedCount > 0)
            {
            }
            else if (ChainOfPuyos.Count > 0)
            {
                _processCount++;

                foreach (var chain in ChainOfPuyos)
                {
                    _score.SetScore(chain.Count, _processCount);

                    foreach (var puyo in chain)
                    {
                        _board[puyo.Line].Remove(puyo);
                        _afterProcessLine[puyo.Line] = true;
                        _pool.ReturnPuyo(puyo);
                    }
                    ChainOfPuyos.Remove(chain);
                }

                for (int i = 0; i < _afterProcessLine.Length; i++)
                {
                    if (_afterProcessLine[i])
                    {
                        foreach (var puyo in _board[i].ToList())
                        {
                            puyo.MoveFlag(true);
                        }
                    }
                    _afterProcessLine[i] = false;
                }
            }
            else
            {
                _inProgress = false;
                _processCount = 0;
            }

        }
        else
        {
            if (_pair?.Pivot.Position.Y < _board.StartHeight || _pair?.Sub.Position.Y < _board.StartHeight)
            {
                _isGameOver = true;
            }

            _pair = _pool.GetNextPair();
            _pair.Pivot.SetPosition(_board.StartWidth + 2, _board.StartHeight - 3).MoveFlag(true);
            _pair.Sub.SetPosition(_board.StartWidth + 3, _board.StartHeight - 3).MoveFlag(true);
            _inProgress = true;
        }

    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (_pair is not null && _pair.Pivot.CanMove && _pair.Sub.CanMove)
        {
            buffer.SetCell(
                    _pair.Pivot.Position.X, _board.EndHeight - _board[_pair.Pivot.Line].Count, '⊙', ConsoleColor.DarkGray);
            if (_isVertical)
            {
                buffer.SetCell(
                    _pair.Sub.Position.X, _board.EndHeight - _board[_pair.Sub.Line].Count - 1, '⊙', ConsoleColor.DarkGray);
            }
            else
            {
                buffer.SetCell(
                    _pair.Sub.Position.X, _board.EndHeight - _board[_pair.Sub.Line].Count, '⊙', ConsoleColor.DarkGray);
            }
        }
        buffer.WriteText(_board.EndWidth + 3, _board.EndHeight - 2, "이동:←,→", ConsoleColor.White);
        buffer.WriteText(_board.EndWidth + 3, _board.EndHeight - 1, "회전:Z,X", ConsoleColor.White);
        buffer.WriteText(_board.EndWidth + 3, _board.EndHeight, "종료:ESC", ConsoleColor.White);

        DrawGameObjects(buffer);

        // 내부 버퍼공간 가리기용
        buffer.DrawBox(_board.StartWidth, _board.StartHeight - 3, 6, 2, ' ', ConsoleColor.White);
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