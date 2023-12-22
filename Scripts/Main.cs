using Godot;
using System;

public class Main : Node2D
{
    private Node2D Cells;
    private Godot.Collections.Array CellsArray;
    private int TurnCount;
    private bool IsX;

    public override void _Ready()
    {
        base._Ready();
        TurnCount = 0;
        Cells = GetNode<Node2D>("Cells");
        CellsArray = Cells.GetChildren();
        SetGrid();
    }

    private void SetGrid()
    {
        int count = 0;
        float size = 120f;
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (CellsArray[count] is Cell _cell)
                {
                    _cell.Position = new Vector2(col * size,row * size);
                    _cell.SetRow(row);
                    _cell.SetCol(col);
                    _cell.Connect("ClickSignal", this, "OnCellClicked");
                }
                count++;
            }
        }
    }

    private void OnCellClicked(Cell _cell)
    {
        IsX = TurnCount % 2 == 0;
        _cell.MarkCell(IsX);
        GD.Print("Row: " + _cell.row);
        GD.Print("Col: " + _cell.col);
        TurnCount++;
    }
}
