using System;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace avaloniaCrossPlat.Models.Particles
{
    
class RainParticle
{
    public Ellipse Shape { get; }

    public double X;
    public double Y;
    public double SpeedY;
    public double DriftX;
    public double Life;
    public double Size;

    public RainParticle(Color color)
{
    var baseSize = 3 + Random.Shared.NextDouble() * 6;

    Shape = new Ellipse
    {
        Width = baseSize * 0.6,   // plus fin
        Height = baseSize * 2.2,  // plus long → goutte
        Fill = new SolidColorBrush(color),
        Opacity = 0.85,
        RenderTransformOrigin = RelativePoint.Center,
        RenderTransform = new TranslateTransform()
    };
}
}

}