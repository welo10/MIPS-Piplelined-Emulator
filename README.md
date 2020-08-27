# MIPS-Piplelined-Emulator
MIPS pipelined CPU which can show the values of internal CPU components cycle by cycle for any given machine code. 
## MIPS Pipleline Briefly Description:
### MIPS Pipeline Architecture 
![](https://github.com/welo10/MIPS-Piplelined-Emulator/blob/master/docs/Pipeline%20Stages.png)
### MIPS 5-Stages
1. IF: Instruction fetch from memory
2. ID: Instruction decode & register read
3. EX: Execute operation or calculate address
4. MEM: Access memory operand
5. WB: Write result back to register
### Supported Instructions
- add
- or
- sub
- lw
- and
### Instructions Format
![](https://github.com/welo10/MIPS-Piplelined-Emulator/blob/master/docs/Instruction%20format.JPG)
### Cycles
![](https://github.com/welo10/MIPS-Piplelined-Emulator/blob/master/docs/Cycles.JPG)
### Instruction Examlpe
![](https://github.com/welo10/MIPS-Piplelined-Emulator/blob/master/docs/Example.JPG)
### How to use the emulator
1- Write an address then a machine code or assembly code.\
2- you can convert from machine code to assembly code and vice versa.\
3- Click on initialize button to initailize the data memory, registers and pipeline registers.\
4- Click on Run Cycle and observe the CPU internal values, registers and data memory.
### Tests to run:
#### Assembly code
1000: and $3, $4, $30\
1004: add $6, $8, $20\
1008: lw $6, 8(9$)\
1012: sub $10, $9, $5\
1016: or $3, $16, $8
#### Machine code
1000: 00000000100111100001100000100100\
1004: 00000001000101000011000000100000\
1008: 10001101001001100000000000001000\
1012: 00000001001001010101000000100010\
1016: 00000010000010000001100000100101
![](https://github.com/welo10/MIPS-Piplelined-Emulator/blob/master/docs/snap.JPG)
