using System;
using System.Collections.Generic;
using Framework.Engine;

public class PuyoPool : GameObject
{
    private int _x, _y; // 보드 위치랑 상호작용해서 위치 조정할 예정
    private int _width = 4, _height = 3;
    private List<Puyo> _pool = new(100); // 보드의 크기는 6*12 + 6*3 (내부 버퍼)이므로 최대 90개 + 여분
    private Random _random = new();

    private (Puyo pivot, Puyo sub) NextPair;

    public PuyoPool(Scene scene, int x, int y) : base(scene)
    {
        _x = x;
        _y = y;
        for (int color = 0; color < 5; color++)
        {
            for (int i = 0; i < 20; i++)
            {
                _pool.Add(new Puyo(scene, color).Reset(0, 0));
            }
        }
    }

    public override void Update(float deltaTime)
    {
        if (NextPair.pivot is null) NextPair.pivot = GetRandomPuyo(true);
        if (NextPair.sub is null) NextPair.sub = GetRandomPuyo();
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(_x, _y, _width, _height, ConsoleColor.White);
    }

    public (Puyo pivot, Puyo sub) GetNextPair()
    {
        var pair = NextPair;
        NextPair = (null, null);

        return pair;
    }

    private Puyo GetRandomPuyo(bool isPivot = false)
    {
        if (_pool.Count == 0) return null;

        int idx = _random.Next(_pool.Count);
        Puyo puyo = _pool[idx];
        _pool.RemoveAt(idx);

        this.Scene.AddGameObject(puyo);

        return puyo.SetPosition(_x + (isPivot ? 1 : 2), _y + 1, false).SetPivotFlag(isPivot);
    }
}