while True:
	if linda.rdp(("done", )):
		break
	
	a_tuple = linda.in_(("a", None))
	b_tuple = linda.in_(("b", None))
	ind_tuple = linda.in_(("ind", None))

	a = a_tuple[1]
	b = b_tuple[1]
	ind = ind_tuple[1]

	ind += 1

	c = a + b

	linda.out(("fib", ind, c))

	linda.out(("a", b))
	linda.out(("b", c))
	linda.out(("ind", ind))