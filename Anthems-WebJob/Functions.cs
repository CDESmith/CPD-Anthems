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
                createSample(input, output, 20);
                outputBlob.Properties.ContentType = "audio/mpeg";
                outputBlob.Metadata["Title"] = blobInfo;
            }
            outputBlob.SetMetadata();
            logger.WriteLine("GenerateClip() completed...");
        }

        private static void createSample(Stream input, Stream output, int duration)
        {
            using (var reader = new Mp3FileReader(input, wave => new NLayer.NAudioSupport.Mp3FrameDecompressor(wave)))
            {
                Mp3Frame frame;
                frame = reader.ReadNextFrame();
                int frameTimeLength = (int)(frame.SampleCount / (double)frame.SampleRate * 1000.0);
                int framesRequired = (int)(duration / (double)frameTimeLength * 1000.0);

                int frameNumber = 0;
                while ((frame = reader.ReadNextFrame()) != null)
                {
                    frameNumber++;

                    if (frameNumber <= framesRequired)
                    {
                        output.Write(frame.RawData, 0, frame.RawData.Length);
                    }
                    else break;
                }
            }
        }
    }
}
