using Godot;
using System;

public class Cell : Sprite
{
    private Sprite Mark;
    private bool HasMark;
    [Signal] public delegate void ClickSignal(Cell _cell);
    public int row;
    public int col;

    public override void _Ready()
    {
        HasMark = false;
        Texture = ResourceLoader.Load<Texture>("res://Assets/marks.png");
        Mark = GetNode<Sprite>("Mark");
    }

    public void SetRow(int _row)
    {
        row = _row;
    }

    public void SetCol(int _col)
    {
        col = _col;
    }

    public void MarkCell(bool _itsX)
    {
        Mark.Frame = _itsX ? 1 : 2;
    }

    public override void _Input(InputEvent @event)
    {
        if (HasMark)
        {
            return;
        }
        else if (@event is InputEventMouseButton btn && 
            btn.ButtonIndex == (int)ButtonList.Left && 
            btn.Pressed)
        {
            if (GetRect().HasPoint(ToLocal(btn.Position)))
            {
                GD.Print("Working");
                EmitSignal("ClickSignal", this);
                HasMark = true;
            }
        }
    }
}
