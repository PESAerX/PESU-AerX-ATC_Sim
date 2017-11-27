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
            static LinkedList<State> States = new LinkedList<State>();
        static void Main(string[] args)
            {
                    string dir = Directory.GetCurrentDirectory();
                    string serializationFile = Path.Combine(dir, "states.bin");
                    //if (!File.Exists(serializationFile))
                   // {
                        MakeStates(serializationFile);
                       
                    //}
                    //else
                    //{
                    //    LoadStates(serializationFile);
                    //}
                    ss.SetOutputToDefaultAudioDevice();
                    CultureInfo ci = new CultureInfo("en-us");
                    sre = new SpeechRecognitionEngine(ci);
                    sre.SetInputToDefaultAudioDevice();
                    sre.SpeechRecognized += sre_SpeechRecognized;
                    ss.Speak("Ready"); 
                    Console.WriteLine("Saying: <Ready>");
                 
                
                while(States.First != null)
                {
                    State state = States.First.Value;
                    sre = Grammars.GetGrammars(sre, state);
                    sre.RecognizeAsync(RecognizeMode.Multiple);
                    Console.WriteLine("Current State: " + States.First.Value.stateName);
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
            //setting call sign here, will remain unchanged throughout
            String callSign = "delta alpha tango one seven two";
            
            //delivery
            String[] stateReplies = new String[] {"validate readback","start up approved. Cleared to lima echo delta via civ nine charlie departure. Climb to fox trot lima six zero. Squawk seven one three two. Monitor atis information x-ray",
                "validate readback","when ready contact bangalore ground, one two one decimal eight seven five","validate readback",""};
            String[][] readbackInfo = { new String[] { "stand two zero seven request start up information whiskey", "stand two zero seven requesting start up information whiskey", "stand two zero seven ready for start up information whiskey",
            "stand two zero seven ready to start up information whiskey"},new String[]{"start up approved cleared to lima echo delta via civ nine charlie departure climb to fox trot lima six zero squawk seven one three two information x ray","roger"},
             new String[]{"when ready contact bangalore ground","when ready contact bangalore ground one two one decimal eight seven five","contact bangalore ground when ready one two one decimal eight seven five","contact bangalore ground when ready","roger"}};
            
            String atcName = "delivery";
            String stateName = "delivery";
            State delivery = new State(stateName, stateReplies, readbackInfo, callSign, atcName);
            
            //pushback
            stateReplies = new String[] {"validate readback" ,"Pushback approved, facing south", "validate readback", "" };
            readbackInfo = new String[][]{new String[]{"requesting pushback","ready for pushback","ready to pushback"}, new String[] { "roger", "push back approved facing south", "facing south push back approved" } };
            
            atcName = "ground";
            stateName = "push back";
            State pushback = new State(stateName, stateReplies, readbackInfo, callSign, atcName);
            //taxi
            stateReplies = new String[] {"validate readback", "Taxi to holding point papa three, runway two five romeo cross runway two, Q N H one zero two four",
                        "validate readback", "Give way to indigo air bus three zero zero on inner nine","validate readback",
                        "standby for one two zero decimal seven seven five bangalore tower","validate readback"," This is Bangalore Tower, report when ready for departure."};

            readbackInfo = new String[][]{new String[]{"requesting taxi","ready for taxi","ready to taxi"}, new String[]{"taxi to holding point papa three run way two five romeo cross run way two",
                                         "taxi to holding point papa three run way two five romeo cross run way two, Q N H one zero two four","roger"},
                                         new String[]{"give way to indigo air bus three zero zero on inner nine","giving way to indigo air bus three zero zero on inner nine",
                                         "giving way to indigo air bus three zero zero","roger"},
                                         new String[]{"stand by for one two zero decimal seven seven five bangalore tower",
                                         "standing by for one two zero decimal seven seven five bangalore tower","standing by for bangalore tower","roger"}};
            stateName = "taxi";

            State taxi = new State(stateName, stateReplies, readbackInfo, callSign, atcName);
            //takeoff
            stateReplies = new String[] {"validate readback","Line up and wait runway two five romeo.","validate readback","wind two nine zero degrees, eight knots, runway two five romeo cleared for takeoff, when airborne contact bangalore departure one two six decimal six two five.",
                "validate readback",""};

            readbackInfo = new String[][] {new String[]{"ready","ready for departure","ready to take off","ready for takeoff"},new String[]{"line up and wait runway two five romeo","roger"},new String[]{"run way two five romeo cleared for takeoff when airborne contact bangalore departure one two six decimal six two five",
            "runway two five romeo cleared for takeoff when airborne contact bangalore departure","roger","runway two five romeo cleared for takeoff"}};
            stateName = "take off";
            State takeoff = new State(stateName, stateReplies, readbackInfo, callSign, atcName);
            
            //landing
            stateReplies = new String[] {"validate readback","wind two four zero degrees eight knots. Cleared to land run way two five lima","validate readback","take convenient left and contact ground one one eight decimal zero five zero","validate readback","" };
            readbackInfo = new String[][] {new String[]{"established i l s run way two five lima"}, new String[]{"roger","cleared to land runway two five lima"},
            new String[]{"take convenient left and contact ground","roger","take convenient left and contact ground one one eight decimal zero five zero"}};
            stateName = "landing";
            atcName = "tower";

            State landing = new State(stateName, stateReplies, readbackInfo, callSign, atcName);
            
            //ground
            stateReplies = new String[] { "validate readback" ,"taxi to terminal bravo stand two zero five via inner nine","validate readback",""};
            readbackInfo = new String[][] { new String[] { "run way two five lima vacated" }, new String[]{"roger", "taxi to terminal bravo stand two zero five via inner nine",
                                            "taxi to terminal bravo stand two zero five"},new String[]{}};
            atcName = "ground";
            stateName = "ground";
            State ground = new State(stateName, stateReplies, readbackInfo, callSign, atcName);
            //making the list
            //States.AddFirst(delivery);
            //States.AddLast(pushback);
            //States.AddLast(taxi);
            //States.AddLast(takeoff);
            //States.AddLast(landing);
            States.AddLast(ground);
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

                States = (LinkedList<State>)bformatter.Deserialize(stream);
                //Console.WriteLine(sizeof(States.First.Value));
            }

        }// LoadStates
    }
}
