[ENABLE]
//aobscanmodule(healthreal ,ROTTR.exe + 6,F3 0F 11 70 2C 48 8B 8B A8) // should be unique
alloc(MemRegion,$1000)
label(patchAddress)
registersymbol(patchAddress)

MemRegion + 4:

patchAddress:
mov eax, 0 
mov [MemRegion], 1
nop 

[DISABLE]
unregistersymbol(patchAddress)
dealloc(MemRegion)