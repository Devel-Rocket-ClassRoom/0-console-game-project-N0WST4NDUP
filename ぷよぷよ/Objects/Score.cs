using System;
using Framework.Engine;

public class Score : GameObject
{
    private int _x, _y;
    private int _score;
    private int _maxScore;

    public Score(Scene scene, int x, int y) : base(scene)
    {
        _x = x;
        _y = y;
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.WriteText(_x, _y, $"점수:{_score}");
        buffer.WriteText(_x, _y + 1, $"최고 점수:{_score}");
    }

    public void Initialize()
    {
        _score = 0;
    }

    public void SetScore(int chainCount, int processCount)
    {
        _score += (2 + chainCount) * processCount;
        _maxScore = Math.Max(_score, _maxScore);
    }
}