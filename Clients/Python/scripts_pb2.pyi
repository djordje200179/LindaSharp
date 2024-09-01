from google.protobuf import struct_pb2 as _struct_pb2
from google.protobuf import empty_pb2 as _empty_pb2
from google.protobuf.internal import enum_type_wrapper as _enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Mapping as _Mapping, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class Script(_message.Message):
    __slots__ = ("type", "code")
    class Type(int, metaclass=_enum_type_wrapper.EnumTypeWrapper):
        __slots__ = ()
        IRONPYTHON: _ClassVar[Script.Type]
        C_SHARP: _ClassVar[Script.Type]
    IRONPYTHON: Script.Type
    C_SHARP: Script.Type
    TYPE_FIELD_NUMBER: _ClassVar[int]
    CODE_FIELD_NUMBER: _ClassVar[int]
    type: Script.Type
    code: str
    def __init__(self, type: _Optional[_Union[Script.Type, str]] = ..., code: _Optional[str] = ...) -> None: ...

class RegisterScriptRequest(_message.Message):
    __slots__ = ("key", "script")
    KEY_FIELD_NUMBER: _ClassVar[int]
    SCRIPT_FIELD_NUMBER: _ClassVar[int]
    key: str
    script: Script
    def __init__(self, key: _Optional[str] = ..., script: _Optional[_Union[Script, _Mapping]] = ...) -> None: ...

class InvokeScriptRequest(_message.Message):
    __slots__ = ("key", "parameter")
    KEY_FIELD_NUMBER: _ClassVar[int]
    PARAMETER_FIELD_NUMBER: _ClassVar[int]
    key: str
    parameter: _struct_pb2.Value
    def __init__(self, key: _Optional[str] = ..., parameter: _Optional[_Union[_struct_pb2.Value, _Mapping]] = ...) -> None: ...

class EvalScriptRequest(_message.Message):
    __slots__ = ("script",)
    SCRIPT_FIELD_NUMBER: _ClassVar[int]
    script: Script
    def __init__(self, script: _Optional[_Union[Script, _Mapping]] = ...) -> None: ...

class EvalScriptResponse(_message.Message):
    __slots__ = ("task_id",)
    TASK_ID_FIELD_NUMBER: _ClassVar[int]
    task_id: int
    def __init__(self, task_id: _Optional[int] = ...) -> None: ...
