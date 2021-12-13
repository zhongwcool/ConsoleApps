using System.Speech.Synthesis;
using CommandLine;
using CommandLine.Text;

namespace Speak.App;

internal class Program
{
    private static readonly SpeechSynthesizer SpeechSynthesizer = new();

    private static void Main(string[] args)
    {
        var parserResult = Parser.Default.ParseArguments<StartOptions>(args);
        parserResult.WithParsed(startOptions =>
        {
            if (startOptions.VoicesList)
            {
                PrintVoicesList();
                return;
            }

            if (!string.IsNullOrEmpty(startOptions.VoiceInfo))
            {
                if (SelectVoiceByName(startOptions.VoiceInfo)) PrintDetailedVoiceInfo();
                return;
            }

            if (!string.IsNullOrEmpty(startOptions.Text))
            {
                if (!string.IsNullOrEmpty(startOptions.Voice)) SelectVoiceByName(startOptions.Voice);

                SpeechSynthesizer.Volume = startOptions.Volume;
                SpeechSynthesizer.Rate = startOptions.Rate;

                SpeechSynthesizer.SetOutputToDefaultAudioDevice();
                SpeechSynthesizer.Speak(startOptions.Text);

                return;
            }

            Console.WriteLine("为什么可以打印出来");
            Console.WriteLine(HelpText.AutoBuild(parserResult, null, null));
        });
    }

    private static void PrintVoicesList()
    {
        foreach (var installedVoice in SpeechSynthesizer.GetInstalledVoices())
            PrintShortVoiceInfo(installedVoice.VoiceInfo);
    }

    private static void PrintShortVoiceInfo(VoiceInfo voiceInfo)
    {
        Console.WriteLine($"Voice name: '{voiceInfo.Name}'");
    }

    private static void PrintDetailedVoiceInfo(VoiceInfo voiceInfo = null)
    {
        voiceInfo ??= SpeechSynthesizer.Voice;

        Console.WriteLine($"Voice name: '{voiceInfo.Name}'");
        Console.WriteLine($"Description: '{voiceInfo.Description}'");
        Console.WriteLine($"Gender: '{voiceInfo.Gender}'");
        Console.WriteLine($"Age: '{voiceInfo.Age}'");
        Console.WriteLine($"Culture: '{voiceInfo.Culture}'");
        if (voiceInfo.AdditionalInfo.Any())
        {
            Console.WriteLine("Additional information:");
            foreach (var additionalInfo in voiceInfo.AdditionalInfo)
                Console.WriteLine($"\t{additionalInfo.Key}: '{additionalInfo.Value}'");
        }
    }

    private static bool SelectVoiceByName(string voiceName)
    {
        try
        {
            SpeechSynthesizer.SelectVoice(voiceName);
            Console.WriteLine($"Selected voice '{voiceName}'.");

            return true;
        }
        catch (Exception)
        {
            Console.WriteLine($"Voice with name '{voiceName}' not found.");
            Console.WriteLine($"Selected voice '{SpeechSynthesizer.Voice.Name}'.");

            return false;
        }
    }
}