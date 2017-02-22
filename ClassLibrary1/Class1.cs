using System;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Globalization;
namespace StatesAndGrammars
{

    public class Grammars
    {
        static SpeechRecognitionEngine Pushback(SpeechRecognitionEngine sre, String atcName, String callSign)
        {

            //request for pushback


            Choices request = new Choices(new string[] { "request", "requesting", "ready" });
            GrammarBuilder gb_request = new GrammarBuilder();
            gb_request.Append(atcName, 0, 1);
            gb_request.Append(callSign);
            gb_request.Append(request);
            gb_request.Append("push back");

            Grammar g_request = new Grammar(gb_request);
            sre.LoadGrammarAsync(g_request);

            //pushback readback
            Choices answer = new Choices(new string[] { "roger", "push back approved facing south", "facing south pushback approved" });
            GrammarBuilder gb_readback = new GrammarBuilder();
            gb_readback.Append(answer);
            gb_readback.Append(callSign);
            //gb_readback.Append(atc, 0, 1);

            Grammar g_readback = new Grammar(gb_readback);
            sre.LoadGrammar(g_readback);

            return sre;

        }//set_grammars


        public static SpeechRecognitionEngine getGrammars(SpeechRecognitionEngine sre, State state)
        {
            if (state.stateName == "pushback")
            {
                return Pushback(sre, state.GetAtcName(), state.GetCallSign());
            }
            else
            {
                return Pushback(sre, state.GetAtcName(), state.GetCallSign()); //Change when adding new states!!
            }

        }
    }
    public class State
    {
        //for a state which begins with ATC contact, speak first command from constructor, validate readback is first stateReply
        public readonly String stateName;
        private bool completed = false;
        private int stateCount = 0;
        private int readbackCount = 0;
        private String[] stateReplies;
       
        private String[][] readbackInfo;
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
                        ///Console.Write(value);
                        if (pilotTxt.Contains(value))
                        {

                            validated = true;
                            Console.WriteLine("validated!");
                            stateCount++;
                            readbackCount++;
                            reply = "";
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



    }

}
