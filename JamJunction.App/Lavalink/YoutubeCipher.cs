using Lavalink4NET;
using Lavalink4NET.Rest.Entities.Tracks;

namespace JamJunction.App.Lavalink;

public class YoutubeCipher
{
    public async Task WarmUpAsync(IAudioService audioService)
    {
        try
        {
            var result = await audioService.Tracks.LoadTracksAsync(
                "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                new TrackLoadOptions(TrackSearchMode.None));
            Console.WriteLine($"Warm Up Is Successful: {result.IsSuccess}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}