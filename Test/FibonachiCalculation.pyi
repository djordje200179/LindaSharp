a_tuple = linda.In(('a', None))
b_tuple = linda.In(('b', None))
iteration_tuple = linda.In(('iteration', None))

a = int(a_tuple[1])
b = int(b_tuple[1])
iteration = int(iteration_tuple[1])

iteration += 1

c = a + b

linda.Out(('a', b))
linda.Out(('b', c))

if iteration != 9:
	linda.Out(('iteration', iteration))
else:
	linda.Out(('done',))