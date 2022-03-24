using System.Speech.Synthesis;
using CommandLine;
using CommandLine.Text;
using Mar.Console;

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

            HelpText.AutoBuild(parserResult, null, null).ToString().PrintGreen();
        });
    }

    private static void PrintVoicesList()
    {
        foreach (var installedVoice in SpeechSynthesizer.GetInstalledVoices())
            PrintShortVoiceInfo(installedVoice.VoiceInfo);
    }

    private static void PrintShortVoiceInfo(VoiceInfo voiceInfo)
    {
        $"Voice name: '{voiceInfo.Name}'".PrintGreen();
    }

    private static void PrintDetailedVoiceInfo(VoiceInfo voiceInfo = null)
    {
        voiceInfo ??= SpeechSynthesizer.Voice;

        $"Voice name: '{voiceInfo.Name}'".PrintGreen();
        $"Description: '{voiceInfo.Description}'".PrintGreen();
        $"Gender: '{voiceInfo.Gender}'".PrintGreen();
        $"Age: '{voiceInfo.Age}'".PrintGreen();
        $"Culture: '{voiceInfo.Culture}'".PrintGreen();
        if (voiceInfo.AdditionalInfo.Any())
        {
            "Additional information:".PrintGreen();
            foreach (var additionalInfo in voiceInfo.AdditionalInfo)
                $"\t{additionalInfo.Key}: '{additionalInfo.Value}'".PrintGreen();
        }
    }

    private static bool SelectVoiceByName(string voiceName)
    {
        try
        {
            SpeechSynthesizer.SelectVoice(voiceName);
            $"Selected voice '{voiceName}'.".PrintMagenta();
            return true;
        }
        catch (Exception)
        {
            $"Voice with name '{voiceName}' not found.".PrintErr();
            $"Selected voice '{SpeechSynthesizer.Voice.Name}'.".PrintErr();
            return false;
        }
    }
}