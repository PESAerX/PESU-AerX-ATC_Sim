using System;
using System.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Globalization;
using StatesAndGrammars;
namespace ConsoleSpeech2
{
    
    class Program
    {   
            // ss,sre and state object made available everywhere
            static SpeechSynthesizer ss = new SpeechSynthesizer();
            static SpeechRecognitionEngine sre;
            static State pushback;
        
        static void Main(string[] args)
        {
            String[] stateReplies = new String[]{"Pushback approved, facing south","validate readback"};
            String[][] readbackInfo = { new String[] { "roger", "pushback approved facing south", "facing south pushback approved" } };
            String callSign = "delta alpha tango one seven seven three";
            String atcName = "apron";
            String stateName = "pushback";
            pushback = new State(stateName,stateReplies,readbackInfo, callSign, atcName);   
        

            //ss.Rate = -2; //arbitrary
            ss.SetOutputToDefaultAudioDevice();
            CultureInfo ci = new CultureInfo("en-us");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += sre_SpeechRecognized;            
            sre = Grammars.getGrammars(sre, pushback);
            Console.WriteLine("\nReady");
            ss.Speak("Ready");
            sre.RecognizeAsync(RecognizeMode.Multiple);
            while (pushback.IsCompleted() == false) { ; }
            Console.WriteLine("Pushback Completed!");
            Console.Read();
            
            
        }
        

        static void sre_SpeechRecognized(object sender,
      SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;
            Console.WriteLine("\nRecognized: " + txt);
            if (confidence < 0.40) return; // arbitrary constant
            //begin handling
            String reply = pushback.GetReply(txt);
            //Console.WriteLine( txt.Length);
            if (txt.Length > 0) {
                Console.WriteLine("\n<Saying>" + reply);
                ss.Speak(reply);
            }
            
        } // sre_SpeechRecognized
  
    }
}
