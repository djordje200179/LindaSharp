import os
from enum import Enum
from abc import abstractmethod
from dataclasses import dataclass
from typing import Callable, Self

class ILinda:
	@abstractmethod
	async def put(self, *tuple) -> None:
		pass

	@abstractmethod
	async def get(self, *pattern) -> list:
		pass

	@abstractmethod
	async def query(self, *pattern) -> list:
		pass

	@abstractmethod
	async def try_get(self, *pattern) -> (list | None):
		pass

	@abstractmethod
	async def try_query(self, *pattern) -> (list | None):
		pass

class IActionEvalLinda(ILinda):
	@abstractmethod
	def eval(self, func: Callable[[Self], None]) -> None:
		pass

class IScriptEvalLinda(ILinda):
	@dataclass
	class Script:
		class Type(Enum):
			IRONPYTHON = 0
			C_SHARP = 1

		type: Type
		code: str

		@staticmethod
		def from_file(file_path: str) -> Self:
			_, extension = os.path.splitext(file_path)
			match extension.lower():
				case ".py":
					typ = IScriptEvalLinda.Script.Type.IRONPYTHON
				case ".cs":
					typ = IScriptEvalLinda.Script.Type.C_SHARP
				case _:
					raise ValueError(f"Unsupported script file extension: {extension}")

			# read file content
			with open(file_path, "r") as file:
				code = file.read()

			return IScriptEvalLinda.Script(typ, code)

	class ScriptExecutionStatus(Enum):
		NOT_FOUND = 0
		FINISHED = 1
		EXCEPTION = 2

	@abstractmethod
	async def register_script(self, key: str, script: Script) -> None:
		pass

	async def register_script_file(self, key: str, file_path: str) -> None:
		script = IScriptEvalLinda.Script.from_file(file_path)
		await self.register_script(key, script)

	@abstractmethod
	async def invoke_script(self, key: str, parameter: any = None) -> int:
		pass

	@abstractmethod
	async def eval_script(self, script: Script) -> int:
		pass

	async def eval_script_file(self, file_path: str) -> int:
		script = IScriptEvalLinda.Script.from_file(file_path)
		return await self.eval_script(script)

	@abstractmethod
	async def get_script_execution_status(self, task_id: int) -> ScriptExecutionStatus:
		pass