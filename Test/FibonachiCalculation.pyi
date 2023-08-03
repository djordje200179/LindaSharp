from System import Array, Object

a_tuple = linda.In(Array[Object](['a', None]))
b_tuple = linda.In(Array[Object](['b', None]))
iteration_tuple = linda.In(Array[Object](['iteration', None]))

a = int(a_tuple[1])
b = int(b_tuple[1])
iteration = int(iteration_tuple[1])

iteration += 1

c = a + b

linda.Out(Array[Object](['a', b]))
linda.Out(Array[Object](['b', c]))

if iteration != 9:
	linda.Out(Array[Object](['iteration', iteration]))
else:
	linda.Out(Array[Object](['done']))