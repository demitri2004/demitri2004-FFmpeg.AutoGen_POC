using System;
using System.Diagnostics;

public class VideoStabilizer
{
    public static void StabilizeVideo(string inputVideoPath, string outputVideoPath)
    {
        string transformsFilePath = "transforms.trf";

        // Step 1: Detect shaky motion
        Console.WriteLine("Step 1: Detecting motion...");
        string detectArgs = $"-i \"{inputVideoPath}\" -vf vidstabdetect=shakiness=10:accuracy=15 -f null -";
        ExecuteFFmpegCommand(detectArgs);

        // Step 2: Apply stabilization
        Console.WriteLine("Step 2: Applying stabilization...");
        string stabilizeArgs = $"-i \"{inputVideoPath}\" -vf vidstabtransform=input={transformsFilePath} -c:v libx264 -crf 18 -preset fast \"{outputVideoPath}\"";
        ExecuteFFmpegCommand(stabilizeArgs);

        // Clean up the transform file
        if (System.IO.File.Exists(transformsFilePath))
        {
            System.IO.File.Delete(transformsFilePath);
        }
    }

    private static void ExecuteFFmpegCommand(string arguments)
    {
        Process ffmpeg = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        ffmpeg.Start();

        // Optional: Log FFmpeg output
        string output = ffmpeg.StandardOutput.ReadToEnd();
        string error = ffmpeg.StandardError.ReadToEnd();

        ffmpeg.WaitForExit();

        if (ffmpeg.ExitCode != 0)
        {
            Console.WriteLine("FFmpeg error:");
            Console.WriteLine(error);
            throw new Exception($"FFmpeg failed: {error}");
        }
        else
        {
            Console.WriteLine("FFmpeg completed successfully.");
        }
    }
}
