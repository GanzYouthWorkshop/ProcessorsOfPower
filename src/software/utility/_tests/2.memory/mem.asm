start:
	MOVE ra, 0
loop:
	ADD  ra, 1		;increase iterator
	
	MOVE rb, 5001	; }
	ADD rb, ra		; |
	MOVE rc, 32		; | Set character value and memory address
	ADD rc, ra		; }
	MOVE [rb], rc	; Save value into memory
	
	COMP ra, 128	; Check iterator
	JIFZ :end		;
	
	JUMP :loop		; continure the iteration
end:
	HALT			;end of program