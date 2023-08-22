from typing import Any, Sequence
import requests


class LindaDisposedException(Exception):
	def __str__(self):
		return "Server Linda was disposed"


class RemoteLinda:
	def __init__(self, host: str, port: int):
		self.__base_url = f"http://{host}:{port}/"
		self.__actions_url = self.__base_url + "actions/"
		self.__health_url = self.__base_url + "health/"

	def __send_text_request(self, http_method: str, path: str, data: any, content_type: str):
		url = self.__actions_url + path
		headers = {"Content-Type": content_type}
		response = requests.request(http_method, url, data=data, headers=headers)

		if response.status_code == requests.codes.server_error:
			raise LindaDisposedException()

		return response

	def __send_json_request(self, http_method: str, path: str, data: any):
		url = self.__actions_url + path
		response = requests.request(http_method, url, json=data)

		if response.status_code == requests.codes.server_error:
			raise LindaDisposedException()

		return response

	def __wait_tuple(self, tuple_pattern: list, delete: bool) -> list:
		method = "DELETE" if delete else "GET"
		path = "in" if delete else "rd"
		response = self.__send_json_request(method, path, tuple_pattern)
		return response.json()

	def __try_get_tuple(self, tuple_pattern: list, delete: bool) -> list:
		method = "DELETE" if delete else "GET"
		path = "in" if delete else "rd"
		response = self.__send_json_request(method, path, tuple_pattern)

		if response.status_code == requests.codes.not_found:
			return None

		return response.json()

	def out(self, tuple: Sequence) -> None:
		self.__send_json_request("POST", "out", tuple)

	def in_(self, tuple_pattern: Sequence) -> list:
		return self.__wait_tuple(tuple_pattern, True)

	def rd(self, tuple_pattern: Sequence) -> list:
		return self.__wait_tuple(tuple_pattern, False)

	def inp(self, tuple_pattern: Sequence) -> list:
		return self.__try_get_tuple(tuple_pattern, True)

	def rdp(self, tuple_pattern: Sequence) -> list:
		return self.__try_get_tuple(tuple_pattern, False)

	def eval_register(self, key: str, ironpython_code: str) -> None:
		self.__send_text_request("PUT", f"eval/{key}", ironpython_code, "text/ironpython")

	def eval_register_file(self, key: str, ironpython_file_path: str) -> None:
		with open(ironpython_file_path, "r") as file:
			file_content = file.read()

		self.eval_register(key, file_content)

	def eval_invoke(self, key: str, parameter: Any = None) -> None:
		self.__send_json_request("POST", f"eval/{key}", parameter)

	def eval(self, ironpython_code: str) -> None:
		self.__send_text_request("POST", "eval", ironpython_code, "text/ironpython")

	def eval_file(self, ironpython_file_path: str) -> None:
		with open(ironpython_file_path, "r") as file:
			file_content = file.read()

		self.eval(file_content)

	def is_healthy(self) -> bool:
		url = self.__health_url + "ping"
		
		try:
			response = requests.get(url, timeout=1)

			return response.status_code % 100 == 2
		except:
			return False
