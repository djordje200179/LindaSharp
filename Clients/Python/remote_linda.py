import requests


class RemoteLinda:
    def __init__(self, host: str, port: int):
        self.__base_url = f"http://{host}:{port}/"

    def __send_request(self, http_method: str, linda_method: str, data: any):
        url = self.__base_url + linda_method
        response = requests.request(http_method, url, json=data)

        if response.status_code == requests.codes.server_error:
            raise Exception("Server Linda was disposed")

        return response

    def __wait_tuple(self, tuple_pattern: list, method: str) -> list:
        response = self.__send_request("GET", method, tuple_pattern)
        return response.json()

    def __try_get_tuple(self, tuple_pattern: list, method: str) -> list:
        response = self.__send_request("GET", method, tuple_pattern)

        if response.status_code == requests.codes.not_found:
            return None

        return response.json()

    def out(self, tuple: list):
        self.__send_request("POST", "out", tuple)

    def in_(self, tuple_pattern: list) -> list:
        return self.__wait_tuple(tuple_pattern, "in")

    def rd(self, tuple_pattern: list) -> list:
        return self.__wait_tuple(tuple_pattern, "rd")

    def inp(self, tuple_pattern: list) -> list:
        return self.__try_get_tuple(tuple_pattern, "inp")

    def rdp(self, tuple_pattern: list) -> list:
        return self.__try_get_tuple(tuple_pattern, "rdp")


if __name__ == "__main__":
    linda = RemoteLinda("localhost", 8080)

    linda.out([1, 2, 3])

    print(linda.in_([1, None, 3]))
