cd .\ASM1_DataAgent\
start "ASM_DA1" SapientDataAgent.exe 14005
cd ..
cd .\ASM1\
start "ASM1" SapientAsmSimulator.exe 14005 cb09bb6a-5b83-4dd8-88f1-2db8ea6d656b
cd ..