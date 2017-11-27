using System;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Globalization;
namespace StatesAndGrammars
{
    
    public class Grammars
    {

         public static SpeechRecognitionEngine GetGrammars(SpeechRecognitionEngine sre, State state)
        {
          

            sre.UnloadAllGrammars();
            
            //state readbacks
            Choices answer = new Choices();
            foreach (String[] i in state.readbackInfo)
            {
                answer.Add(i);
            }
            GrammarBuilder gb_readback = new GrammarBuilder();
            GrammarBuilder gb_request = new GrammarBuilder();
            gb_readback.Append(answer);
            gb_readback.Append(state.GetCallSign());
            //gb_readback.Append(atc, 0, 1);
            gb_request.Append(state.GetAtcName(), 0, 1);
            gb_request.Append("this is", 0, 1);
            gb_request.Append(state.GetCallSign());
            gb_request.Append(answer);

            Grammar g_request = new Grammar(gb_request);    
            Grammar g_readback = new Grammar(gb_readback);
            sre.LoadGrammar(g_readback);
            sre.LoadGrammar(g_request);
            
            //appending dictation grammar, will pick up all that is said
            //only for debugging, comment out otherwise
            //GrammarBuilder gb_dictation = new GrammarBuilder();
            //gb_dictation.AppendDictation();
            //Grammar g_dictation = new Grammar(gb_dictation);
            //sre.LoadGrammar(g_dictation);
            
            return sre;



        }
        
        
    }
    [Serializable]
    public class State
    {
        //for a state which begins with ATC contact, speak first command from constructor, validate readback is first stateReply
        public readonly String stateName;
        private bool completed = false;
        private int stateCount = 0;
        private int readbackCount = 0;
        private String[] stateReplies;
       
        internal String[][] readbackInfo;
        private string callSign;
        private string atcName;
        public bool IsCompleted()
        {
            return completed;
        }
        private void UpdateCompletion()
        {
            if (stateCount >= stateReplies.Length)
            {
                completed = true;
            }
            //Console.WriteLine(completed);
        }

        public String GetReply(String pilotTxt)
        {
            if (IsCompleted()) { return ""; }
            String reply = "";
            if (stateCount < stateReplies.Length)
            {

                if (stateReplies[stateCount] == "validate readback")
                {
                    pilotTxt = pilotTxt.ToLower();
                    Console.WriteLine("validating..");
                    String[] readback = readbackInfo[readbackCount];
                    bool validated = false;
                    foreach (String value in readback)
                    {
                        //Console.Write(value);
                        if (pilotTxt.Contains(value))
                        {

                            validated = true;
                            Console.WriteLine("validated!");
                            stateCount++;
                            readbackCount++;
                            if (stateReplies[stateCount] != "")
                            {
                                reply = callSign + " " + stateReplies[stateCount++]; //for atc initiated contact
                            }
                            else 
                            {
                                reply = stateReplies[stateCount++];
                            }
                            break;
                        }

                    }
                    if (!validated)
                    {
                        reply = callSign + " say again";
                    }
                }
                else
                {
                    reply = callSign + " " + stateReplies[stateCount++];
                }

            }
            UpdateCompletion();
            return reply;
        }
        public String GetCallSign()
        {
            return callSign;
        }
        public String GetAtcName() 
        {
            return atcName;
        }



        public State(String StateName, String[] stateReplies, String[][] readbackInfo, String callSign, String atcName)
        {

            this.stateName = StateName;
            this.stateReplies = stateReplies;
            this.readbackInfo = readbackInfo;
            this.callSign = callSign;
            this.atcName = atcName;

        }
        public State()
        {
            this.stateName = "";
            this.stateReplies = new String[] { };
            this.readbackInfo = new String[][] { };
            this.callSign = "";
            this.atcName = "";
        }



    }

}
