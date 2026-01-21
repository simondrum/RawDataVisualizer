using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using avaloniaCrossPlat.Models.Particles;

namespace avaloniaCrossPlat.Controls;


public partial class Weather : UserControl
{
    readonly List<RainParticle> _particles = new();
    readonly DispatcherTimer _timer;
    readonly Random _rnd = new();
    int MaxParticles = 300;

 
    public Weather()
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
        var height = Bounds.Top;

        



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
            0,
            (byte)_rnd.Next(0, 160),
            255
            );

        int wind = 0;
        var p = new RainParticle(color)
        {
            X = _rnd.NextDouble() * width,
            Y = height,
            SpeedY = _rnd.Next(-10,-4),
            DriftX = _rnd.NextDouble() * wind,
            Life = 4
        };
test
        _particles.Add(p);
        PART_Canvas.Children.Add(p.Shape);
    }
}


