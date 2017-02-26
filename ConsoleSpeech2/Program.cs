using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Speech.Synthesis;
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
            static State taxi;
            static LinkedList<State> States = new LinkedList<State>();
        static void Main(string[] args)
            {
                
                    String[] stateReplies = new String[] { "Pushback approved, facing south", "validate readback", "" };
                    String[][] readbackInfo = { new String[] { "roger", "pushback approved facing south", "facing south pushback approved" } };
                    String callSign = "delta alpha tango one seven seven three";
                    String atcName = "apron";
                    String stateName = "push back";
                    pushback = new State(stateName, stateReplies, readbackInfo, callSign, atcName);

                    stateReplies = new String[] { "Taxi to holding point papa three. Run way two five romeo cross run way two. q n h one zero two four",
                "validate readback", "Give way to indigo air bus three zero zero on inner nine","validate readback",
                "stand by for one two zero decimal seven seven five bangalore tower","validate readback",""};

                    readbackInfo = new String[][]{ new String[]{"taxi to holding point papa three run way two five romeo cross run way two",
                                         "taxi to holding point papa three run way two five romeo cross run way two q  n h one zero two four","roger"},
                                         new String[]{"give way to indigo air bus three zero zero on inner nine","giving way to indigo air bus three zero zero on inner nine",
                                         "giving way to indigo air bus three zero zero","roger"},
                                         new String[]{"stand by for one two zero decimal seven seven five bangalore tower",
                                         "standing by for one two zero decimal seven seven five bangalore tower","standing by for bangalore tower","roger"}};
                    stateName = "taxi";

                    taxi = new State(stateName, stateReplies, readbackInfo, callSign, atcName);

                    
                    States.AddFirst(pushback);
                    States.AddLast(taxi);
                    ss.SetOutputToDefaultAudioDevice();
                    CultureInfo ci = new CultureInfo("en-us");
                    sre = new SpeechRecognitionEngine(ci);
                    sre.SetInputToDefaultAudioDevice();
                    sre.SpeechRecognized += sre_SpeechRecognized;
                    Console.WriteLine("\nReady");
                    ss.Speak("Ready");
                
                while(States.First != null)
                {
                    State state = States.First.Value;
                    sre = Grammars.getGrammars(sre, state);
                    sre.RecognizeAsync(RecognizeMode.Multiple);
                    while (state.IsCompleted() == false) { ; }

                    sre.RecognizeAsyncStop();
                    Console.WriteLine(state.stateName + " Completed!");
                    States.RemoveFirst();


                }

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

            String reply = States.First.Value.GetReply(txt); // current state 
            
            if (txt.Length > 0) {
                Console.WriteLine("\n<Saying>" + reply);
                ss.Speak(reply);
            }
            
        } // sre_SpeechRecognized
  
    }
}
