using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Globalization;
using StatesAndGrammars;
using System.IO;

namespace ConsoleSpeech2
{
    
    class Program
    {   
            // ss,sre and state object made available everywhere
            static SpeechSynthesizer ss = new SpeechSynthesizer();
            static SpeechRecognitionEngine sre;
            static State pushback;
            static State taxi;
            static State takeoff;
            static LinkedList<State> States = new LinkedList<State>();
        static void Main(string[] args)
            {
                    string dir = Directory.GetCurrentDirectory();
                    string serializationFile = Path.Combine(dir, "states.bin");
                    if (!File.Exists(serializationFile))
                    {
                        MakeStates(serializationFile);
                       
                    }
                    else
                    {
                        LoadStates(serializationFile);
                    }
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
                    sre = Grammars.GetGrammars(sre, state);
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

        static void MakeStates(string serializationFile)
        {
            //pushback
            String[] stateReplies = new String[] { "Pushback approved, facing south", "validate readback", "" };
            String[][] readbackInfo = { new String[] { "roger", "push back approved facing south", "facing south push back approved" } };
            String callSign = "delta alpha tango one seven seven three";
            String atcName = "apron";
            String stateName = "push back";
            pushback = new State(stateName, stateReplies, readbackInfo, callSign, atcName);
            //taxi
            stateReplies = new String[] { "Taxi to holding point papa three, runway two five romeo cross runway two, Q N H one zero two four",
                        "validate readback", "Give way to indigo air bus three zero zero on inner nine","validate readback",
                        "standby for one two zero decimal seven seven five bangalore tower","validate readback","Bangalore Tower report when ready for departure"};

            readbackInfo = new String[][]{ new String[]{"taxi to holding point papa three run way two five romeo cross run way two",
                                         "taxi to holding point papa three run way two five romeo cross run way two, Q N H one zero two four","roger"},
                                         new String[]{"give way to indigo air bus three zero zero on inner nine","giving way to indigo air bus three zero zero on inner nine",
                                         "giving way to indigo air bus three zero zero","roger"},
                                         new String[]{"stand by for one two zero decimal seven seven five bangalore tower",
                                         "standing by for one two zero decimal seven seven five bangalore tower","standing by for bangalore tower","roger"}};
            stateName = "taxi";

            taxi = new State(stateName, stateReplies, readbackInfo, callSign, atcName);
            //takeoff
            stateReplies = new String[] {"Line up and wait runway two five romeo.","Wind two nine zero degrees, eight knots, runway two five romeo cleared for takeoff, when airborne contact bangalore departure one two six decimal six two five."};

            readbackInfo = new String[][] {new String[]{"line up and wait runway two five romeo"},new String[]{"runway two five romeo cleared for takeoff when airborne contact bangalore departure one two six decimal six two five",
            "runway two five romeo cleared for takeoff when airborne contact bangalore departure"}};
            
            //making the list
            States.AddFirst(pushback);
            States.AddLast(taxi);
            States.AddLast(takeoff);
            //serialize
            using (Stream stream = File.Open(serializationFile, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, States);
            }

            
        } //MakeStates

        static void LoadStates(string serializationFile)
        {
            using (Stream stream = File.Open(serializationFile, FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                List<State> salesman = (List<State>)bformatter.Deserialize(stream);
            }

        }// LoadStates
    }
}
