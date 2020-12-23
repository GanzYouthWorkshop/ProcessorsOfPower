start:
	MOVE ra, 0
loop:
	ADD  ra, 1
	COMP ra, 10
	JNOZ :end
	JUMP :loop
end:
	HALT