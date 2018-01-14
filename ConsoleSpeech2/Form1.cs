using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using StatesAndGrammars;

namespace ConsoleSpeech2
{
    public partial class Form1 : Form
    {
        static LinkedList<State> States = new LinkedList<State>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //setting call sign here, will remain unchanged throughout
            String callSign = textBox1.Text;

            //delivery

            String[] stateReplies = textBox4.Text.Split('#');
      
            List<String[]> readbackInfo=new List<String[]>();
           
            String[] temp = textBox2.Text.Split('\n');
            foreach (String test in temp)
            {
                    String[] aaa = test.Split('#');
                    readbackInfo.Add(aaa);
            }
            
            
            String atcName = textBox3.Text;
            String stateName = "delivery";
            State delivery = new State(stateName, stateReplies, readbackInfo.ToArray(), callSign, atcName);
            
            //pushback
            stateReplies = textBox7.Text.Split('#');
            readbackInfo.Clear();
            temp = textBox6.Text.Split('\n');
            foreach (String test in temp)
            {
                String[] aaa = test.Split('#');
                readbackInfo.Add(aaa);
            }

            atcName = textBox5.Text;
            stateName = "push back";
            State pushback = new State(stateName, stateReplies, readbackInfo.ToArray(), callSign, atcName);
            
            //taxi
            stateReplies = textBox10.Text.Split('#');
            readbackInfo.Clear();
            temp = textBox9.Text.Split('\n');
            foreach (String test in temp)
            {
                String[] aaa = test.Split('#');
                readbackInfo.Add(aaa);
            }
            stateName = "taxi";

            State taxi = new State(stateName, stateReplies, readbackInfo.ToArray(), callSign, atcName);

            //takeoff
            stateReplies = textBox13.Text.Split('#');

            readbackInfo.Clear();
            temp = textBox12.Text.Split('\n');
            foreach (String test in temp)
            {
                String[] aaa = test.Split('#');
                readbackInfo.Add(aaa);
            }
            State takeoff = new State(stateName, stateReplies, readbackInfo.ToArray(), callSign, atcName);

            //landing
            stateReplies = textBox16.Text.Split('#');
            readbackInfo.Clear();
            temp = textBox15.Text.Split('\n');
            foreach (String test in temp)
            {
                String[] aaa = test.Split('#');
                readbackInfo.Add(aaa);
            }
            atcName = textBox14.Text;

            State landing = new State(stateName, stateReplies, readbackInfo.ToArray(), callSign, atcName);

            //ground
            stateReplies = textBox19.Text.Split('#');
            readbackInfo.Clear();
            temp = textBox18.Text.Split('\n');
            foreach (String test in temp)
            {
                String[] aaa = test.Split('#');
                readbackInfo.Add(aaa);
            }
            atcName = textBox17.Text;
            stateName = "ground";
            State ground = new State(stateName, stateReplies, readbackInfo.ToArray(), callSign, atcName);
            //making the list
            States.AddFirst(delivery);
            States.AddLast(pushback);
            States.AddLast(taxi);
            States.AddLast(takeoff);
            States.AddLast(landing);
            States.AddLast(ground);
            //serialize
            string dir = Directory.GetCurrentDirectory();
            string serializationFile = Path.Combine(dir, "states.bin");
            using (Stream stream = File.Open(serializationFile, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, States);
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {

            string dir = Directory.GetCurrentDirectory();
            string serializationFile = Path.Combine(dir, "states.bin");
            using (Stream stream = File.Open(serializationFile, FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                States = (LinkedList<State>)bformatter.Deserialize(stream);

            }
            Console.WriteLine(States.First.Value);

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
    
}
