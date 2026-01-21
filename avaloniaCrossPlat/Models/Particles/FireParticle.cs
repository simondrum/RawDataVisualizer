using System;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace avaloniaCrossPlat.Models.Particles
{
    
class FireParticle
{
    public Ellipse Shape { get; }

    public double X;
    public double Y;
    public double SpeedY;
    public double DriftX;
    public double Life;
    public double Size;

    public FireParticle(Color color)
    {
        Size = 3 + Random.Shared.NextDouble() * 6;

        Shape = new Ellipse
        {
            Width = Size,
            Height = Size,
            Fill = new SolidColorBrush(color),
            Opacity = 1,
            RenderTransformOrigin = RelativePoint.Center,
            RenderTransform = new TranslateTransform()
        };
    }
}

}