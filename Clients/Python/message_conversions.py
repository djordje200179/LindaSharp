from google.protobuf.internal.well_known_types import ListValue, Struct
from google.protobuf.struct_pb2 import Value
from linda import IScriptEvalLinda
from scripts_pb2 import Script as GrpcScript
from health_pb2 import ScriptExecutionStatus as GrpcScriptExecutionStatus

def elem_to_value(elem):
	match elem:
		case bool():
			return Value(bool_value=elem)
		case str():
			return Value(string_value=elem)
		case int():
			return Value(number_value=elem)
		case float():
			return Value(number_value=elem)
		case list():
			grpc_list = ListValue()
			grpc_list.values.extend(elem_to_value(e) for e in elem)
			return Value(list_value=grpc_list)
		case dict():
			grpc_struct = Struct()
			grpc_struct.update({k: elem_to_value(v) for k, v in elem.items()})
			return Value(struct_value=grpc_struct)
		case None:
			return Value(null_value=0)
		case _:
			raise ValueError(f"Unsupported type {type(elem)}")


def value_to_elem(value: Value):
	if value.HasField("bool_value"):
		return value.bool_value
	elif value.HasField("string_value"):
		return value.string_value
	elif value.HasField("number_value"):
		return value.number_value
	elif value.HasField("list_value"):
		return [value_to_elem(v) for v in value.list_value.values]
	elif value.HasField("struct_value"):
		return {k: value_to_elem(v) for k, v in value.struct_value.fields.items()}
	elif value.HasField("null_value"):
		return None
	else:
		raise ValueError(f"Unsupported type {value}")


def to_grpc_script(script: IScriptEvalLinda.Script) -> GrpcScript:
	match script:
		case IScriptEvalLinda.Script(IScriptEvalLinda.Script.Type.IRONPYTHON, code):
			return GrpcScript(type=GrpcScript.Type.IRONPYTHON, code=code)
		case IScriptEvalLinda.Script(IScriptEvalLinda.Script.Type.C_SHARP, code):
			return GrpcScript(type=GrpcScript.Type.C_SHARP, code=code)
		case _:
			raise ValueError(f"Unsupported script type {script.type}")

def from_grpc_script_execution_status(status: GrpcScriptExecutionStatus) -> GrpcScriptExecutionStatus:
	if status.HasField("not_found"):
		return IScriptEvalLinda.ScriptExecutionStatus.NOT_FOUND
	elif status.HasField("ok"):
		return IScriptEvalLinda.ScriptExecutionStatus.FINISHED
	elif status.HasField("exception"):
		grpc_extension = status.exception
		#TODO: Set exception message
		return IScriptEvalLinda.ScriptExecutionStatus.EXCEPTION