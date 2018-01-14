using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Globalization;
using StatesAndGrammars;
using System.IO;

using System.Windows.Forms;

namespace ConsoleSpeech2
{            
    
    class Program
    {   
            // ss,sre and state object made available everywhere
            static SpeechSynthesizer ss = new SpeechSynthesizer();
            static SpeechRecognitionEngine sre;
            static LinkedList<State> States = new LinkedList<State>();
            static LinkedList<State> Temp = States;
            static LinkedList<State> EmergencyStatesLL = new LinkedList<State>();
            
        
        static void Main(string[] args)
            {
            Application.EnableVisualStyles();
            Application.Run(new Form1());//To run Form1 over the console.
            string dir = Directory.GetCurrentDirectory();
                    string serializationFile = Path.Combine(dir, "states.bin");
                    if (!File.Exists(serializationFile))
                    {
                        MakeStates(serializationFile);
                Console.WriteLine("HIHIHIHI1");
                    }
                    else
                    {
                        LoadStates(serializationFile);
                Console.WriteLine("HIHIHIHI2");
            }
                    ss.SetOutputToDefaultAudioDevice();
                    CultureInfo ci = new CultureInfo("en-US");
                    sre = new SpeechRecognitionEngine(ci);
                        
                    sre.SetInputToDefaultAudioDevice();
                    sre.SpeechRecognized += sre_SpeechRecognized;
                    sre.SpeechHypothesized += sre_SpeechHypothesized;
                    sre.SpeechRecognitionRejected += sre_SpeechRecognitionRejected;
                  //  sre.EndSilenceTimeoutAmbiguous = TimeSpan.FromSeconds(2);
                    ss.Speak("Ready"); 
                    Console.WriteLine("Saying: <Ready>");
           
                
                while(Temp.First != null) 
                {
                    State state = Temp.First.Value;
                    sre = Grammars.GetGrammars(sre, state);
                    //sre.EmulateRecognizeAsync();
                    sre.RecognizeAsync(RecognizeMode.Multiple);
                   
                    Console.WriteLine("Current State: " + Temp.First.Value.stateName);
                    while (state.IsCompleted() == false)
                    {
                        if(State.IsEmergency())
                        {
                            Temp = EmergencyStatesLL;
                           
                            break;
                        }
                    }
                    sre.RecognizeAsyncStop();
                    Console.WriteLine(state.stateName + " Completed!");
                    if (State.IsEmergency())
                    {
                        State.setEmergency(false);
                        //Console.Write(EmergencyStatesLL.First.Value.stateName);
                        //Console.Write(Temp.First.Value.stateName);
                    }
                    else
                    {
                        Temp.RemoveFirst();
                    }    
                    

                }

                Console.Read();
                



            }

        static void sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine(e.Result.Confidence + " " + e.Result.Text);
            if (e.Result.Confidence > 0)
            {
                string txt = e.Result.Text;
                float confidence = e.Result.Confidence;
                Console.WriteLine("\nRecognized: " + txt);
                //   if (confidence < 0.40) return; // arbitrary constant
                //begin handling

                String reply = Temp.First.Value.GetReply(txt); // current state 

                if (txt.Length > 0)
                {
                    Console.WriteLine("\n<Saying>" + reply);
                    ss.Speak(reply);
                }
                
            }
        }

