using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archi_Ver1
{
    class MIPS
    {
        public int[] registers;
        public Hashtable Instructions, DataMemory;
        public string[] IF_ID = new string[2];
        public string[] ID_EX = new string[9];
        public string[] Ex_MEM = new string[7];
        public string[] MEM_WB = new string[4];
        public Queue FetchQ = new Queue();
        public string[] StagesQ = new string[5];
        public MIPS()
        {
            registers = new int[32];
            Instructions = new Hashtable();
            DataMemory = new Hashtable();
            IF_ID = new string[2];
             ID_EX = new string[9];
             Ex_MEM = new string[8];
             MEM_WB = new string[4];
             FetchQ = new Queue();
             StagesQ = new string[5];
            for (int i = 0; i < 5; i++)
                StagesQ[i] = "";
            init();

        }
        public int BinaryToDecimal(string Bin)
        {
            double ret = 0, z = 0;
            for (int i = Bin.Length - 1; i >= 0; i--)
            {
                if (Bin[i] == '1')
                    ret += Math.Pow(2, z);
                z++;
            }
            return Convert.ToInt32(ret);
        }
        public void init()
        {
            registers[0] = 0;
            for (int i = 1; i < 32; i++)
            {
                registers[i] = i + 100;
            }
        }
        public int Adder(int x, int y) { return x + y; }
        public string[] controlUnit(string instr)
        {
            string[] ret = new string[3];
            if (instr.Substring(0, 6) == "000000")//R-type
            {
                ret[0] = "1100";
                ret[1] = "000";
                ret[2] = "10";
            }
            else //I-type
            {
                ret[0] = "0001";
                ret[1] = "010";
                ret[2] = "11";
            }
            return ret;
        }
        public int[] regFile(int[] reg, int writeRegister, int WriteData, char RegWrite)
        {
            int[] ret = new int[2];
            ret[0] = -1;
            ret[1] = -1;
            if (RegWrite == '1')
            {
                ret[0] = registers[reg[0]];
                ret[1] = registers[reg[1]];

            }
            return ret;
        }
        public int dataMem(int address, int writedata, char memWrtie, char memRead)
        {
            if (memRead == '1')
            {
                return Convert.ToInt32(DataMemory[address]);
            }
            DataMemory[address] = writedata;
            return -1;
        }
        public int[] Alu(int x, int y, string aluOP, string funct)
        {
            int[] ret = new int[2];
            ret[0] = 0;
            int res = 0;
            if (aluOP == "00")
            {
                ret[1] = x + y;
                return ret;
            }
            if (funct == "100000")
            {
                res = Adder(x, y);
            }
            else if (funct == "100100")
            {
                res = x & y;
            }
            else if (funct == "100010")
            {
                res = x - y;
            }
            else if (funct == "100101")
            {
                res = x | y;
            }

            if (res == 0) ret[0] = 1;
            ret[1] = res;
            Console.WriteLine(funct);
            return ret;
        }
        public T mux<T>(T s1, T S2, char selector)
        {
            if (selector == '0')
                return s1;
            return S2;
        }
        public string signExtend(string address)
        {
            if (address[0] == '1')
                return "1111111111111111" + address;
            return "0000000000000000" + address;

        }
        public void fetch(int PC, string Instr)
        {
            init();
            IF_ID[0] = (PC + 4).ToString();
            IF_ID[1] = Instr;
            print(IF_ID);

        }
        public void decode()
        {
            string[] ret_control = new string[3];
            ret_control = controlUnit(IF_ID[1]);

            int rs = BinaryToDecimal(IF_ID[1].Substring(6, 5));
            int rt = BinaryToDecimal(IF_ID[1].Substring(11, 5));
            int[] ret_regFile = new int[2];
            ret_regFile = regFile(new int[] { rs, rt }, -1, -1, ret_control[2][0]);

            ID_EX[0] = ret_control[0];
            ID_EX[1] = ret_control[1];
            ID_EX[2] = ret_control[2];
            ID_EX[3] = IF_ID[1];
            ID_EX[4] = ret_regFile[0].ToString();
            ID_EX[5] = ret_regFile[1].ToString();
            ID_EX[6] = signExtend(IF_ID[1].Substring(16, 16));
            ID_EX[7] = IF_ID[1].Substring(11, 5);
            ID_EX[8] = IF_ID[1].Substring(16, 5);
            print(ID_EX);

        }
        public void Execute()
        {
            string Exec_controlLine = ID_EX[0];
            char AluSrc = Exec_controlLine[3];
            string ALUop = Exec_controlLine.Substring(1, 2);
            char regDst = Exec_controlLine[0];
            string data2 = mux(ID_EX[5], ID_EX[6], AluSrc);
            int y = Convert.ToInt32(ID_EX[5]);
            if (AluSrc == '1')
            {
                y = BinaryToDecimal(data2);
            }
            int[] ret_ALU = new int[2];
            ret_ALU = Alu(Convert.ToInt32(ID_EX[4]), y, ALUop, ID_EX[6].Substring(26, 6));
            Ex_MEM[0] = ID_EX[1];
            Ex_MEM[1] = ID_EX[2];
            Ex_MEM[2] = ID_EX[3];
            Ex_MEM[3] = ret_ALU[0].ToString();
            Ex_MEM[4] = ret_ALU[1].ToString();
            Ex_MEM[5] = ID_EX[5];
            Ex_MEM[6] = mux(ID_EX[7], ID_EX[8], regDst);
            print(Ex_MEM);

        }
        public void Memory()
        {
            char memWrite = Ex_MEM[0][2], memRead = Ex_MEM[0][1];
            int ret_dataMem = dataMem(Convert.ToInt32(Ex_MEM[4]), Convert.ToInt32(Ex_MEM[5]), memWrite, memRead);
            MEM_WB[0] = Ex_MEM[1];
            MEM_WB[1] = ret_dataMem.ToString();
            MEM_WB[2] = Ex_MEM[4];
            MEM_WB[3] = Ex_MEM[6];
            print(MEM_WB);

        }
        public void WriteBack()
        {
            char memtoreg = MEM_WB[0][1];
            int readData = mux(Convert.ToInt32(MEM_WB[2]), Convert.ToInt32(MEM_WB[1]), memtoreg);
            print(new string[] { memtoreg.ToString(), readData.ToString(), MEM_WB[3] });

        }
        public void Cycle()
        {
            if (StagesQ[4] != "")
            {
                WriteBack();
                StagesQ[4] = "";
            }
            if (StagesQ[3] != "")
            {
                Memory();
                StagesQ[4] = StagesQ[3];
                StagesQ[3] = "";
            }
            if (StagesQ[2] != "")
            {
                Execute();
                StagesQ[3] = StagesQ[2];
                StagesQ[2] = "";

            }
            if (StagesQ[1] != "")
            {
                decode();
                StagesQ[2] = StagesQ[1];
                StagesQ[1] = "";

            }
            if (StagesQ[0] != "")
            {
                int key = Convert.ToInt32(StagesQ[0]);
                fetch(key, Instructions[key].ToString());
                StagesQ[1] = StagesQ[0];
                StagesQ[0] = "";

            }
            if (FetchQ.Count > 0)
            {
                StagesQ[0] = FetchQ.Dequeue().ToString();
            }
        }
        public void print(string[] pipe)
        {
            for (int i = 0; i < pipe.Length; i++)
            {
                Console.Write(pipe[i] + " ");
            }
            Console.WriteLine();
            Console.WriteLine("=============");
        }

    }
}
