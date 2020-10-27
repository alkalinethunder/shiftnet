using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.Cutscenes
{
    public interface IVideoFormat
    {
        uint Length { get; }
        float MsPerFrame { get; }
        
        VideoFrame NextFrame(GraphicsContext gfx);
    }
}

