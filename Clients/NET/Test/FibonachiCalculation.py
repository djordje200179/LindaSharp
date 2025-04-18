while True:
	if linda.try_query("done"):
		break
	
	_, a = linda.get("a", None)
	_, b = linda.get("b", None)
	_, ind = linda.get("ind", None)

	ind += 1
	c = a + b

	linda.put("fib", ind, c)

	linda.put("a", b)
	linda.put("b", c)
	linda.put("ind", ind)