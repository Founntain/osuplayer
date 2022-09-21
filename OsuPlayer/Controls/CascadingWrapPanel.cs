// This source file is adapted from the Windows Presentation Foundation project. 
// (https://github.com/dotnet/wpf/) 
// 
// Licensed to The Avalonia Project under MIT License, courtesy of The .NET Foundation.

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
        AvaloniaProperty.Register<CascadingWrapPanel, Orientation>(nameof(Orientation));

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

    private int _lineCount;

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
    /// Initializes static members of the <see cref="CascadingWrapPanel" /> class.
    /// </summary>
    static CascadingWrapPanel()
    {
        AffectsMeasure<CascadingWrapPanel>(OrientationProperty, ItemWidthProperty, ItemHeightProperty);
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

        return null;
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size constraint)
    {
        var parameters = new MeasureOverrideParameters
        {
            ItemWidth = ItemWidth,
            ItemHeight = ItemHeight,
            Orientation = Orientation
        };

        _lineCount = (int) Round((parameters.IsHorizontal ? constraint.Width : constraint.Height) / (parameters.IsHorizontal ? parameters.ItemWidth : parameters.ItemHeight));

        double[]? heights = null;

        if (_lineCount > 0)
            heights = new double[_lineCount];

        var childConstraint = new Size(
            parameters.ItemWidthSet ? parameters.ItemWidth : constraint.Width,
            parameters.ItemHeightSet ? parameters.ItemHeight : constraint.Height);

        for (int i = 0, count = Children.Count; i < count; i++)
        {
            var child = Children[i];

            if (child == null) continue;

            // Flow passes its own constraint to children
            child.Measure(childConstraint);

            // This is the size of the child in UV space
            var sz = new UvSize(parameters.Orientation,
                parameters.ItemWidthSet ? parameters.ItemWidth : child.DesiredSize.Width,
                parameters.ItemHeightSet ? parameters.ItemHeight : child.DesiredSize.Height);

            if (heights == null) continue;

            if (i - _lineCount < 0)
            {
                heights[i % _lineCount] = parameters.IsHorizontal ? sz.V : sz.U;
                continue;
            }

            var line = heights.IndexOf(heights.Min());
            heights[line] += parameters.IsHorizontal ? sz.V : sz.U;
        }

        var x = parameters.IsHorizontal ? constraint.Width : constraint.Height;

        // Go from UV space to W/H space
        return new Size(parameters.IsHorizontal ? x : heights?.Max() ?? 0, parameters.IsHorizontal ? heights?.Max() ?? 0 : x);
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var parameters = new ArrangeOverrideParameters
        {
            ItemWidth = ItemWidth,
            ItemHeight = ItemHeight,
            Orientation = Orientation
        };

        double[]? heights = null;

        _lineCount = (int) Round((parameters.IsHorizontal ? finalSize.Width : finalSize.Height) / (parameters.IsHorizontal ? parameters.ItemWidth : parameters.ItemHeight));

        if (_lineCount > 0)
            heights = new double[_lineCount];

        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[i];

            if (child == null) continue;

            var sz = new UvSize(parameters.Orientation,
                parameters.ItemWidthSet ? parameters.ItemWidth : child.DesiredSize.Width,
                parameters.ItemHeightSet ? parameters.ItemHeight : child.DesiredSize.Height);

            if (heights == null) continue;

            int line;

            if (i - _lineCount < 0)
            {
                line = i % _lineCount;

                child.Arrange(new Rect(
                    parameters.IsHorizontal ? line * sz.U : 0,
                    parameters.IsHorizontal ? 0 : line * sz.V,
                    sz.U,
                    sz.V));

                heights[line] += parameters.IsHorizontal ? sz.V : sz.U;

                continue;
            }

            line = heights.IndexOf(heights.Min());

            child.Arrange(new Rect(
                parameters.IsHorizontal ? line * sz.U : heights[line],
                parameters.IsHorizontal ? heights[line] : line * sz.V,
                sz.U,
                sz.V));

            heights[line] += parameters.IsHorizontal ? sz.V : sz.U;
        }

        return finalSize;
    }

    private record MeasureOverrideParameters
    {
        internal double ItemHeight;
        internal double ItemWidth;
        internal Orientation Orientation;
        internal bool IsHorizontal => Orientation == Orientation.Horizontal;
        internal bool ItemWidthSet => !double.IsNaN(ItemWidth);
        internal bool ItemHeightSet => !double.IsNaN(ItemHeight);
    }

    private record ArrangeOverrideParameters
    {
        internal double ItemHeight;
        internal double ItemWidth;
        internal Orientation Orientation;
        internal bool IsHorizontal => Orientation == Orientation.Horizontal;
        internal bool ItemWidthSet => !double.IsNaN(ItemWidth);
        internal bool ItemHeightSet => !double.IsNaN(ItemHeight);
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