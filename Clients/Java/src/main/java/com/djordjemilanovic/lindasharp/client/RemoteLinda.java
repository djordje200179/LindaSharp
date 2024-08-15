package com.djordjemilanovic.lindasharp.client;

import com.djordjemilanovic.lindasharp.services.ActionsGrpc;
import com.djordjemilanovic.lindasharp.services.ActionsOuterClass;
import com.djordjemilanovic.lindasharp.services.HealthGrpc;
import com.djordjemilanovic.lindasharp.services.ScriptsGrpc;
import io.grpc.*;
import com.google.protobuf.Empty;

import java.util.Optional;

public class RemoteLinda {
	private final ActionsGrpc.ActionsBlockingStub actionsStub;
	private final HealthGrpc.HealthBlockingStub healthStub;
	private final ScriptsGrpc.ScriptsBlockingStub scriptsStub;

	public RemoteLinda(String hostname, int port) {
		var channel = ManagedChannelBuilder.forAddress(hostname, port).usePlaintext().build();
		actionsStub = ActionsGrpc.newBlockingStub(channel);
		healthStub = HealthGrpc.newBlockingStub(channel);
		scriptsStub = ScriptsGrpc.newBlockingStub(channel);
	}

	public void put(Object... tuple) {
		actionsStub.out(MessageConversions.ToGrpcTuple(tuple));
	}

	public Object[] get(Object... pattern) {
		return MessageConversions.ToLindaTuple(actionsStub.in(MessageConversions.ToGrpcPattern(pattern)));
	}

	public Object[] query(Object... pattern) {
		return MessageConversions.ToLindaTuple(actionsStub.rd(MessageConversions.ToGrpcPattern(pattern)));
	}

	public Optional<Object[]> tryGet(Object... pattern) {
		return actionsStub.inp(MessageConversions.ToGrpcPattern(pattern)).getTuple() instanceof ActionsOuterClass.Tuple tuple
		    ? Optional.of(MessageConversions.ToLindaTuple(tuple))
		    : Optional.empty();
	}

	public Optional<Object[]> tryQuery(Object... pattern) {
		return actionsStub.rdp(MessageConversions.ToGrpcPattern(pattern)).getTuple() instanceof ActionsOuterClass.Tuple tuple
		    ? Optional.of(MessageConversions.ToLindaTuple(tuple))
		    : Optional.empty();
	}

	public boolean isHealthy() {
		try {
			var result = healthStub.ping(Empty.getDefaultInstance());
			return result.getValue().equals("pong");
		} catch (Exception e) {
			return false;
		}
	}
}
