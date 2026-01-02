using Lavalink4NET;
using Lavalink4NET.Rest.Entities.Tracks;
using Quartz;

namespace JamJunction.App.Quartz_Scheduler;

public class YoutubeCipherJob : IJob
{
    private readonly IAudioService _audioService;

    public YoutubeCipherJob(IAudioService audioService)
    {
        _audioService = audioService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var result = await _audioService.Tracks.LoadTracksAsync(
                "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                new TrackLoadOptions(TrackSearchMode.None));
            Console.WriteLine($"Warm Up Is Successful: {result.IsSuccess}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
    }
}