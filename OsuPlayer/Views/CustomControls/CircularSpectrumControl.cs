using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using OsuPlayer.Modules.Audio.Interfaces;

namespace OsuPlayer.Views.CustomControls;

public sealed class CircularSpectrumControl : Control
{
    private const int BarCount = 128;
    private readonly float[] _levels = new float[BarCount];
    private readonly DispatcherTimer _timer;

    public static readonly StyledProperty<IAudioEngine?> AudioEngineProperty =
        AvaloniaProperty.Register<CircularSpectrumControl, IAudioEngine?>(nameof(AudioEngine));

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CircularSpectrumControl, bool>(nameof(IsActive));

    public static readonly StyledProperty<double> ArtworkDiameterProperty =
        AvaloniaProperty.Register<CircularSpectrumControl, double>(nameof(ArtworkDiameter), 320);

    public static readonly StyledProperty<IBrush?> AccentBrushProperty =
        AvaloniaProperty.Register<CircularSpectrumControl, IBrush?>(nameof(AccentBrush));

    public IAudioEngine? AudioEngine
    {
        get => GetValue(AudioEngineProperty);
        set => SetValue(AudioEngineProperty, value);
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public double ArtworkDiameter
    {
        get => GetValue(ArtworkDiameterProperty);
        set => SetValue(ArtworkDiameterProperty, value);
    }

    public IBrush? AccentBrush
    {
        get => GetValue(AccentBrushProperty);
        set => SetValue(AccentBrushProperty, value);
    }

    public CircularSpectrumControl()
    {
        IsHitTestVisible = false;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += (_, _) => UpdateSpectrum();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _timer.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _timer.Stop();
        base.OnDetachedFromVisualTree(e);
    }

    private void UpdateSpectrum()
    {
        var data = IsActive ? AudioEngine?.GetVisualizationData() : null;

        for (var i = 0; i < BarCount; i++)
        {
            var target = 0f;
            if (data is { Length: > 16 })
            {
                var startRatio = Math.Pow(i / (double)BarCount, 2.15);
                var endRatio = Math.Pow((i + 1d) / BarCount, 2.15);
                var start = Math.Clamp(2 + (int)(startRatio * 900), 0, data.Length - 1);
                var end = Math.Clamp(3 + (int)(endRatio * 900), start + 1, data.Length);

                var peak = 0f;
                for (var bin = start; bin < end; bin++)
                    peak = Math.Max(peak, data[bin]);

                target = (float)Math.Clamp(Math.Sqrt(peak * 22), 0, 1);
            }

            var smoothing = target > _levels[i] ? 0.48f : 0.86f;
            _levels[i] = (_levels[i] * smoothing) + (target * (1 - smoothing));
        }

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
        var outerLimit = Math.Min(Bounds.Width, Bounds.Height) / 2;
        var innerRadius = Math.Min((ArtworkDiameter / 2) + 7, outerLimit - 12);
        var availableLength = Math.Max(5, outerLimit - innerRadius - 5);

        var ringBrush = AccentBrush ?? Brushes.White;
        context.DrawEllipse(null, new Pen(ringBrush, 1), center, innerRadius - 2, innerRadius - 2);

        var barPen = new Pen(new SolidColorBrush(Color.FromArgb(222, 255, 255, 255)), 1.25,
            lineCap: PenLineCap.Round);

        for (var i = 0; i < BarCount; i++)
        {
            var angle = (-Math.PI / 2) + (i * Math.PI * 2 / BarCount);
            var level = _levels[i];
            var length = 3 + (level * availableLength);
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            var start = new Point(center.X + (cos * innerRadius), center.Y + (sin * innerRadius));
            var end = new Point(center.X + (cos * (innerRadius + length)),
                center.Y + (sin * (innerRadius + length)));
            context.DrawLine(barPen, start, end);
        }
    }
}

public sealed class AmbientParticleControl : Control
{
    private const int ParticleCount = 76;
    private readonly Particle[] _particles = new Particle[ParticleCount];
    private readonly DispatcherTimer _timer;
    private readonly Random _random = new(8027);

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<AmbientParticleControl, bool>(nameof(IsActive));

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public AmbientParticleControl()
    {
        IsHitTestVisible = false;
        for (var i = 0; i < _particles.Length; i++)
            _particles[i] = CreateParticle(initial: true);

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40) };
        _timer.Tick += (_, _) => AdvanceParticles();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _timer.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _timer.Stop();
        base.OnDetachedFromVisualTree(e);
    }

    private Particle CreateParticle(bool initial)
    {
        return new Particle(
            _random.NextDouble(),
            initial ? _random.NextDouble() : 1.02,
            0.00025 + (_random.NextDouble() * 0.00065),
            0.55 + (_random.NextDouble() * 1.25),
            0.10 + (_random.NextDouble() * 0.42));
    }

    private void AdvanceParticles()
    {
        var speedMultiplier = IsActive ? 1d : 0.32d;
        for (var i = 0; i < _particles.Length; i++)
        {
            var particle = _particles[i];
            particle.Y -= particle.Speed * speedMultiplier;
            particle.X += Math.Sin((particle.Y * 19) + i) * 0.00008;

            if (particle.Y < -0.02)
                particle = CreateParticle(initial: false);

            _particles[i] = particle;
        }

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        foreach (var particle in _particles)
        {
            var point = new Point(particle.X * Bounds.Width, particle.Y * Bounds.Height);
            var alpha = (byte)Math.Clamp(particle.Opacity * 255, 0, 255);
            var brush = new SolidColorBrush(Color.FromArgb(alpha, 255, 255, 255));
            context.DrawEllipse(brush, null, point, particle.Size, particle.Size);
        }
    }

    private struct Particle(double x, double y, double speed, double size, double opacity)
    {
        public double X = x;
        public double Y = y;
        public readonly double Speed = speed;
        public readonly double Size = size;
        public readonly double Opacity = opacity;
    }
}
