while True:
	if linda.try_query("done"):
		break
	
	a_tuple = linda.get("a", None)
	b_tuple = linda.get("b", None)
	ind_tuple = linda.get("ind", None)

	a = a_tuple[1]
	b = b_tuple[1]
	ind = ind_tuple[1]

	ind += 1
	c = a + b

	linda.put("fib", ind, c)

	linda.put("a", b)
	linda.put("b", c)
	linda.put("ind", ind)