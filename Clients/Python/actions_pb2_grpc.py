# Generated by the gRPC Python protocol compiler plugin. DO NOT EDIT!
"""Client and server classes corresponding to protobuf-defined services."""
import grpc
import warnings

import actions_pb2 as actions__pb2
from google.protobuf import empty_pb2 as google_dot_protobuf_dot_empty__pb2

GRPC_GENERATED_VERSION = '1.65.4'
GRPC_VERSION = grpc.__version__
EXPECTED_ERROR_RELEASE = '1.66.0'
SCHEDULED_RELEASE_DATE = 'August 6, 2024'
_version_not_supported = False

try:
    from grpc._utilities import first_version_is_lower
    _version_not_supported = first_version_is_lower(GRPC_VERSION, GRPC_GENERATED_VERSION)
except ImportError:
    _version_not_supported = True

if _version_not_supported:
    warnings.warn(
        f'The grpc package installed is at version {GRPC_VERSION},'
        + f' but the generated code in actions_pb2_grpc.py depends on'
        + f' grpcio>={GRPC_GENERATED_VERSION}.'
        + f' Please upgrade your grpc module to grpcio>={GRPC_GENERATED_VERSION}'
        + f' or downgrade your generated code using grpcio-tools<={GRPC_VERSION}.'
        + f' This warning will become an error in {EXPECTED_ERROR_RELEASE},'
        + f' scheduled for release on {SCHEDULED_RELEASE_DATE}.',
        RuntimeWarning
    )


class ActionsStub(object):
    """Missing associated documentation comment in .proto file."""

    def __init__(self, channel):
        """Constructor.

        Args:
            channel: A grpc.Channel.
        """
        self.Out = channel.unary_unary(
                '/Actions/Out',
                request_serializer=actions__pb2.Tuple.SerializeToString,
                response_deserializer=google_dot_protobuf_dot_empty__pb2.Empty.FromString,
                _registered_method=True)
        self.In = channel.unary_unary(
                '/Actions/In',
                request_serializer=actions__pb2.Pattern.SerializeToString,
                response_deserializer=actions__pb2.Tuple.FromString,
                _registered_method=True)
        self.Rd = channel.unary_unary(
                '/Actions/Rd',
                request_serializer=actions__pb2.Pattern.SerializeToString,
                response_deserializer=actions__pb2.Tuple.FromString,
                _registered_method=True)
        self.Inp = channel.unary_unary(
                '/Actions/Inp',
                request_serializer=actions__pb2.Pattern.SerializeToString,
                response_deserializer=actions__pb2.OptionalTuple.FromString,
                _registered_method=True)
        self.Rdp = channel.unary_unary(
                '/Actions/Rdp',
                request_serializer=actions__pb2.Pattern.SerializeToString,
                response_deserializer=actions__pb2.OptionalTuple.FromString,
                _registered_method=True)


class ActionsServicer(object):
    """Missing associated documentation comment in .proto file."""

    def Out(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def In(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def Rd(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def Inp(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def Rdp(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')


def add_ActionsServicer_to_server(servicer, server):
    rpc_method_handlers = {
            'Out': grpc.unary_unary_rpc_method_handler(
                    servicer.Out,
                    request_deserializer=actions__pb2.Tuple.FromString,
                    response_serializer=google_dot_protobuf_dot_empty__pb2.Empty.SerializeToString,
            ),
            'In': grpc.unary_unary_rpc_method_handler(
                    servicer.In,
                    request_deserializer=actions__pb2.Pattern.FromString,
                    response_serializer=actions__pb2.Tuple.SerializeToString,
            ),
            'Rd': grpc.unary_unary_rpc_method_handler(
                    servicer.Rd,
                    request_deserializer=actions__pb2.Pattern.FromString,
                    response_serializer=actions__pb2.Tuple.SerializeToString,
            ),
            'Inp': grpc.unary_unary_rpc_method_handler(
                    servicer.Inp,
                    request_deserializer=actions__pb2.Pattern.FromString,
                    response_serializer=actions__pb2.OptionalTuple.SerializeToString,
            ),
            'Rdp': grpc.unary_unary_rpc_method_handler(
                    servicer.Rdp,
                    request_deserializer=actions__pb2.Pattern.FromString,
                    response_serializer=actions__pb2.OptionalTuple.SerializeToString,
            ),
    }
    generic_handler = grpc.method_handlers_generic_handler(
            'Actions', rpc_method_handlers)
    server.add_generic_rpc_handlers((generic_handler,))
    server.add_registered_method_handlers('Actions', rpc_method_handlers)


 # This class is part of an EXPERIMENTAL API.
class Actions(object):
    """Missing associated documentation comment in .proto file."""

    @staticmethod
    def Out(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/Actions/Out',
            actions__pb2.Tuple.SerializeToString,
            google_dot_protobuf_dot_empty__pb2.Empty.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def In(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/Actions/In',
            actions__pb2.Pattern.SerializeToString,
            actions__pb2.Tuple.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def Rd(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/Actions/Rd',
            actions__pb2.Pattern.SerializeToString,
            actions__pb2.Tuple.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def Inp(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/Actions/Inp',
            actions__pb2.Pattern.SerializeToString,
            actions__pb2.OptionalTuple.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def Rdp(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/Actions/Rdp',
            actions__pb2.Pattern.SerializeToString,
            actions__pb2.OptionalTuple.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)