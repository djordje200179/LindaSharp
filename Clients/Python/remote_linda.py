from typing import Any, Sequence
import requests


class LindaDisposedException(Exception):
	def __str__(self):
		return "Server Linda was disposed"


class RemoteLinda:
	def __init__(self, host: str, port: int):
		self.__base_url = f"http://{host}:{port}/api/"
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

	def __wait_tuple(self, pattern: list, delete: bool) -> list:
		method = "DELETE" if delete else "GET"
		path = "in" if delete else "rd"
		response = self.__send_json_request(method, path, pattern)
		return response.json()

	def __try_get_tuple(self, pattern: list, delete: bool) -> (list | None):
		method = "DELETE" if delete else "GET"
		path = "in" if delete else "rd"
		response = self.__send_json_request(method, path, pattern)

		if response.status_code == requests.codes.not_found:
			return None

		return response.json()

	def put(self, *tuple) -> None:
		self.__send_json_request("POST", "out", tuple)

	def get(self, *pattern) -> list:
		return self.__wait_tuple(pattern, True)

	def query(self, *pattern) -> list:
		return self.__wait_tuple(pattern, False)

	def try_get(self, *pattern) -> (list | None):
		return self.__try_get_tuple(pattern, True)

	def try_query(self, *pattern) -> (list | None):
		return self.__try_get_tuple(pattern, False)

	def register_script(self, key: str, ironpython_code: str) -> None:
		self.__send_text_request("PUT", f"eval/{key}", ironpython_code, "text/ironpython")

	def register_script_file(self, key: str, ironpython_file_path: str) -> None:
		with open(ironpython_file_path, "r") as file:
			file_content = file.read()

		self.eval_register(key, file_content)

	def invoke_script(self, key: str, parameter: Any = None) -> None:
		self.__send_json_request("POST", f"eval/{key}", parameter)

	def eval_script(self, ironpython_code: str) -> None:
		self.__send_text_request("POST", "eval", ironpython_code, "text/ironpython")

	def eval_script_file(self, ironpython_file_path: str) -> None:
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
