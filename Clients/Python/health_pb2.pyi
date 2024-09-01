from google.protobuf import wrappers_pb2 as _wrappers_pb2
from google.protobuf import empty_pb2 as _empty_pb2
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Mapping as _Mapping, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class ScriptExecutionStatus(_message.Message):
    __slots__ = ("not_found", "ok", "exception")
    class Exception(_message.Message):
        __slots__ = ("message", "source", "stack_trace", "type")
        MESSAGE_FIELD_NUMBER: _ClassVar[int]
        SOURCE_FIELD_NUMBER: _ClassVar[int]
        STACK_TRACE_FIELD_NUMBER: _ClassVar[int]
        TYPE_FIELD_NUMBER: _ClassVar[int]
        message: str
        source: str
        stack_trace: str
        type: str
        def __init__(self, message: _Optional[str] = ..., source: _Optional[str] = ..., stack_trace: _Optional[str] = ..., type: _Optional[str] = ...) -> None: ...
    NOT_FOUND_FIELD_NUMBER: _ClassVar[int]
    OK_FIELD_NUMBER: _ClassVar[int]
    EXCEPTION_FIELD_NUMBER: _ClassVar[int]
    not_found: _empty_pb2.Empty
    ok: _empty_pb2.Empty
    exception: ScriptExecutionStatus.Exception
    def __init__(self, not_found: _Optional[_Union[_empty_pb2.Empty, _Mapping]] = ..., ok: _Optional[_Union[_empty_pb2.Empty, _Mapping]] = ..., exception: _Optional[_Union[ScriptExecutionStatus.Exception, _Mapping]] = ...) -> None: ...
