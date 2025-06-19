[ENABLE]
aobscanmodule(health, HMA.exe, D9 86 18 02 00 00 D8 65) // should be unique
label(healthjmp)
label(healthplaya)
registersymbol(healthjmp)              
health+4:
readmem(healthplaya-5,5)
    mov [rax+2C],47C34F80

healthjmp:
    fld dword ptr[esi + 0000021C]
    fadd dword ptr[ebp - 04]
    mov [r13+rbp*4+00], #99

[DISABLE]
healthjmp:
    fld dword ptr[esi + 00000218]
    fsub dword ptr[ebp - 04]
unregistersymbol(healthjmp)