import clr
clr.AddReference("System.Numerics")
from System.Numerics import BigInteger

a_tuple = linda.In(('a', None))
b_tuple = linda.In(('b', None))
iteration_tuple = linda.In(('iteration', None))

a = a_tuple[1]
b = b_tuple[1]
iteration = iteration_tuple[1]

iteration += 1

c = a + b

linda.Out(('a', b))
linda.Out(('b', c))

if iteration != 9:
	linda.Out(('iteration', iteration))
else:
	linda.Out(('done',))