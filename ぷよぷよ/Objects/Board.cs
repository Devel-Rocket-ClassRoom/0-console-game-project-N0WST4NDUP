using System;
using System.Collections.Generic;
using Framework.Engine;

public class Board : GameObject
{
    private const int k_x = 4, k_y = 0;
    private const int k_width = 6, k_height = 15; // 뷰 12칸 + 내부 버퍼 3칸
    private List<Puyo>[] _top = new List<Puyo>[k_width]; // 각 열의 puyo가 쌓인 높이 (0부터 시작)

    public int StartWidth => k_x;
    public int StartHeight => k_y + 3; // 내부 버퍼 제외한 실제 보드의 시작 높이
    public int EndWidth => k_x + k_width - 1;
    public int EndHeight => k_y + k_height - 1; // 내부 버퍼 제외한 실제 보드의 끝 높이

    public Board(Scene scene) : base(scene)
    {
        Name = "Board";

        for (int i = 0; i < _top.Length; i++)
        {
            _top[i] = new List<Puyo>();
        }
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(StartWidth - 1, StartHeight - 1, k_width + 2, k_height - StartHeight + 2, ConsoleColor.White);
        // for (int i = 0; i < _top.Length; i++)
        // {
        //     buffer.SetCell(StartWidth + i, EndHeight - _top[i] - 1, '⊙', ConsoleColor.DarkGray);
        // }
    }

    public List<Puyo> this[int index] => _top[index];

    public bool CanPlacePuyo((int x, int y) position)
    {
        if (position.x < StartWidth || position.x > EndWidth || position.y < StartHeight - 3 || position.y > EndHeight) return false;

        int column = position.x - StartWidth;
        int height = EndHeight - _top[column].Count;

        return position.y <= height;
    }
}