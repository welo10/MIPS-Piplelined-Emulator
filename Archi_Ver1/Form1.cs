using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIPS_Emulator
{
    public partial class Form1 : Form
    {
        MIPS mips;
        int c;
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void clr()
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            dataGridView1.Rows.Add(new string[] { "Register", "Value" });
            dataGridView2.Rows.Add(new string[] { "Register", "Value" });
            dataGridView3.Rows.Add(new string[] { "Register", "Value" });
            c = 0;
            label2.Text = "Cycle Number: " + c;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            clr();
            mips = new MIPS();
            mips.init();
            for(int i = 0; i < 32; i++)
            {
                dataGridView3.Rows.Add(new string[] {"$"+i, mips.registers[i].ToString() });
            }
            for (int i = 0; i < 1024; i++)
            {
                dataGridView2.Rows.Add(new string[] { "$" + i, "99" });
            }
            string[] r = textBox1.Text.Split('\n');
            for(int i = 0; i < r.Length; i++)
            {
                if (r[i].Length >= 4)
                {
                    string[] tmp = r[i].Split(':');
                    string s = "";
                    for (int j = 0; j < tmp[1].Length; j++)
                        if (tmp[1][j] != ' ')
                            s += tmp[1][j];
                    mips.Instructions.Add(Convert.ToInt32(tmp[0]),s);
                    mips.FetchQ.Enqueue(tmp[0]);
                }
            }
            if (mips.FetchQ.Count > 0)
                mips.StagesQ[0] = mips.FetchQ.Dequeue().ToString();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            c = 0;
            clr();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            c++;
            label2.Text = "Cycle Number: " + c;
            int[] found = new int[] { 0, 0, 0, 0, 0 };
            for(int i = 0; i < 5; i++)
            {
                if (mips.StagesQ[i]!="")
                {
                    found[i] = 1;
                }
            }
            mips.Cycle();
       //     MessageBox.Show(mips.IF_ID[0] + mips.IF_ID[1]);
             string[] IF_ID = new string []{"Address","Instruction"};
         string[] ID_EX = new string[] {"EX","M","WB","Address","Read Data1","ReadData2","Sign Extend","Instruction[20-16]","Instruction[15-11]"};
         string[] Ex_MEM = new string[] {"M","WB","Address","Zero","Alu Result","Read Data 2","Mux"};
         string[] MEM_WB = new string[] {"WB","Read Data","ALU Result","Mux" };
            if(found[0]>0)
            for (int i = 0; i < 2; i++)
            {
                dataGridView1.Rows.Add(new string[] { "IF/ID:" + IF_ID[i], mips.IF_ID[i].ToString() });
            }
            if (found[1] > 0)
                for (int i = 0; i < 9; i++)
            {
                dataGridView1.Rows.Add(new string[] { "ID/EX:" + ID_EX[i], mips.ID_EX[i].ToString() });

            }
            if(found[2]>0)
            for (int i = 0; i <7; i++)
            {
                dataGridView1.Rows.Add(new string[] { "EX/MEM:" + Ex_MEM[i], mips.Ex_MEM[i].ToString() });
            }
            if (found[3] > 0)
            for (int i = 0; i < 4; i++)
            {
                dataGridView1.Rows.Add(new string[] { "MEM/WB:" + MEM_WB[i], mips.MEM_WB[i].ToString() });
            }
        }
    }
}
