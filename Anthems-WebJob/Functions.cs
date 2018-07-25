using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using NAudio.Wave;

namespace Anthems_WebJob
{
    public class Functions
    {
        public static void GenerateClip(
        [QueueTrigger("anthemmaker")] String blobInfo,
        [Blob("audiogallery/audio/{queueTrigger}")] CloudBlockBlob inputBlob,
        [Blob("audiogallery/clips/{queueTrigger}")] CloudBlockBlob outputBlob, TextWriter logger)
        {
            logger.WriteLine("GenerateClip() started...");
            logger.WriteLine("Input blob is: " + blobInfo);

            using (Stream input = inputBlob.OpenRead())
            using (Stream output = outputBlob.OpenWrite())
            {
                ConvertAudioToClipMP3(input, output, 20);
                outputBlob.Properties.ContentType = "audio/mpeg3";
            }
            logger.WriteLine("GenerateClip() completed...");
        }

        public static void ConvertAudioToClipMP3(Stream input, Stream output, int length)
        {
            using (var mp3FileReader = new Mp3FileReader(input, wave => new NLayer.NAudioSupport.Mp3FrameDecompressor(wave)))
            {
                Mp3Frame frame;
                frame = mp3FileReader.ReadNextFrame();
                int fLength = (int)(frame.SampleCount / (double)frame.SampleRate * 1000.0);
                int fTarget = (int)(length / (double)fLength * 1000.0);

                int frameCount = 0;
                while ((frame = mp3FileReader.ReadNextFrame()) != null)
                {
                    frameCount++;

                    if (frameCount <= fTarget)
                    {
                        output.Write(frame.RawData, 0, frame.RawData.Length);
                    }
                    else break;
                }
            }
        }
    }
}