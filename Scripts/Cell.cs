using Godot;
using System;

public class Cell : Sprite
{
    private Sprite Mark;
    public bool HasMark;
    [Signal] public delegate void ClickSignal(Cell _cell);
    public int row;
    public int col;
    public string marker;
    public static bool IsPlayerTurn;

    public override void _Ready()
    {
        IsPlayerTurn = true;
        marker = "";
        HasMark = false;
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
        marker = _itsX ? "x" : "o";
        HasMark = true;
    }

    public void _CellInput(InputEvent @event)
    {
        if (HasMark)
        {
            return;
        }
        else if (@event is InputEventMouseButton btn && 
            btn.ButtonIndex == (int)ButtonList.Left && 
            btn.Pressed)
        {
            if (GetRect().HasPoint(ToLocal(btn.Position)) && IsPlayerTurn)
            {
                EmitSignal("ClickSignal", this);
            }
        }
    }
}