        static void sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.WriteLine(e.Result.Text);
        }
        

        static void sre_SpeechRecognized(object sender,
      SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;
            Console.WriteLine("\nRecognized: " + txt);
         //   if (confidence < 0.40) return; // arbitrary constant
            //begin handling

            String reply = Temp.First.Value.GetReply(txt); // current state 
            
            if (txt.Length > 0) {
                Console.WriteLine("\n<Saying>" + reply);
                
                ss.Speak(reply);
            }
            
        } // sre_SpeechRecognized

    static void MakeStates(string serializationFile)
        {
            //setting call sign here, will remain unchanged throughout
            //stateReplies adds the callsign.
            //no capitalization, no full stops.
            String callSign = "indigo four five six";

            //delivery
            String[] stateReplies = new String[]{"validate readback",
                                                 "this is Bangalore Delivery. Start up approved. Cleared for IFR to Delhi as filed. Depart runway niner. Squawk seven one three two. Monitor ATIS zulu.",
                                                 "validate readback",
                                                 "when ready contact apron for pushback on one two one decimal eight seven five.",
                                                 "validate readback",""};

            String[][] readbackInfo = { new String[]{"stand two zero seven, requesting start up information to delhi",
                                                     "requesting start up information for delhi",
                                                     "stand two zero seven, request startup for delhi",
                                                     "request startup as filled for delhi"},
                                        new String[]{"start up approved, cleared to delhi, departure via runway niner with zulu",
                                                     "cleared to delhi, depart runway niner, squawk seven one three two with zulu",
                                                     "start up approved, cleared to delhi as filed, depart runway niner with zulu, squawk seven one three two",
                                                     "cleared to delhi, runway niner with zulu",
                                                     "roger that"},
                                        new String[]{"contact apron on decimal eight seven five when ready",
                                                     "contact apron on one two one decimal eight seven five",
                                                     "apron on decimal eight seven five",
                                                     "decimal eight seven five for pushback",
                                                     "roger that"}};
            String atcName = "bangalore clearance";
            String stateName = "delivery";
            State delivery = new State(stateName, stateReplies, readbackInfo, callSign, atcName);

            //pushback
            stateReplies = new String[] { };
            readbackInfo = new String[][] { new String[] { "" }, new String[] { } };

            atcName = "apron";
            stateName = "pushback";
            State pushback = new State(stateName, stateReplies, readbackInfo, callSign, atcName);

            //taxi
            stateReplies = new String[] { };
            readbackInfo = new String[][] { new String[] { }, new String[] { }, new String[] { }, new String[] { } };

            atcName = "bangalore ground";
            stateName = "taxi";
            State taxi = new State(stateName, stateReplies, readbackInfo, callSign, atcName);

            //takeoff
            stateReplies = new String[] { };
            readbackInfo = new String[][] { new String[] { }, new String[] { }, new String[] { } };

            atcName = "bangalore tower";
            stateName = "takeoff";
            State takeoff = new State(stateName, stateReplies, readbackInfo, callSign, atcName);

            //departure
            stateReplies = new String[] { };
            readbackInfo = new String[][] { new String[] { }, new String[] { }, new String[] { } };

            atcName = "bangalore departure";
            stateName = "departure";
            State departure = new State(stateName, stateReplies, readbackInfo, callSign, atcName);

            //following
            stateReplies = new String[] { };
            readbackInfo = new String[][] { new String[] { }, new String[] { }, new String[] { } };

            atcName = "mumbai centre";
            stateName = "following";
            State following = new State(stateName, stateReplies, readbackInfo, callSign, atcName);

            //arrival
            stateReplies = new String[] { };
            readbackInfo = new String[][] { new String[] { }, new String[] { }, new String[] { } };

            atcName = "delhi arrival";
            stateName = "arrival";
            State arrival = new State(stateName, stateReplies, readbackInfo, callSign, atcName);

            //landing
            stateReplies = new String[] { };
            readbackInfo = new String[][] { new String[] { }, new String[] { }, new String[] { } };

            stateName = "landing";
            atcName = "delhitower";
            State landing = new State(stateName, stateReplies, readbackInfo, callSign, atcName);

            //ground
            stateReplies = new String[] { };
            readbackInfo = new String[][] { new String[] { }, new String[] { }, new String[] { } };

            atcName = "delhi ground";
            stateName = "delhiground";
            State ground = new State(stateName, stateReplies, readbackInfo, callSign, atcName);

            //making the list
            States.AddFirst(delivery);
            States.AddLast(pushback);
            States.AddLast(taxi);
            States.AddLast(takeoff);
            States.AddLast(landing);
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
