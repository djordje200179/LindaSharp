from typing import Any
import requests


class ObjectDisposedException(Exception):
	def __str__(self):
		return "Server Linda was disposed"


class RemoteLinda:
	def __init__(self, host: str, port: int):
		self.__base_url = f"http://{host}:{port}/"

	def __send_text_request(self, http_method: str, linda_method: str, data: any, content_type: str):
		url = self.__base_url + linda_method
		headers = {"Content-Type": content_type}
		response = requests.request(http_method, url, data=data, headers=headers)

		if response.status_code == requests.codes.server_error:
			raise ObjectDisposedException()

		return response

	def __send_json_request(self, http_method: str, linda_method: str, data: any):
		url = self.__base_url + linda_method
		response = requests.request(http_method, url, json=data)

		if response.status_code == requests.codes.server_error:
			raise ObjectDisposedException()

		return response

	def __wait_tuple(self, tuple_pattern: list, method: str) -> list:
		response = self.__send_json_request("GET", method, tuple_pattern)
		return response.json()

	def __try_get_tuple(self, tuple_pattern: list, method: str) -> list:
		response = self.__send_json_request("GET", method, tuple_pattern)

		if response.status_code == requests.codes.not_found:
			return None

		return response.json()

	def out(self, tuple: list):
		self.__send_json_request("POST", "out", tuple)

	def in_(self, tuple_pattern: list) -> list:
		return self.__wait_tuple(tuple_pattern, "in")

	def rd(self, tuple_pattern: list) -> list:
		return self.__wait_tuple(tuple_pattern, "rd")

	def inp(self, tuple_pattern: list) -> list:
		return self.__try_get_tuple(tuple_pattern, "inp")

	def rdp(self, tuple_pattern: list) -> list:
		return self.__try_get_tuple(tuple_pattern, "rdp")

	def eval_register(self, key: str, ironpython_code: str):
		return self.__send_text_request("PUT", f"eval/{key}", ironpython_code, "text/ironpython")

	def eval_register_file(self, key: str, ironpython_file_path: str):
		with open(ironpython_file_path, "r") as file:
			file_content = file.read()

		self.eval_register(key, file_content)

	def eval_invoke(self, key: str, parameter: Any = None):
		return self.__send_json_request("POST", f"eval/{key}", parameter)

	def eval(self, ironpython_code: str):
		return self.__send_text_request("POST", "eval", ironpython_code, "text/ironpython")

	def eval_file(self, ironpython_file_path: str):
		with open(ironpython_file_path, "r") as file:
			file_content = file.read()

		return self.eval(file_content)


if __name__ == "__main__":
	linda = RemoteLinda("localhost", 8080)

	script = "linda.Out(('mutex', 1, 2, 3.1))"

	linda.eval_register("script", script)
	linda.eval_invoke("script")

	print(linda.in_(("mutex", 1, None, None)))
