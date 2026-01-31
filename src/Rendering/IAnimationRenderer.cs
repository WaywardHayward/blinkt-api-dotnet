using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public interface IAnimationRenderer
{
    void Render(BlinktController blinkt, Color color, double elapsedSeconds);
}
