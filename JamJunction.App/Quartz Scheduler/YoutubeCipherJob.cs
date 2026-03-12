using Lavalink4NET;
using Lavalink4NET.Rest.Entities.Tracks;
using Quartz;

namespace JamJunction.App.Quartz_Scheduler;

/// <summary>
/// Represents a scheduled job used to warm up the YouTube cipher
/// used by the Lavalink audio service.
/// </summary>
/// <remarks>
/// This job was originally intended to preload or refresh the YouTube
/// cipher by attempting to load a known YouTube track. It is currently
/// retained for archival purposes and is no longer actively used.
/// </remarks>
public class YoutubeCipherJob : IJob
{
    private readonly IAudioService _audioService;
    
    public YoutubeCipherJob(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Executes the scheduled job responsible for attempting to load a
    /// YouTube track in order to initialize the cipher used for playback.
    /// </summary>
    /// <param name="context">
    /// The <see cref="IJobExecutionContext"/> containing runtime information
    /// about the scheduled job execution.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous job execution.
    /// </returns>
    /// <remarks>
    /// This method attempts to load a known YouTube video to verify that
    /// the audio service is functioning correctly. The job is currently
    /// preserved for archival purposes and is not actively used.
    /// </remarks>
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var result = await _audioService.Tracks.LoadTracksAsync(
                "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                new TrackLoadOptions(TrackSearchMode.None));
            Console.WriteLine($"[{DateTime.UtcNow}] Warm Up Is Successful: {result.IsSuccess}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
    }
}