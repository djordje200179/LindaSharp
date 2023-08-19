class RemoteLinda {
	private baseUrl: string;

	public constructor(host: string, port: number) {
		this.baseUrl = `http://${host}:${port}/actions/`
	}

	private sendTextRequest(method: string, path: string, data: any, contentType: string) {
		
	}

	private sendJsonRequest(method: string, path: string, data: any) {

	}

	private waitTuple(tuplePattern: any[], del: boolean): any[] {

	}

	private tryGetTuple(tuplePattern: any[], del: boolean): any[] | null {

	}

	public out(tuple: any[]): void {
		return this.sendJsonRequest("POST", "out", tuple);
	}

	public in(tuplePattern: any[]): any[] {
		return this.waitTuple(tuplePattern, true);
	}

	public rd(tuplePattern: any[]): any[] {
		return this.waitTuple(tuplePattern, false);
	}

	public inp(tuplePattern: any[]): any[] | null {
		return this.tryGetTuple(tuplePattern, true);
	}

	public rdp(tuplePattern: any[]): any[] | null {
		return this.tryGetTuple(tuplePattern, false);
	}

	public evalRegister(key: string, ironpythonCode: string): void {
		return this.sendTextRequest("PUT", `eval/${key}`, ironpythonCode, "text/ironpython");
	}

	public evalInvoke(key: string, parameter: any = null): void {
		return this.sendJsonRequest("POST", `eval/${key}`, parameter);
	}

	public eval(ironpythonCode: string): void {
		return this.sendTextRequest("POST", `eval`, ironpythonCode, "text/ironpython");
	}
}