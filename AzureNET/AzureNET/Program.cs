using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Threading.Tasks;

namespace AzureNET
{
    class Program
    {
        public static async Task RecognizeSpeechFromFileAsync( string fileNameString )
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westeurope").
            var config = SpeechConfig.FromSubscription( AzurePrivateData.SUBSCRIPTION_KEY, AzurePrivateData.REGION_STRING );
            config.OutputFormat = OutputFormat.Detailed;
            var stopRecognition = new TaskCompletionSource<int>();
            int counter = 0;

            // Creates a speech recognizer using file as audio input.
            // Replace with your own audio file name.
            using (var audioInput = AudioConfig.FromWavFileInput( fileNameString ) )
            {
                using (var recognizer = new SpeechRecognizer(config, audioInput))
                {
                    // Subscribes to events.
                    recognizer.Recognizing += (s, e) =>
                    {
                        Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                    };

                    recognizer.Recognized += (s, e) =>
                    {
                        if( e.Result.Reason == ResultReason.RecognizedSpeech )
                        {
                            Console.WriteLine($"RECOGNIZED:\tText={e.Result.Text}");
                            Console.WriteLine($"Duration:\t{e.Result.Duration}");
                            Console.WriteLine($"Duration:\t{e.Result.Duration}");
                            Console.WriteLine("Best Results:");
                            
                            foreach( var result in e.Result.Best() )
                            {
                                counter++;
                                Console.WriteLine(
                                    $"\nRezultat: {counter} " +
                                    $"\nConfidence: {result.Confidence} " +
                                    $"\nLexical: {result.LexicalForm}" +
                                    $"\nITN: {result.NormalizedForm}" +
                                    $"\nMaskedITN: {result.MaskedNormalizedForm}" +
                                    $"\nDisplay: {result.Text}");
                            }
                        }
                        else if( e.Result.Reason == ResultReason.NoMatch )
                        {
                            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        }
                    };

                    recognizer.Canceled += (s, e) =>
                    {
                        Console.WriteLine($"CANCELED: Reason={e.Reason}");

                        if (e.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }

                        stopRecognition.TrySetResult(0);
                    };

                    recognizer.SessionStarted += (s, e) =>
                    {
                        Console.WriteLine("\n    Session started event.");
                    };

                    recognizer.SessionStopped += (s, e) =>
                    {
                        Console.WriteLine("\n    Session stopped event.");
                        Console.WriteLine("\nStop recognition.");
                        stopRecognition.TrySetResult(0);
                    };

                    // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                    // Waits for completion.
                    // Use Task.WaitAny to keep the task rooted.
                    Task.WaitAny(new[] { stopRecognition.Task });

                    // Stops recognition.
                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                }
            }
        }

        public static async Task RecognizeSpeechFromMicrophoneAsync()
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription( AzurePrivateData.SUBSCRIPTION_KEY, AzurePrivateData.REGION_STRING );
            //config.OutputFormat = OutputFormat.Detailed;
            
            // Creates a speech recognizer.
            using (var recognizer = new SpeechRecognizer(config))
            {
                Console.WriteLine("Say something...");

                // Performs recognition. RecognizeOnceAsync() returns when the first utterance has been recognized,
                // so it is suitable only for single shot recognition like command or query. For long-running
                // recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync();

                // Checks result.
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"We recognized: {result.Text}");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                } 
            }
        }

        static void Main()
        {
            //RecognizeFromFile( "whatstheweatherlikeTest.wav" );
            //RecognizeFromFile("speechNormal.wav");
            //RecognizeFromFile("whatstheweatherlikeIncreaseTempo.wav");
            //RecognizeFromFile("whatstheweatherlikeDecreaseTempo.wav");
            //RecognizeFromFile("whatstheweatherlikeIncreaseTempo175.wav");
            //RecognizeFromFile("whatstheweatherlikeIncreaseTempo175_1.wav");
            //RecognizeFromFile("whatstheweatherlikeIncreaseTempo176.wav");
            //RecognizeFromFile("whatstheweatherlikeIncreaseTempo200.wav");
            //RecognizeFromFile("whatstheweatherlikeDecreaseTempo75.wav");
            //RecognizeFromFile("whatstheweatherlikeDecreaseTempo85.wav");
            //RecognizeFromFile("whatstheweatherlikeMusicDecrease10dB.wav");
            //RecognizeFromFile("whatstheweatherlikeMusicDecrease1dB.wav");
              RecognizeFromFile("whatstheweatherlikeMusicDecrease2dB.wav");
            //RecognizeFromFile("whatstheweatherlikeMusicDecrease3dB.wav");
            //RecognizeFromFile("whatstheweatherlikeMusicDecrease5dB.wav");
            //RecognizeFromFile("whatstheweatherlikeMusicNormal.wav");
            //RecognizeFromFile("whatstheweatherlikeMusicIncrease5dB.wav");
            //RecognizeFromFile("whatstheweatherlikeMusicIncrease10dB.wav");
            //RecognizeFromFile("whatstheweatherlikeMusicIncrease12dB.wav");
            //RecognizeFromFile("whatstheweatherlikeMusicIncrease13dB.wav");
            //RecognizeFromFile("whatstheweatherlikeMusicIncrease14dB.wav");

            Console.WriteLine("Please press a key to continue.");
            Console.ReadLine();
        }

        private static void RecognizeFromFile( string fileNameString )
        {
            Console.Out.WriteLine("Speech recognition in a file: {0}", fileNameString );
            RecognizeSpeechFromFileAsync( fileNameString ).Wait();
        }
    }
}
