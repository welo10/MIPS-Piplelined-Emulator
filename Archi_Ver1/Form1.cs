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
        private void init()
        {
            string[] r = textBox1.Text.Split('\n');
            for (int i = 0; i < r.Length; i++)
            {
                if (r[i].Length >= 4)
                {
                    string[] tmp = r[i].Split(':');
                    string s = "";
                    for (int j = 0; j < tmp[1].Length; j++)
                        if (tmp[1][j] != ' ')
                            s += tmp[1][j];
                    mips.Instructions.Add(Convert.ToInt32(tmp[0]), s);
                    mips.FetchQ.Enqueue(tmp[0]);
                }
            }
            if (mips.FetchQ.Count > 0)
                mips.StagesQ[0] = mips.FetchQ.Dequeue().ToString();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            clr();
            mips = new MIPS();
            mips.init();
            for (int i = 0; i < 32; i++)
            {
                dataGridView3.Rows.Add(new string[] { "$" + i, mips.registers[i].ToString() });
            }
            for (int i = 0; i < 1024; i++)
            {
                dataGridView2.Rows.Add(new string[] { "$" + i, "99" });
            }
            if (textBox1.Text.Length < 4)
            {
                convert_from_assembly_to_machine();
            }
            if (textBox2.Text.Length < 4)
            {
                convert_from_machine_to_assembly();
            }
            init();
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
            for (int i = 0; i < 5; i++)
            {
                if (mips.StagesQ[i] != "")
                {
                    found[i] = 1;
                }
            }
            mips.Cycle();
            //     MessageBox.Show(mips.IF_ID[0] + mips.IF_ID[1]);
            string[] IF_ID = new string[] { "Address", "Instruction" };
            string[] ID_EX = new string[] { "EX", "M", "WB", "Address", "Read Data1", "ReadData2", "Sign Extend", "Instruction[20-16]", "Instruction[15-11]" };
            string[] Ex_MEM = new string[] { "M", "WB", "Address", "Zero", "Alu Result", "Read Data 2", "Mux" };
            string[] MEM_WB = new string[] { "WB", "Read Data", "ALU Result", "Mux" };
            if (found[0] > 0)
                for (int i = 0; i < 2; i++)
                {
                    dataGridView1.Rows.Add(new string[] { "IF/ID:" + IF_ID[i], mips.IF_ID[i].ToString() });
                }
            if (found[1] > 0)
                for (int i = 0; i < 9; i++)
                {
                    dataGridView1.Rows.Add(new string[] { "ID/EX:" + ID_EX[i], mips.ID_EX[i].ToString() });

                }
            if (found[2] > 0)
                for (int i = 0; i < 7; i++)
                {
                    dataGridView1.Rows.Add(new string[] { "EX/MEM:" + Ex_MEM[i], mips.Ex_MEM[i].ToString() });
                }
            if (found[3] > 0)
                for (int i = 0; i < 4; i++)
                {
                    dataGridView1.Rows.Add(new string[] { "MEM/WB:" + MEM_WB[i], mips.MEM_WB[i].ToString() });
                }
        }
        private string formatBin(string Bin, int bits)
        {
            string ret = "";
            for (int i = 0; i < bits - Bin.Length; i++)
                ret += "0";
            ret += Bin;
            return ret;
        }
        private string Assembly_To_machine(char type, string funct, string rs, string rt, string rd)
        {
            string ret = "";
            if (type == 'R')
            {
                ret = "000000";
                string tmp =
                ret += formatBin(Convert.ToString(Convert.ToInt32(rs.Substring(1)), 2), 5);
                ret += formatBin(Convert.ToString(Convert.ToInt32(rt.Substring(1)), 2), 5);
                ret += formatBin(Convert.ToString(Convert.ToInt32(rd.Substring(1)), 2), 5);
                ret += "00000";

                if (funct == "add") ret += "100000";
                else if (funct == "sub") ret += "100010";
                else if (funct == "and") ret += "100100";
                else if (funct == "or") ret += "100101";


            }
            else
            {
                ret = "100011";


                ret += formatBin(Convert.ToString(Convert.ToInt32(rs.Substring(0)), 2), 5);
                ret += formatBin(Convert.ToString(Convert.ToInt32(rt.Substring(1)), 2), 5);
                ret += formatBin(Convert.ToString(Convert.ToInt32(rd), 2), 16);
            }
            return ret;
        }
        private void convert_from_assembly_to_machine()
        {
            string machine = "";
            if (textBox2.Text.Length > 4)
            {
                string[] r = textBox2.Text.Split('\n');
                for (int i = 0; i < r.Length; i++)
                {
                    if (r[i].Length >= 4)
                    {
                        string[] tmp = r[i].Split(':');
                        machine += tmp[0] + ": ";
                        string code = "";
                        if (tmp[1].Substring(1, 2) == "lw")
                        {
                            string rs, rt, rd, funct;
                            string[] tmp_regs;
                            tmp_regs = tmp[1].Substring(3).Split(',');
                            funct = tmp[1].Substring(1, 2);
                            rt = tmp_regs[0].Substring(1);
                            rd = tmp_regs[1].Substring(1, 1);
                            rs = tmp_regs[1].Substring(3);
                            rs = rs.Substring(0, rs.Length - 3);
                            code = Assembly_To_machine('I', funct, rs, rt, rd);

                        }
                        else
                        {
                            string[] tmp_regs;
                            string rs, rt, rd, funct;

                            if (tmp[1].Substring(1, 2) == "or")
                            {
                                tmp_regs = tmp[1].Substring(3).Split(',');
                                funct = "or";
                            }
                            else
                            {
                                tmp_regs = tmp[1].Substring(4).Split(',');
                                funct = tmp[1].Substring(1, 3);

                            }
                            rd = tmp_regs[0].Substring(1);
                            rs = tmp_regs[1].Substring(1);
                            rt = tmp_regs[2].Substring(1);
                            code = Assembly_To_machine('R', funct, rs, rt, rd);
                        }
                        machine += code + System.Environment.NewLine;

                    }
                }
                textBox1.Text = machine;
            }
            /*
1000: and $3, $4, $30
1004: add $6, $8, $20
1008: lw $6, 8(9$)
1012: sub $10, $9, $5
1016: or $3, $16, $8
             */

        }
        private string convert_from_machine_to_assembly()
        {
            string assembly = "";
            string[] r = textBox1.Text.Split('\n');
            for (int i = 0; i < r.Length; i++)
            {
                if (r[i].Length >= 4)
                {
                    string[] tmp = r[i].Split(':');
                    string row = tmp[0] + ": ";
                    string s = "";
                    for (int j = 0; j < tmp[1].Length; j++)
                        if (tmp[1][j] != ' ')
                            s += tmp[1][j];
                    string funct, rs, rt, rd;
                    if (s.Substring(0, 6) == "000000") //R-Type
                    {

                        rs = s.Substring(6, 5);
                        rt = s.Substring(11, 5);
                        rd = s.Substring(16, 5);
                        funct = s.Substring(26, 6);
                        string fun = "";
                        if (funct == "100000") fun = "add";
                        else if (funct == "100010") fun = "sub";
                        else if (funct == "100100") fun = "and";
                        else if (funct == "100101") fun = "or";
                        row += fun + " $" + Convert.ToInt32(rd, 2) + ", $" + Convert.ToInt32(rs, 2) + ", $" + Convert.ToInt32(rt, 2) + "\n";
                    }
                    else //I-Type
                    {
                        rs = s.Substring(6, 5);
                        rt = s.Substring(11, 5);
                        rd = s.Substring(16, 16);
                        funct = s.Substring(0, 6);
                        string fun = "";
                        if (funct == "100011") fun = "lw";
                        row += fun + " $" + Convert.ToInt32(rt, 2) + ", " + Convert.ToInt32(rd, 2) + "(" + Convert.ToInt32(rs, 2) + "$)";
                    }
                    assembly += row + System.Environment.NewLine;
                }
                textBox2.Text = assembly;
            }
            return assembly;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            convert_from_assembly_to_machine();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            convert_from_machine_to_assembly();
        }
    }
}
