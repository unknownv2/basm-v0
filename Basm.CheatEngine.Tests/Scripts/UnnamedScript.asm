[ENABLE]
aobscanmodule(health, HMA.exe, D9 86 18 02 00 00 D8 65) // should be unique
label(healthjmp)
registersymbol(healthjmp)
                
health+4:
    dd (float)-2.0
    fstp st(0)
    dd 0
@@:
    dd 0
    dd 0
    mov [edx+234], (float)999999 //fuel
    mov [rcx+20], (float)0.09666666754
    //readmem(healthplaya-5,5)
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