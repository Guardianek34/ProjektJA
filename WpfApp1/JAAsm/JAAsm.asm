.data
float1		real4   0.2126
float2		real4	0.7152
float3		real4	0.0722
r4_0p5      real4	0.5
r4_255p0    real4	255.0

;													RCX			  RDX		  R8		
; unsafe private static extern void GrayscaleAsm(byte* pCur, byte* output, int width);
.code
GrayscaleAsm proc
        align 16

        push rax
        push r15

        vbroadcastss ymm10,real4 ptr [float1] ;ymm10 = packed coef[0]
        vbroadcastss ymm11,real4 ptr [float2] ;ymm11 = packed coef[1]
        vbroadcastss ymm12,real4 ptr [float3] ;ymm12 = packed coef[2]
        ;vbroadcastss ymm13,real4 ptr [r4_0p5] ;ymm13 = packed 0.5
        ;vbroadcastss ymm14,real4 ptr [r4_255p0] ;ymm14 = packed 255.0
        ;vxorps ymm15,ymm15,ymm15                ;ymm15 = packed 0.0
        xor rax, rax
        xor r15, r15
        mov r8d,r8d                             ;r8 = num_pixels
        mov r10,8                               ;r10 - number of pixels / iteration

@@:     pmovzxbd xmm0,dword ptr [rcx+r15]
        add r15, 4
        pmovzxbd xmm1,dword ptr [rcx+r15]	
        add r15, 4
        pmovzxbd xmm2,dword ptr [rcx+r15]	
        add r15, 4

        pmovzxbd xmm3,dword ptr [rcx+r15]
        add r15, 4
        pmovzxbd xmm4,dword ptr [rcx+r15]
        add r15, 4
        pmovzxbd xmm5,dword ptr [rcx+r15]
        add r15, 4

        vinsertf128  ymm0, ymm0, xmm3, 1	; ymm0 = m03
        vinsertf128  ymm1, ymm1, xmm4, 1	; ymm1 = m14
        vinsertf128  ymm2, ymm2, xmm5, 1	; ymm2 = m25

        vshufps ymm3, ymm1, ymm2, 158	; ymm3 = xy (shuffle m14, m25)
        vshufps ymm4, ymm0, ymm1, 73	; ymm4 = yz (shuffle m03, m14)
        vshufps ymm5, ymm0, ymm3, 140	; ymm5 = x (shuffle m03, xy)
        vshufps ymm6, ymm4, ymm3, 216	; ymm6 = y (shuffle yz, xy)
        vshufps ymm7, ymm4, ymm2, 205	; ymm7 = z (shuffle yz, m25)

        vmovaps ymm0, ymm5;
        vmovaps ymm1, ymm6;
        vmovaps ymm2, ymm7;

        vcvtdq2ps ymm0,ymm0 ;ymm0 = 8 pixels SPFP (r values)
        vcvtdq2ps ymm1,ymm1 ;ymm1 = 8 pixels SPFP (g values)
        vcvtdq2ps ymm2,ymm2 ;ymm2 = 8 pixels SPFP (b values)

        vmulps ymm0,ymm0,ymm10              ;ymm0 = r values * coef[0]
        vmulps ymm1,ymm1,ymm11              ;ymm1 = g values * coef[1]
        vmulps ymm2,ymm2,ymm12              ;ymm2 = b values * coef[2]

        vaddps ymm3,ymm0,ymm1               ;r + g
        vaddps ymm4,ymm3,ymm2               ;r + g + b       
        ;vaddps ymm5,ymm4,ymm13              ;r + g + b + 0.5 
        ;vminps ymm0,ymm4,ymm14              ;clip pixels above 255.0
        ;vmaxps ymm1,ymm0,ymm15              ;clip pixels below 0.0

        vcvtps2dq ymm2,ymm1 ;convert SPFP values to dwords
        vpackusdw ymm3,ymm2,ymm2
        vextracti128 xmm4,ymm3,1
        vpackuswb xmm5,xmm3,xmm4 ;byte GS pixels in xmm5[31:0] and xmm5[95:64]
        vpextrd r11d,xmm5,0 ;r11d = 4 grayscale pixels
        mov dword ptr [rdx+rax],r11d ;save grayscale image pixels
        vpextrd r11d,xmm5,2 ;r11d = 4 grayscale pixels
        mov dword ptr [rdx+rax+4],r11d ;save grayscale image pixels

        add rax,r10
        sub r8,r10
        jnz @B

Done:   vzeroupper
        pop rax
        pop r15
        ret
GrayscaleAsm endp
end