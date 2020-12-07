@base $1k

setup:
	MOVE ds, 0x1000
	NOPE			; }
	NOPE			; |
	NOPE			;  > some timing shit idk
	NOPE			; |
	NOPE			; }
loop:
	LOAD ra, $32k4	; load from memory, on VM should be 0 on firs try
	MOVE rc, 120
	MOVE ra, rc		; ra = 120
	MOVE rb, 150	; rb = 150
	ADD  ra, rb		; ra += rb
	SAVE $32k4, ra	; save ra to memory
	JUMP :loop		; loops indefinitely

test:
	NOPE
	HALT
	ADD  ra, [rb]
	SUB  ra, rb
	DIV  ra, rb
	MUL  ra, rb
	AND  ra, rb
	OR   ra, rb
	NOT  ra
	RSFT ra, rb
	LSFT ra, rb
	LROT ra, rb
	RROT ra, rb
	INCR  rc
	DECR rc

	HALT