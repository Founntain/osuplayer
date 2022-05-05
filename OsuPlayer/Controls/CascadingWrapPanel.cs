// This source file is adapted from the Windows Presentation Foundation project. 
// (https://github.com/dotnet/wpf/) 
// 
// Licensed to The Avalonia Project under MIT License, courtesy of The .NET Foundation.

using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using DynamicData;
using static System.Math;

namespace OsuPlayer.Controls;

/// <summary>
/// Positions child elements in sequential position from left to right,
/// breaking content to the next line at the edge of the containing box.
/// Subsequent ordering happens sequentially from top to bottom or from right to left,
/// depending on the value of the <see cref="Orientation" /> property.
/// </summary>
public class CascadingWrapPanel : Panel, INavigableContainer
{
    /// <summary>
    /// Defines the <see cref="Orientation" /> property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CascadingWrapPanel, Orientation>(nameof(Orientation), Orientation.Horizontal);

    /// <summary>
    /// Defines the <see cref="ItemWidth" /> property.
    /// </summary>
    public static readonly StyledProperty<double> ItemWidthProperty =
        AvaloniaProperty.Register<CascadingWrapPanel, double>(nameof(ItemWidth), double.NaN);

    /// <summary>
    /// Defines the <see cref="ItemHeight" /> property.
    /// </summary>
    public static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<CascadingWrapPanel, double>(nameof(ItemHeight), double.NaN);

    private int _lineCount = 0;

    /// <summary>
    /// Initializes static members of the <see cref="CascadingWrapPanel" /> class.
    /// </summary>
    static CascadingWrapPanel()
    {
        AffectsMeasure<CascadingWrapPanel>(OrientationProperty, ItemWidthProperty, ItemHeightProperty);
    }

    /// <summary>
    /// Gets or sets the orientation in which child controls will be layed out.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of all items in the WrapPanel.
    /// </summary>
    public double ItemWidth
    {
        get => GetValue(ItemWidthProperty);
        set => SetValue(ItemWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of all items in the WrapPanel.
    /// </summary>
    public double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    /// <summary>
    /// Gets the next control in the specified direction.
    /// </summary>
    /// <param name="direction">The movement direction.</param>
    /// <param name="from">The control from which movement begins.</param>
    /// <param name="wrap">Whether to wrap around when the first or last item is reached.</param>
    /// <returns>The control.</returns>
    IInputElement? INavigableContainer.GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
    {
        var orientation = Orientation;
        var horiz = orientation == Orientation.Horizontal;
        var index = from is not null ? Children.IndexOf((IControl) from) : -1;

        switch (direction)
        {
            case NavigationDirection.First:
                index = 0;
                break;
            case NavigationDirection.Last:
                index = Children.Count - 1;
                break;
            case NavigationDirection.Next:
                ++index;
                break;
            case NavigationDirection.Previous:
                --index;
                break;
            case NavigationDirection.Left:
                index = horiz ? index - 1 : -1;
                break;
            case NavigationDirection.Right:
                index = horiz ? index + 1 : -1;
                break;
            case NavigationDirection.Up:
                index = horiz ? -1 : index - 1;
                break;
            case NavigationDirection.Down:
                index = horiz ? -1 : index + 1;
                break;
        }

        if (index >= 0 && index < Children.Count)
            return Children[index];
        else
            return null;
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size constraint)
    {
        var itemWidth = ItemWidth;
        var itemHeight = ItemHeight;
        var orientation = Orientation;
        var isHorizontal = orientation == Orientation.Horizontal;
        _lineCount = (int) Round((isHorizontal ? constraint.Width : constraint.Height) / (isHorizontal ? itemWidth : itemHeight));
        var itemWidthSet = !double.IsNaN(itemWidth);
        var itemHeightSet = !double.IsNaN(itemHeight);
        double[]? heights = null;
        if (_lineCount > 0)
            heights = new double[_lineCount];

        var childConstraint = new Size(
            itemWidthSet ? itemWidth : constraint.Width,
            itemHeightSet ? itemHeight : constraint.Height);

        for (int i = 0, count = Children.Count; i < count; i++)
        {
            var child = Children[i];
            if (child == null) continue;
            // Flow passes its own constraint to children
            child.Measure(childConstraint);

            // This is the size of the child in UV space
            var sz = new UvSize(orientation,
                itemWidthSet ? itemWidth : child.DesiredSize.Width,
                itemHeightSet ? itemHeight : child.DesiredSize.Height);

            if (heights == null) continue;

            if (i - _lineCount >= 0)
            {
                var line = heights.IndexOf(heights.Min());
                heights[line] += isHorizontal ? sz.V : sz.U;
            }

            if (i - _lineCount < 0) heights[i % _lineCount] = isHorizontal ? sz.V : sz.U;
        }

        var x = isHorizontal ? constraint.Width : constraint.Height;

        // Go from UV space to W/H space
        return new Size(isHorizontal ? x : heights?.Max() ?? 0, isHorizontal ? heights?.Max() ?? 0 : x);
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var itemWidth = ItemWidth;
        var itemHeight = ItemHeight;
        var orientation = Orientation;
        var isHorizontal = orientation == Orientation.Horizontal;
        _lineCount = (int) Round((isHorizontal ? finalSize.Width : finalSize.Height) / (isHorizontal ? itemWidth : itemHeight));
        var itemWidthSet = !double.IsNaN(itemWidth);
        var itemHeightSet = !double.IsNaN(itemHeight);
        double[]? heights = null;
        if (_lineCount > 0)
            heights = new double[_lineCount];

        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            if (child == null) continue;
            var sz = new UvSize(orientation,
                itemWidthSet ? itemWidth : child.DesiredSize.Width,
                itemHeightSet ? itemHeight : child.DesiredSize.Height);

            if (i - _lineCount >= 0)
            {
                if (heights == null) continue;

                var line = heights.IndexOf(heights.Min());

                child.Arrange(new Rect(
                    isHorizontal ? line * sz.U : heights[line],
                    isHorizontal ? heights[line] : line * sz.V,
                    sz.U,
                    sz.V));

                heights[line] += isHorizontal ? sz.V : sz.U;
            }
            else if (i - _lineCount < 0)
            {
                var curLine = i % _lineCount;

                child.Arrange(new Rect(
                    isHorizontal ? curLine * sz.U : 0,
                    isHorizontal ? 0 : curLine * sz.V,
                    sz.U,
                    sz.V));

                if (heights != null)
                    heights[curLine] += isHorizontal ? sz.V : sz.U;
            }
        }

        return finalSize;
    }

    private struct UvSize
    {
        internal UvSize(Orientation orientation, double width, double height)
        {
            U = V = 0d;
            _orientation = orientation;
            Width = width;
            Height = height;
        }

        internal UvSize(Orientation orientation)
        {
            U = V = 0d;
            _orientation = orientation;
        }

        internal readonly double U;
        internal readonly double V;
        private readonly Orientation _orientation;

        private double Width
        {
            get => _orientation == Orientation.Horizontal ? U : V;
            init
            {
                if (_orientation == Orientation.Horizontal) U = value;
                else V = value;
            }
        }

        private double Height
        {
            get => _orientation == Orientation.Horizontal ? V : U;
            init
            {
                if (_orientation == Orientation.Horizontal) V = value;
                else U = value;
            }
        }
    }
}