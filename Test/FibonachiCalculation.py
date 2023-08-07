import clr
clr.AddReference("System.Numerics")
from System.Numerics import BigInteger

while True:
	if linda.Rdp(("done"))[0]:
		break
	
	a_tuple = linda.In(("a", None))
	b_tuple = linda.In(("b", None))
	ind_tuple = linda.In(("ind", None))

	a = a_tuple[1]
	b = b_tuple[1]
	ind = ind_tuple[1]

	ind += 1

	c = a + b

	linda.Out(("fib", ind.ToBigInteger(), c.ToBigInteger()))

	linda.Out(("a", b.ToBigInteger()))
	linda.Out(("b", c.ToBigInteger()))
	linda.Out(("ind", ind.ToBigInteger()))