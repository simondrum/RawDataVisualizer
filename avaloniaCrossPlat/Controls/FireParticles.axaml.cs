using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using avaloniaCrossPlat.Models.Particles;

namespace avaloniaCrossPlat.Controls;


public partial class FireParticles : UserControl
{
    readonly List<FireParticle> _particles = new();
    readonly DispatcherTimer _timer;
    readonly Random _rnd = new();
    int MaxParticles = 60;

    public static readonly StyledProperty<decimal?> TemperatureProperty = AvaloniaProperty.Register<FireParticles, decimal?>(nameof(Temperature));
    public decimal? Temperature
    {
        get => GetValue(TemperatureProperty);
        set => SetValue(TemperatureProperty, value);
    }

    public FireParticles()
    {
        InitializeComponent();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        _timer.Tick += Update;
        _timer.Start();
    }

    void Update(object? sender, EventArgs e)
    {
        var width = Bounds.Width;
        var height = Bounds.Height;

        if (Temperature is null)
            return;

        decimal temp = (decimal)Temperature;

        if (temp < 30)
            MaxParticles = 0;
        else if (temp > 50)
            MaxParticles = (int)temp;
        else
            MaxParticles = (int)(temp / 5);



        // Génération
        if (_particles.Count < MaxParticles)
        {
            SpawnParticle(width, height);
        }

        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            var p = _particles[i];

            p.Y -= p.SpeedY;
            p.X += p.DriftX;
            p.Life -= 0.02;

            p.Shape.Opacity = p.Life;

            ((TranslateTransform)p.Shape.RenderTransform!).X = p.X;
            ((TranslateTransform)p.Shape.RenderTransform!).Y = p.Y;

            if (p.Life <= 0 || p.Y < -20)
            {
                PART_Canvas.Children.Remove(p.Shape);
                _particles.RemoveAt(i);
            }
        }
    }

    void SpawnParticle(double width, double height)
    {
        var color = Color.FromRgb(
            255,
            (byte)_rnd.Next(80, 160),
            0);

        var p = new FireParticle(color)
        {
            X = width * 0.2 + _rnd.NextDouble() * width * 0.6,
            Y = height,
            SpeedY = 0.8 + _rnd.NextDouble() * 1.8,
            DriftX = (_rnd.NextDouble() - 0.5) * 0.5,
            Life = 1
        };

        _particles.Add(p);
        PART_Canvas.Children.Add(p.Shape);
    }
}


