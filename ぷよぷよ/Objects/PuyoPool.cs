using System;
using System.Collections.Generic;
using Framework.Engine;

public class PuyoPool
{
    private int x, y;
    private Scene _scene;
    private List<Puyo> _pool; // 보드의 크기는 6*12 + 6*3 (내부 버퍼)이므로 최대 90개 + 여분
    private Random _random = new();

    private PuyoPair NextPair;

    public PuyoPool(Scene scene) { _scene = scene; }

    public void Initialize(int count, Board board)
    {
        _pool = new List<Puyo>(count);
        x = board.EndWidth + 4;
        y = board.StartHeight;

        for (int color = 0; color < 5; color++)
        {
            for (int i = 0; i < 20; i++)
            {
                _pool.Add(new Puyo(_scene, board, color).Reset(x, y));
            }
        }
        NextPair = new(GetRandomPuyo(true), GetRandomPuyo());
    }


    public PuyoPair GetNextPair()
    {
        var pair = NextPair;
        NextPair = new(GetRandomPuyo(true), GetRandomPuyo());

        return pair;
    }

    public void ReturnPuyo(Puyo puyo)
    {
        if (puyo is null) return;

        _pool.Add(puyo.Reset(x, y));
        _scene.RemoveGameObject(puyo);
    }

    private Puyo GetRandomPuyo(bool isPivot = false)
    {
        if (_pool is null || _pool?.Count == 0) return null;

        int idx = _random.Next(_pool.Count);
        Puyo puyo = _pool[idx];
        _pool.RemoveAt(idx);

        _scene.AddGameObject(puyo);

        return puyo.SetPosition(puyo.Position.X + (isPivot ? 0 : 1), puyo.Position.Y).PivotFlag(isPivot);
    }

    public void Clear()
    {
        _pool.Clear();
        NextPair = null;
    }
}