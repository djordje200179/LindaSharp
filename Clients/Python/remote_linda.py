import asyncio
import grpc
from google.protobuf.wrappers_pb2 import Int32Value, StringValue
from google.protobuf.empty_pb2 import Empty
from actions_pb2 import OptionalTuple, Pattern, Tuple
from actions_pb2_grpc import ActionsStub
from linda import IScriptEvalLinda
from scripts_pb2 import EvalScriptRequest, EvalScriptResponse, InvokeScriptRequest, RegisterScriptRequest
from scripts_pb2_grpc import ScriptsStub
from health_pb2_grpc import HealthStub
from health_pb2 import ScriptExecutionStatus as GrpcScriptExecutionStatus
from message_conversions import elem_to_value, from_grpc_script_execution_status, to_grpc_script, value_to_elem

class RemoteLinda(IScriptEvalLinda):
	def __init__(self, target: str):
		self.__channel = grpc.aio.insecure_channel(target)
		self.__health_stub = HealthStub(self.__channel)
		self.__actions_stub = ActionsStub(self.__channel)
		self.__scripts_stub = ScriptsStub(self.__channel)

	async def put(self, *tuple) -> None:
		request = Tuple()
		request.fields.extend(elem_to_value(elem) for elem in tuple)
		await self.__actions_stub.Out(request)

	async def get(self, *pattern) -> list:
		request = Pattern()
		request.fields.extend(elem_to_value(elem) for elem in pattern)
		response: Tuple = await self.__actions_stub.In(request)
		return [value_to_elem(value) for value in response.fields]

	async def query(self, *pattern) -> list:
		request = Pattern()
		request.fields.extend(elem_to_value(elem) for elem in pattern)
		response: Tuple = await self.__actions_stub.Rd(request)
		return [value_to_elem(value) for value in response.fields]

	async def try_get(self, *pattern) -> (list | None):
		request = Pattern()
		request.fields.extend(elem_to_value(elem) for elem in pattern)
		response: OptionalTuple = await self.__actions_stub.Inp(request)
		return [value_to_elem(value) for value in response.tuple.fields] if response.tuple else None

	async def try_query(self, *pattern) -> (list | None):
		request = Pattern()
		request.fields.extend(elem_to_value(elem) for elem in pattern)
		response: OptionalTuple = await self.__actions_stub.Rdp(request)
		return [value_to_elem(value) for value in response.tuple.fields] if response.tuple else None

	async def register_script(self, key: str, script: IScriptEvalLinda.Script) -> None:
		request = RegisterScriptRequest(key=key, script=to_grpc_script(script))
		await self.__scripts_stub.Register(request)

	async def invoke_script(self, key: str, parameter: any = None) -> int:
		request = InvokeScriptRequest(key=key, parameter=elem_to_value(parameter))
		response: EvalScriptResponse = await self.__scripts_stub.Invoke(request)
		return response.task_id

	async def eval_script(self, script: IScriptEvalLinda.Script) -> int:
		request = EvalScriptRequest(script=to_grpc_script(script))
		response: EvalScriptResponse = await self.__scripts_stub.Eval(request)
		return response.task_id

	async def is_healthy(self) -> bool:
		try:
			response: StringValue = await self.__health_stub.Ping(Empty())
			return response.value == "pong"
		except:
			return False

	async def get_script_execution_status(self, id: int) -> IScriptEvalLinda.ScriptExecutionStatus:
		request = Int32Value(value=id)
		response: GrpcScriptExecutionStatus = await self.__scripts_stub.GetStatus(request)
		return from_grpc_script_execution_status(response)

if __name__ == "__main__":
	async def main():
		linda = RemoteLinda("localhost:5001")

		while not await linda.is_healthy():
			print("Waiting for server...")
			await asyncio.sleep(1)

		result = await linda.get("fib", 101, None)
		print(result)

	loop = asyncio.get_event_loop()
	loop.run_until_complete(main())
	loop.close()