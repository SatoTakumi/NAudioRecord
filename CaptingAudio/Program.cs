using NAudio.Wave;
using NAudio.Lame;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

namespace CaptingAudio
{
    class Program
    {

        // private WaveFileWriter writer;

        private static LameMP3FileWriter writer;
        public static String outputFilename;
        private static IWaveIn waveIn;
        static void Main(string[] args)
        {
            outputFilename = String.Format("CaptingAudio_{0:yyy-MM-dd HH-mm-ss}.mp3", DateTime.Now);
            string outputFolder;
            Console.WriteLine("This program is record in computer. Same loopback record. ");
            Console.WriteLine("Save : " + Path.GetTempPath() + "\\CaptingAudio");
            Console.WriteLine("0: StartRecording, 1: StopRecord");
            outputFolder = Path.Combine(Path.GetTempPath(), "CaptingAudio");
            Directory.CreateDirectory(outputFolder);
            if (int.Parse(Console.ReadLine()) == 0)
            {
                if (waveIn == null)
                {
                    waveIn = new WasapiLoopbackCapture();
                    writer = new LameMP3FileWriter(Path.Combine(outputFolder, outputFilename), waveIn.WaveFormat, 128);

                    waveIn.DataAvailable += OnDataAvailable;
                    waveIn.RecordingStopped += OnRecordingStopped;
                    waveIn.StartRecording();
                }
            }
            if (int.Parse(Console.ReadLine()) == 1)
            {
                StopRecording();
            }
        }
        /*
            Record Method
        */
        static void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        static void StopRecording()
        {
            Console.WriteLine("StopRecording");
            waveIn.StopRecording();
        }

        static void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            Cleanup();
            if (e.Exception != null)
            {
                Console.WriteLine(String.Format("A problem was encountered during recording {0}",
                                                e.Exception.Message));
            }
        }

        private static void Cleanup()
        {
            if (waveIn != null) // working around problem with double raising of RecordingStopped
            {
                waveIn.Dispose();
                waveIn = null;
            }
            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
        }
    }
}
