start:
	MOVE ra, 0
loop:
	ADD  ra, 1
	COMP ra, 100
	JIFZ :end
	JUMP :loop
end:
	HALT