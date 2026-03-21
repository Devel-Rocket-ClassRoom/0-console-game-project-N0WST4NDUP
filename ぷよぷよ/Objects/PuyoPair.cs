public class PuyoPair
{
    public readonly Puyo Pivot;
    public readonly Puyo Sub;

    private bool _isVertical => Pivot.Position.X == Sub.Position.X; // true: 수직, false: 수평

    public PuyoPair(Puyo pivot, Puyo sub)
    {
        this.Pivot = pivot;
        this.Sub = sub;
    }

    public bool IsStuck() => !Pivot.CanMove && !Sub.CanMove;

    public void MoveDown(Board board)
    {
        if (!Pivot.CanMove || !Sub.CanMove) return;

        Puyo targetPuyo;
        if (_isVertical) { targetPuyo = Pivot.Position.Y > Sub.Position.Y ? Pivot : Sub; }
        else { targetPuyo = board[Pivot.Line].Count > board[Sub.Line].Count ? Pivot : Sub; }

        var newPos = (x: targetPuyo.Position.X, y: targetPuyo.Position.Y + 1);
        if (targetPuyo.MoveCheck(newPos))
        {
            Pivot.SetPosition(Pivot.Position.X, Pivot.Position.Y + 1);
            Sub.SetPosition(Sub.Position.X, Sub.Position.Y + 1);
        }
    }

    public void MoveLeft()
    {
        if (!Pivot.CanMove || !Sub.CanMove) return;

        Puyo targetPuyo;
        if (_isVertical) { targetPuyo = Pivot.Position.Y > Sub.Position.Y ? Pivot : Sub; }
        else { targetPuyo = Pivot.Position.X < Sub.Position.X ? Pivot : Sub; }

        var newPos = (x: targetPuyo.Position.X - 1, y: targetPuyo.Position.Y);
        if (targetPuyo.MoveCheck(newPos))
        {
            Pivot.SetPosition(Pivot.Position.X - 1, Pivot.Position.Y);
            Sub.SetPosition(Sub.Position.X - 1, Sub.Position.Y);
        }
    }

    public void MoveRight()
    {
        if (!Pivot.CanMove || !Sub.CanMove) return;

        Puyo targetPuyo;
        if (_isVertical) { targetPuyo = Pivot.Position.Y > Sub.Position.Y ? Pivot : Sub; }
        else { targetPuyo = Pivot.Position.X < Sub.Position.X ? Sub : Pivot; }

        var newPos = (x: targetPuyo.Position.X + 1, y: targetPuyo.Position.Y);
        if (targetPuyo.MoveCheck(newPos))
        {
            Pivot.SetPosition(Pivot.Position.X + 1, Pivot.Position.Y);
            Sub.SetPosition(Sub.Position.X + 1, Sub.Position.Y);
        }
    }

    public void RotateCCW() // 반시계
    {
        if (!Pivot.CanMove || !Sub.CanMove) return;

        int dx = Pivot.Position.X - Sub.Position.X;
        int dy = Pivot.Position.Y - Sub.Position.Y;
        var rotated = (-dy, dx);
        var newSubPos = Pivot.GetPositionFromOffset(rotated);

        if (Sub.MoveCheck(newSubPos))
        {
            Sub.SetPosition(newSubPos.x, newSubPos.y);
        }
        else
        {
            var kicks = new[] { (1, 0), (-1, 0), (1, 1), (-1, 1), (1, -1), (-1, -1) };
            foreach (var (kx, ky) in kicks)
            {
                var kick = (x: newSubPos.x + kx, y: newSubPos.y + ky);
                if (Pivot.MoveCheck(kick))
                {
                    Pivot.SetPosition(Pivot.Position.X + kx, Pivot.Position.Y + ky);
                    Sub.SetPosition(newSubPos.x + kx, newSubPos.y + ky);
                    return;
                }
            }
        }
    }

    public void RotateCW() // 시계
    {
        if (!Pivot.CanMove || !Sub.CanMove) return;

        int dx = Pivot.Position.X - Sub.Position.X;
        int dy = Pivot.Position.Y - Sub.Position.Y;
        var rotated = (dy, -dx);
        var newSubPos = Pivot.GetPositionFromOffset(rotated);

        if (Sub.MoveCheck(newSubPos))
        {
            Sub.SetPosition(newSubPos.x, newSubPos.y);
        }
        else
        {
            var kicks = new[] { (-1, 0), (1, 0), (-1, 1), (1, 1), (-1, -1), (1, -1) };
            foreach (var (kx, ky) in kicks)
            {
                var kick = (x: newSubPos.x + kx, y: newSubPos.y + ky);
                if (Pivot.MoveCheck(kick))
                {
                    Pivot.SetPosition(Pivot.Position.X + kx, Pivot.Position.Y + ky);
                    Sub.SetPosition(newSubPos.x + kx, newSubPos.y + ky);
                    return;
                }
            }
        }
    }
}